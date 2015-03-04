﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace NAT_Test_Host__Console_
{
    class Program
    {
        //string hostname;
        //short hostport;
        static ushort localport;

        static UdpClient hostSocket;

        static IPEndPoint RemoteIpEndPoint;

        const Byte PortO_Normal = 0;
        const Byte PortO_Random = 1;
        const Byte PortR_Normal = 2;
        const Byte PortR_Random = 3;

        static TextWriter logfile;

        struct Response
        {
            public Byte num;
            public Byte remotetype;
            public ushort toPort;
            public ushort ranPort;

            public Response(Byte _num, Byte _remotetype, ushort _toPort, ushort _ranPort)
            {
                num = _num;
                remotetype = _remotetype;
                toPort = _toPort;
                ranPort = _ranPort;
            }
        }

        static void Main(string[] args)
        {
            //txtUDPPort.Enabled = btnListen.Enabled = false;
            //btnListen.Text = "Listening...";
            logfile = File.CreateText("nattesthost.log");
            Host_Listen(6000);
        }

        static void Host_Listen(ushort _localport)
        {
            localport = _localport;
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, localport);
            hostSocket = new UdpClient(RemoteIpEndPoint);
            Output("hostSocket bound to " + RemoteIpEndPoint);

            try
            {
                hostSocket.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception e)
            {
                Output(e.ToString());
                return;
            }

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape);
        }

        // CallBack
        static void recv(IAsyncResult res)
        {
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, localport);
                byte[] buffer = hostSocket.EndReceive(res, ref RemoteIpEndPoint);

                Thread thread = new Thread(() => Host_Respond(buffer, RemoteIpEndPoint));
                thread.Start();
                Output("recv: spawned thread " + thread.ToString());
            }
            catch (Exception e)
            {
                Output("hostSocket: " + e.Message);
            }

            hostSocket.BeginReceive(new AsyncCallback(recv), null);
        }

        // Begin post back test
        static void Host_Respond(byte[] buffer, IPEndPoint RemoteIpEndPoint)
        {
            UdpClient randomSocket = new UdpClient(0, AddressFamily.InterNetwork);

            bool Redirect;
            ushort remoteportHost = 0;
            ushort remoteportEndpoint = 0;
            ushort localportRandom = Convert.ToUInt16(((IPEndPoint)randomSocket.Client.LocalEndPoint).Port);

            //Process codes
            Output("hostSocket: received data from " + RemoteIpEndPoint + " " + ByteArrayToString(buffer));

            remoteportHost = GetUShort(buffer);
            if (remoteportHost == 0)
            {
                Output("hostSocket: errant keepalive from " + RemoteIpEndPoint);
                return;
            }

            Output("Endpoint " + RemoteIpEndPoint + ": remoteportHost is " + remoteportHost);
            remoteportEndpoint = Convert.ToUInt16(RemoteIpEndPoint.Port);
            Output("Endpoint " + RemoteIpEndPoint + ": remoteportEndpoint is " + remoteportEndpoint);
            Redirect = (remoteportHost != remoteportEndpoint);
            Output("Endpoint " + RemoteIpEndPoint + ": Hole punch is " + Redirect.ToString());

            for (int i = 0; i < 10; i++)
            {
                // Orginal suggested port
                if (!Redirect)
                {
                    buffer = GetBytes(new Response((Byte)i, PortO_Normal, remoteportHost, localportRandom));
                    hostSocket.Send(buffer, buffer.Length, RemoteIpEndPoint.Address.ToString(), remoteportHost);
                    Output("Normal - Endpoint " + RemoteIpEndPoint + ": Sent to " + remoteportHost + " " + ByteArrayToString(buffer));

                    buffer = GetBytes(new Response((Byte)i, PortO_Random, remoteportHost, localportRandom));
                    randomSocket.Send(buffer, buffer.Length, RemoteIpEndPoint.Address.ToString(), remoteportHost);
                    Output("Random - Endpoint " + RemoteIpEndPoint + ": Sent to " + remoteportHost + " " + ByteArrayToString(buffer));
                }

                // Redirected port
                else
                {
                    buffer = GetBytes(new Response((Byte)i, PortR_Normal, remoteportEndpoint, localportRandom));
                    hostSocket.Send(buffer, buffer.Length, RemoteIpEndPoint.Address.ToString(), remoteportEndpoint);
                    Output("Redirect Normal - Endpoint " + RemoteIpEndPoint + ": Sent to " + remoteportEndpoint + " " + ByteArrayToString(buffer));

                    buffer = GetBytes(new Response((Byte)i, PortR_Random, remoteportEndpoint, localportRandom));
                    randomSocket.Send(buffer, buffer.Length, RemoteIpEndPoint.Address.ToString(), remoteportEndpoint);
                    Output("Redirect Random - Endpoint " + RemoteIpEndPoint + ": Sent to " + remoteportEndpoint + " " + ByteArrayToString(buffer));
                }
                Thread.Sleep(1000);
            }
        }

        static void Output(string text)
        {
            Console.Write(text + System.Environment.NewLine);
            logfile.Write(text + System.Environment.NewLine);
            logfile.Flush();
        }

        static byte[] GetBytes(ushort bit16)
        {
            return BitConverter.GetBytes(bit16);
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
    }
}
