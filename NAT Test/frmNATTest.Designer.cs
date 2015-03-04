namespace NAT_Test
{
    partial class frmNATTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNATTest));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.txtUDPPort = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.lblPortType = new System.Windows.Forms.Label();
            this.lblNAT = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnUPnPRep = new System.Windows.Forms.Button();
            this.cboUPnPList = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboUseUPnP = new System.Windows.Forms.ComboBox();
            this.btnClipboard = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Test Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "UDP port:";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(74, 12);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(159, 20);
            this.txtHost.TabIndex = 2;
            this.txtHost.Text = "crantime.org:6000";
            // 
            // txtUDPPort
            // 
            this.txtUDPPort.Location = new System.Drawing.Point(74, 38);
            this.txtUDPPort.Name = "txtUDPPort";
            this.txtUDPPort.Size = new System.Drawing.Size(82, 20);
            this.txtUDPPort.TabIndex = 3;
            this.txtUDPPort.Text = "5029";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(98, 74);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutput.Location = new System.Drawing.Point(17, 103);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(755, 246);
            this.txtOutput.TabIndex = 5;
            // 
            // lblPortType
            // 
            this.lblPortType.AutoSize = true;
            this.lblPortType.Location = new System.Drawing.Point(179, 79);
            this.lblPortType.Name = "lblPortType";
            this.lblPortType.Size = new System.Drawing.Size(100, 13);
            this.lblPortType.TabIndex = 6;
            this.lblPortType.Text = "Port type: Waiting...";
            // 
            // lblNAT
            // 
            this.lblNAT.AutoSize = true;
            this.lblNAT.Location = new System.Drawing.Point(334, 79);
            this.lblNAT.Name = "lblNAT";
            this.lblNAT.Size = new System.Drawing.Size(80, 13);
            this.lblNAT.TabIndex = 7;
            this.lblNAT.Text = "NAT: Waiting...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(163, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "0 = Random";
            // 
            // btnUPnPRep
            // 
            this.btnUPnPRep.Location = new System.Drawing.Point(17, 74);
            this.btnUPnPRep.Name = "btnUPnPRep";
            this.btnUPnPRep.Size = new System.Drawing.Size(75, 23);
            this.btnUPnPRep.TabIndex = 9;
            this.btnUPnPRep.Text = "UPnP report";
            this.btnUPnPRep.UseVisualStyleBackColor = true;
            this.btnUPnPRep.Click += new System.EventHandler(this.btnUPnPRep_Click);
            // 
            // cboUPnPList
            // 
            this.cboUPnPList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboUPnPList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUPnPList.Enabled = false;
            this.cboUPnPList.FormattingEnabled = true;
            this.cboUPnPList.Location = new System.Drawing.Point(479, 38);
            this.cboUPnPList.Name = "cboUPnPList";
            this.cboUPnPList.Size = new System.Drawing.Size(293, 21);
            this.cboUPnPList.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(380, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "UPnP Device list: ";
            // 
            // cboUseUPnP
            // 
            this.cboUseUPnP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboUseUPnP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUseUPnP.FormattingEnabled = true;
            this.cboUseUPnP.Items.AddRange(new object[] {
            "No",
            "Yes",
            "All"});
            this.cboUseUPnP.Location = new System.Drawing.Point(651, 12);
            this.cboUseUPnP.Name = "cboUseUPnP";
            this.cboUseUPnP.Size = new System.Drawing.Size(121, 21);
            this.cboUseUPnP.TabIndex = 12;
            this.cboUseUPnP.SelectedIndexChanged += new System.EventHandler(this.cboUseUPnP_SelectedIndexChanged);
            // 
            // btnClipboard
            // 
            this.btnClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClipboard.Image = ((System.Drawing.Image)(resources.GetObject("btnClipboard.Image")));
            this.btnClipboard.Location = new System.Drawing.Point(745, 74);
            this.btnClipboard.Name = "btnClipboard";
            this.btnClipboard.Size = new System.Drawing.Size(27, 23);
            this.btnClipboard.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btnClipboard, "Copy text to Clipboard");
            this.btnClipboard.UseVisualStyleBackColor = true;
            this.btnClipboard.Click += new System.EventHandler(this.btnClipboard_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearAll.Image = ((System.Drawing.Image)(resources.GetObject("btnClearAll.Image")));
            this.btnClearAll.Location = new System.Drawing.Point(712, 74);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(27, 23);
            this.btnClearAll.TabIndex = 14;
            this.toolTip1.SetToolTip(this.btnClearAll, "Clear all text");
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(585, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Use UPnP:";
            // 
            // frmNATTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnClipboard);
            this.Controls.Add(this.cboUseUPnP);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboUPnPList);
            this.Controls.Add(this.btnUPnPRep);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblNAT);
            this.Controls.Add(this.lblPortType);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.txtUDPPort);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(660, 240);
            this.Name = "frmNATTest";
            this.Text = "NAT Test";
            this.Load += new System.EventHandler(this.frmNATTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.TextBox txtUDPPort;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label lblPortType;
        private System.Windows.Forms.Label lblNAT;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnUPnPRep;
        private System.Windows.Forms.ComboBox cboUPnPList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboUseUPnP;
        private System.Windows.Forms.Button btnClipboard;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
    }
}

