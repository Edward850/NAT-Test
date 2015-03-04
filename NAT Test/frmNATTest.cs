using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Xml;
using System.IO;

namespace NAT_Test
{
    public partial class frmNATTest : Form
    {
        string hostname;
        ushort hostport;
        ushort localport;

        int gotPortType; // 0 Expected, 1 Redirected
        int NATType; // 0 Strict, 1 Fair, 2 Open
        int NoRandom;
        bool anyresults;

        System.Windows.Forms.Timer timeout;
        object statuslock = new Object();

        UdpClient testSocket;

        //IPEndPoint RemoteIpEndPoint;

        const Byte PortO_Normal = 0;
        const Byte PortO_Random = 1;
        const Byte PortR_Normal = 2;
        const Byte PortR_Random = 3;

        struct Response
        {
            public Byte num;
            public Byte remotetype;
            public ushort toPort;
            public ushort ranPort;
        }

        public frmNATTest()
        {
            InitializeComponent();
        }

        private String GetHostAddress ()
        {
            IPAddress[] ips;

            string lhostname = Dns.GetHostName();
            ips = Dns.GetHostAddresses(lhostname);
            foreach (IPAddress ip in ips)
            {
                if(ip.ToString().Contains('.'))
                {
                    Output("GetHostAddress(" + lhostname + ") returns: " + ip);
                    return ip.ToString();
                }
            }

            Output("GetHostAddress(" + lhostname + ") failed to find valid IP!");
            return "";
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Lock_Controls(true);
            btnTest.Text = "Contacting...";
            try
            {
                Test_Contact(txtHost.Text, Convert.ToUInt16(txtUDPPort.Text));
            }
            catch (FormatException ex)
            {
                Test_End(false);
                Output("Failed to start becuase of UDP port entry: " + ex.Message);
            }
        }

        private void Lock_Controls (bool lockme)
        {
            cboUPnPList.Enabled = cboUseUPnP.Enabled = btnUPnPRep.Enabled = txtHost.Enabled = txtUDPPort.Enabled = btnTest.Enabled = !lockme;
        }

        private void Test_End(bool good)
        {
            if (InvokeRequired)
                Invoke(new Action<bool>(Test_End), good);
            else
            {
                lock (statuslock)
                {
                    if (devicelist != null)
                    {
                        for (int i = 0; i < devicelist.Length; i++)
                        {
                            DeleteForwardingRule(ref devicelist[i], localport, ProtocolType.Udp);
                        }
                    }

                    if (NoRandom == 1)
                    {
                        NATType = -2;
                        Status(gotPortType, NATType);
                    }

                    if (!good) Status(-1, -1);
                    Lock_Controls(false);
                    btnTest.Text = "Test";
                    testSocket.Close();
                    timeout.Stop();

                    String typestr = "I am a teapot!";
                    switch (gotPortType)
                    {
                        case 0: typestr = "Expected"; break;
                        case 1: typestr = "Redirected"; break;
                    }
                    String natstr = "Huh?";
                    switch (NATType)
                    {
                        case -2: natstr = "Strict+ <!>"; break;
                        case 0: natstr = "Strict"; break;
                        case 1: natstr = "Fair"; break;
                        case 2: natstr = "Open"; break;
                    }

                    if (good && anyresults)
                    {
                        Output("Test results: Port type: " + typestr + " - NAT type: " + natstr);
                    }
                }
            }
        }

        private void Test_Contact(string host, ushort _localport)
        {
            try
            {
                Output(" --- --- --- --- --- ");
                Output(" --- --- --- --- --- ");
                Output(" --- --- --- --- --- ");

                lock (statuslock)
                {
                    gotPortType = 1;
                    NATType = 0;
                    NoRandom = 0;
                    anyresults = false;
                    timeout = new System.Windows.Forms.Timer();
                    timeout.Interval = 1000 * 15;
                    timeout.Tick += OnTimeout;
                    timeout.Start();
                }

                localport = _localport;
                testSocket = new UdpClient(localport);
                if (localport == 0)
                {
                    localport = Convert.ToUInt16(((IPEndPoint)testSocket.Client.LocalEndPoint).Port);
                    txtUDPPort.Text = localport.ToString();
                }

                if (devicelist != null)
                {
                    if (cboUseUPnP.SelectedIndex > 0 && devicelist.Length > 0)
                    {
                        if (cboUseUPnP.SelectedIndex == 1)
                            ForwardPort(ref devicelist[cboUPnPList.SelectedIndex], localport, ProtocolType.Udp, "ZDO NAT Test");
                        else if (cboUseUPnP.SelectedIndex == 2)
                        {
                            for (int i = 0; i < devicelist.Length; i++)
                            {
                                ForwardPort(ref devicelist[i], localport, ProtocolType.Udp, "ZDO NAT Test");
                            }
                        }
                    }
                }

                Output("testSocket bound to " + localport);
                Byte[] buffer = GetBytes(localport);

                hostname = host.Split(':')[0];
                hostport = Convert.ToUInt16(host.Split(':')[1]);

                testSocket.Send(buffer, buffer.Length, hostname, hostport);
                Output("testSocket: Sent request " + ByteArrayToString(buffer));

                testSocket.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception e)
            {
                Test_End(false);
                Output("testSocket init: " + e.Message);
            }
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            Test_End(true);
            Output("testSocket finished: Finish after 15 seconds.");
        }

        // CallBack
        private void recv(IAsyncResult res)
        {
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, localport);
                byte[] buffer = testSocket.EndReceive(res, ref RemoteIpEndPoint);

                Test_Responce(buffer, RemoteIpEndPoint);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception e)
            {
                Test_End(true);
                Output("testSocket EndReceive: " + e.Message);
            }

            try
            {
                testSocket.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                Test_End(true);
                Output("testSocket BeginReceive: " + ex.Message);
            }
        }

        // Begin post back test
        private void Test_Responce(byte[] buffer, IPEndPoint RemoteIpEndPoint)
        {
            bool WasRedirected;
            bool TrueHost;
            bool RandomPort;

            Output("testSocket: received data from " + RemoteIpEndPoint + " " + ByteArrayToString(buffer));

            Response data = GetResponse(buffer);
            Output("Data " + data.num + ": toPort is " + data.toPort + " (local port is " + localport + ")");
            WasRedirected = (data.remotetype == PortR_Normal || data.remotetype == PortR_Random);
            TrueHost = (data.remotetype == PortO_Random);
            RandomPort = (data.remotetype == PortO_Random || data.remotetype == PortR_Random);
            Output("Data " + data.num + ": Message Redirection: " + WasRedirected.ToString());
            Output("Data " + data.num + ": True Host: " + TrueHost.ToString());
            Output("Data " + data.num + ": Random Host: " + RandomPort.ToString());

            lock (statuslock)
            {
                anyresults = true;

                if (gotPortType == 1 && !WasRedirected)
                    gotPortType = 0;

                if (NATType == 0 && NoRandom > 0 && RandomPort)
                {
                    NATType = 1;
                    NoRandom = -1;
                }

                if (NATType == 0 && TrueHost)
                    NATType = 2;

                Status(gotPortType, NATType);

                if (NATType == 0 && data.num >= 4)
                {
                    buffer = GetBytes(0);
                    testSocket.Send(buffer, buffer.Length, RemoteIpEndPoint.Address.ToString(), data.ranPort);
                    Output("testSocket: Sent outbound punch " + ByteArrayToString(buffer));
                    NoRandom++;
                }
            }
        }

        private void Output(string text)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(Output), text);
            else
                txtOutput.AppendText(text + System.Environment.NewLine);
        }

        private void Status(int PortType, int NAT)
        {
            if (InvokeRequired)
                Invoke(new Action<int, int>(Status), PortType, NAT);
            else
            {
                String typestr = "I am a teapot!";
                switch (PortType)
                {
                    case -1: typestr = "Waiting..."; break;
                    case 0: typestr = "Expected"; break;
                    case 1: typestr = "Redirected"; break;
                }
                String natstr = "Huh?";
                switch (NAT)
                {
                    case -2: natstr = "Strict+ <!>"; break;
                    case -1: natstr = "Waiting..."; break;
                    case 0: natstr = "Strict"; break;
                    case 1: natstr = "Fair"; break;
                    case 2: natstr = "Open"; break;
                }
                lblPortType.Text = "Port type: " + typestr;
                lblNAT.Text = "NAT: " + natstr;
                btnTest.Text = "Receiving...";
            }
        }

        static byte[] GetBytes(ushort bit16)
        {
            return BitConverter.GetBytes(bit16);
        }

        static byte[] GetBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        static byte[] GetBytes(Response data)
        {
            byte[] buffer = new byte[sizeof(Byte) * 2 + sizeof(ushort) * 2];
            buffer[0] = data.num;
            buffer[1] = data.remotetype;
            BitConverter.GetBytes(data.toPort).CopyTo(buffer, 2);
            BitConverter.GetBytes(data.ranPort).CopyTo(buffer, 4);
            return buffer;
        }

        static ushort GetUShort(byte[] buffer)
        {
            return BitConverter.ToUInt16(buffer, 0);
        }

        static string GetString(byte[] buffer)
        {
            return Encoding.ASCII.GetString(buffer);
        }

        static Response GetResponse(byte[] buffer)
        {
            Response data = new Response();
            data.num = buffer[0];
            data.remotetype = buffer[1];
            data.toPort = BitConverter.ToUInt16(buffer, 2);
            data.ranPort = BitConverter.ToUInt16(buffer, 4);
            return data;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return '[' + hex.ToString().ToUpper() + ']';
        }

        const string UPnP_Broadcast = "M-SEARCH * HTTP/1.1\r\n" + "HOST: 239.255.255.250:1900\r\n" + "ST:upnp:rootdevice\r\n" + "MAN:\"ssdp:discover\"\r\n" + "MX:3\r\n\r\n";
        UdpClient UPnPSocket;

        struct UPnPDevice
        {
            public String Name;
            public String Location;
            public String Event;
            public String Control;
            public Boolean Used;
            public UPnPDevice(String _Name, String _Location, String _Event, String _Control)
            {
                Name = _Name;
                Location = _Location;
                Event = _Event;
                Control = _Control;
                Used = false;
            }
        }

        UPnPDevice[] devicelist;

        private void UPnPListUpdate(UPnPDevice[] devices)
        {
            if (InvokeRequired)
                Invoke(new Action<UPnPDevice[]>(UPnPListUpdate), devices);
            else
            {
                devicelist = devices;
                cboUPnPList.Items.Clear();
                cboUPnPList.Enabled = false;
                foreach (UPnPDevice dev in devicelist)
                {
                    cboUPnPList.Items.Add(dev.Name + " : " + GetExternalIP(dev));
                }
                if (devicelist.Length >= 1)
                {
                    cboUPnPList.SelectedIndex = 0;
                    cboUPnPList.Enabled = true;
                }
                else
                {
                    cboUPnPList.Text = "No UPnP Devices found!";
                }
                Lock_Controls(false);
            }
        }

        private void btnUPnPRep_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() => UPnP_Start());
            thread.Start();
            Lock_Controls(true);
        }

        private void UPnP_Start()
        {
            UPnPSocket = new UdpClient();
            UPnPSocket.Client.Blocking = false;
            byte[] buffer = GetBytes(UPnP_Broadcast);
            UPnPSocket.Send(buffer, buffer.Length, IPAddress.Broadcast.ToString(), 1900);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 1900);

            List<String> Responses = new List<String>(0);
            List<UPnPDevice> Gateways = new List<UPnPDevice>(0);

            bool keep = true;
            while (keep)
            {
                keep = false;
                Thread.Sleep(3000);
                try
                {
                    buffer = UPnPSocket.Receive(ref RemoteIpEndPoint);
                    while (buffer.Length > 0)
                    {
                        keep = true;
                        Responses.Add(GetString(buffer));
                        Output(GetString(buffer));
                        buffer = UPnPSocket.Receive(ref RemoteIpEndPoint);
                    }
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode != 10035) // Blocking exception
                    {
                        Output(e.Message);
                        keep = false;
                    }
                }
            }

            try
            {
                string type, name, location;
                foreach (String res in Responses)
                {
                    if (res.StartsWith("HTTP/1.1 200 OK"))
                    {
                        type = FindStringProperty(res, "ST:").ToLower();
                        name = FindStringProperty(res, "Server:");
                        location = FindStringProperty(res, "Location:");

                        if (type == "upnp:rootdevice" &&
                            name != String.Empty && location != String.Empty)
                        {
                            UPnPDevice? newdev = BuildUPnPDevice(name, location);
                            if (newdev != null)
                                Gateways.Add((UPnPDevice)newdev);
                        }
                    }
                }

                UPnPListUpdate(Gateways.ToArray());
            }
            catch (Exception e)
            {
                Output(e.ToString());
                Gateways.Clear();
                UPnPListUpdate(Gateways.ToArray());
            }
        }

        private UPnPDevice? BuildUPnPDevice (String name, String location)
        {
            try
            {
                XmlDocument xmldoc;
                XmlNamespaceManager xmlnamespace;

                xmldoc = new XmlDocument();
                xmldoc.Load(WebRequest.Create(location).GetResponse().GetResponseStream());

                xmlnamespace = new XmlNamespaceManager(xmldoc.NameTable);
                xmlnamespace.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");

                XmlNode typen = xmldoc.SelectSingleNode("//tns:device/tns:deviceType/text()", xmlnamespace);
                if (!typen.Value.Contains("InternetGatewayDevice"))
                    return null;

                XmlNode node = xmldoc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", xmlnamespace);
                if (node == null)
                    return null;

                XmlNode eventnode = xmldoc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:eventSubURL/text()", xmlnamespace);

                return new UPnPDevice(name, location, CombineUrls(location, eventnode.Value), CombineUrls(location, node.Value));
            }
            catch (Exception e)
            {
                Output("Error during XML process on UPnP device " + name + " - " + location + " :: (Likely not a NAT UPnP) " + e.Message);
                return null;
            }
        }

        private String FindStringProperty(string input, string find)
        {
            if (input == null || input == String.Empty || find == null || find == String.Empty)
                return String.Empty;

            String retinput = input;
            input = input.ToLower();
            find = find.ToLower();

            int end = -1, start = input.IndexOf(find);
            if (start != -1)
            {
                start += find.Length;
                end = input.IndexOf("\r\n", start);
            }
            if (end != -1)
            {
                return retinput.Substring(start, end - start).Trim();
            }
            return String.Empty;
        }

        private static string CombineUrls(string resp, string p)
        {
            int n = resp.IndexOf("://");
            n = resp.IndexOf('/', n + 3);
            return resp.Substring(0, n) + p;
        }

        private void ForwardPort(ref UPnPDevice device, int port, ProtocolType protocol, string description)
        {
            try
            {
                XmlDocument xdoc = SOAPRequest(device.Control, "<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
                    "<NewRemoteHost></NewRemoteHost><NewExternalPort>" + port.ToString() + "</NewExternalPort><NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol>" +
                    "<NewInternalPort>" + port.ToString() + "</NewInternalPort><NewInternalClient>" + GetHostAddress() +
                    "</NewInternalClient><NewEnabled>1</NewEnabled><NewPortMappingDescription>" + description +
                    "</NewPortMappingDescription><NewLeaseDuration>0</NewLeaseDuration></u:AddPortMapping>", "AddPortMapping");
                Output("UPnP: ForwardPort (" + port + " " + protocol.ToString().ToUpper() + ") on " + device.Name);
                device.Used = true;
            }
            catch (Exception e)
            {
                Output("UPnP: ForwardPort Failure (" + port + " " + protocol.ToString().ToUpper() + ") on " + device.Name + " (" + e.Message + ")");
            }

        }

        private void DeleteForwardingRule(ref UPnPDevice device, int port, ProtocolType protocol)
        {
            try
            {
                if(!device.Used)
                {
                    Output("UPnP: DeleteForwardingRule Ignored (" + port + " " + protocol.ToString().ToUpper() + ") on " + device.Name);
                    return;
                }
                XmlDocument xdoc = SOAPRequest(device.Control,
                "<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
                "<NewRemoteHost>" +
                "</NewRemoteHost>" +
                "<NewExternalPort>" + port + "</NewExternalPort>" +
                "<NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol>" +
                "</u:DeletePortMapping>", "DeletePortMapping");
                Output("UPnP: DeleteForwardingRule (" + port + " " + protocol.ToString().ToUpper() + ") on " + device.Name);
            }
            catch (Exception e)
            {
                Output("UPnP: DeleteForwardingRule Failure (" + port + " " + protocol.ToString().ToUpper() + ") on " + device.Name + " (" + e.Message + ")");
            }
            device.Used = false;
        }

        private static IPAddress GetExternalIP(UPnPDevice device)
        {
            XmlDocument xdoc = SOAPRequest(device.Control, "<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
            "</u:GetExternalIPAddress>", "GetExternalIPAddress");
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
            nsMgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
            string IP = xdoc.SelectSingleNode("//NewExternalIPAddress/text()", nsMgr).Value;
            return IPAddress.Parse(IP);
        }

        private static XmlDocument SOAPRequest(string url, string soap, string function)
        {
            string req = "<?xml version=\"1.0\"?>" +
            "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
            "<s:Body>" +
            soap +
            "</s:Body>" +
            "</s:Envelope>";
            WebRequest r = HttpWebRequest.Create(url);
            r.Method = "POST";
            byte[] b = Encoding.UTF8.GetBytes(req);
            r.Headers.Add("SOAPACTION", "\"urn:schemas-upnp-org:service:WANIPConnection:1#" + function + "\"");
            r.ContentType = "text/xml; charset=\"utf-8\"";
            r.ContentLength = b.Length;
            r.GetRequestStream().Write(b, 0, b.Length);
            XmlDocument resp = new XmlDocument();
            WebResponse wres = r.GetResponse();
            Stream ress = wres.GetResponseStream();
            resp.Load(ress);
            return resp;
        }

        private void frmNATTest_Load(object sender, EventArgs e)
        {
            cboUseUPnP.SelectedItem = 0;
            cboUseUPnP.Text = "No";
        }

        private void cboUseUPnP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboUseUPnP.SelectedIndex != 0 && devicelist == null)
            {
                btnUPnPRep_Click(sender, e);
            }
        }

        private void btnClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
        }
    }
}
