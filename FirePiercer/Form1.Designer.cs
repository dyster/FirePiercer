namespace FirePiercer
{
    partial class Form1
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
            sonesson_tools.TCP.Stats stats1 = new sonesson_tools.TCP.Stats();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.buttonStartClient = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSendClient = new System.Windows.Forms.Button();
            this.timerLong = new System.Windows.Forms.Timer(this.components);
            this.buttonRemoteDesk = new System.Windows.Forms.Button();
            this.buttonTestP = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelSenderStatusUpdate = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxLogging = new System.Windows.Forms.CheckBox();
            this.textBoxConnectIP = new System.Windows.Forms.TextBox();
            this.textBoxConnectPort = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fastObjectListView1 = new BrightIdeasSoftware.FastObjectListView();
            this._strumpServer = new sonesson_tools.Strump.StrumpServer();
            this.olvColumnId = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSent = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnRec = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnIp = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAddress = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timerFlicker = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(735, 551);
            this.listBox1.TabIndex = 0;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBox1_DrawItem);
            // 
            // buttonStartClient
            // 
            this.buttonStartClient.Location = new System.Drawing.Point(12, 81);
            this.buttonStartClient.Name = "buttonStartClient";
            this.buttonStartClient.Size = new System.Drawing.Size(75, 23);
            this.buttonStartClient.TabIndex = 3;
            this.buttonStartClient.Text = "Start Client";
            this.buttonStartClient.UseVisualStyleBackColor = true;
            this.buttonStartClient.Click += new System.EventHandler(this.buttonStartClient_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(204, 20);
            this.textBox1.TabIndex = 4;
            // 
            // buttonSendClient
            // 
            this.buttonSendClient.Location = new System.Drawing.Point(222, 10);
            this.buttonSendClient.Name = "buttonSendClient";
            this.buttonSendClient.Size = new System.Drawing.Size(145, 23);
            this.buttonSendClient.TabIndex = 6;
            this.buttonSendClient.Text = "Send Msg from client";
            this.buttonSendClient.UseVisualStyleBackColor = true;
            this.buttonSendClient.Click += new System.EventHandler(this.buttonSendClient_Click);
            // 
            // timerLong
            // 
            this.timerLong.Interval = 2000;
            this.timerLong.Tick += new System.EventHandler(this.timerFlicker_Tick);
            // 
            // buttonRemoteDesk
            // 
            this.buttonRemoteDesk.Location = new System.Drawing.Point(401, 12);
            this.buttonRemoteDesk.Name = "buttonRemoteDesk";
            this.buttonRemoteDesk.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoteDesk.TabIndex = 10;
            this.buttonRemoteDesk.Text = "Remote Desk";
            this.buttonRemoteDesk.UseVisualStyleBackColor = true;
            this.buttonRemoteDesk.Click += new System.EventHandler(this.ButtonRemoteDesk_Click);
            // 
            // buttonTestP
            // 
            this.buttonTestP.Location = new System.Drawing.Point(548, 103);
            this.buttonTestP.Name = "buttonTestP";
            this.buttonTestP.Size = new System.Drawing.Size(75, 23);
            this.buttonTestP.TabIndex = 12;
            this.buttonTestP.Text = "Test";
            this.buttonTestP.UseVisualStyleBackColor = true;
            this.buttonTestP.Click += new System.EventHandler(this.ButtonTestP_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Monospac821 BT", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 14;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Monospac821 BT", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 42);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 14);
            this.label3.TabIndex = 15;
            this.label3.Text = "label3";
            // 
            // labelSenderStatusUpdate
            // 
            this.labelSenderStatusUpdate.AutoSize = true;
            this.labelSenderStatusUpdate.Location = new System.Drawing.Point(3, 61);
            this.labelSenderStatusUpdate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelSenderStatusUpdate.Name = "labelSenderStatusUpdate";
            this.labelSenderStatusUpdate.Size = new System.Drawing.Size(128, 13);
            this.labelSenderStatusUpdate.TabIndex = 16;
            this.labelSenderStatusUpdate.Text = "labelSenderStatusUpdate";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.labelSenderStatusUpdate);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(629, 26);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1027, 100);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // checkBoxLogging
            // 
            this.checkBoxLogging.AutoSize = true;
            this.checkBoxLogging.Location = new System.Drawing.Point(356, 132);
            this.checkBoxLogging.Name = "checkBoxLogging";
            this.checkBoxLogging.Size = new System.Drawing.Size(64, 17);
            this.checkBoxLogging.TabIndex = 18;
            this.checkBoxLogging.Text = "Logging";
            this.checkBoxLogging.UseVisualStyleBackColor = true;
            // 
            // textBoxConnectIP
            // 
            this.textBoxConnectIP.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FirePiercer.Properties.Settings.Default, "ClientIP", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxConnectIP.Location = new System.Drawing.Point(93, 83);
            this.textBoxConnectIP.Name = "textBoxConnectIP";
            this.textBoxConnectIP.Size = new System.Drawing.Size(204, 20);
            this.textBoxConnectIP.TabIndex = 11;
            this.textBoxConnectIP.Text = global::FirePiercer.Properties.Settings.Default.ClientIP;
            // 
            // textBoxConnectPort
            // 
            this.textBoxConnectPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FirePiercer.Properties.Settings.Default, "ClientPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxConnectPort.Location = new System.Drawing.Point(303, 83);
            this.textBoxConnectPort.Name = "textBoxConnectPort";
            this.textBoxConnectPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxConnectPort.TabIndex = 19;
            this.textBoxConnectPort.Text = global::FirePiercer.Properties.Settings.Default.ClientPort;
            this.textBoxConnectPort.TextChanged += new System.EventHandler(this.textBoxConnectPort_TextChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(4, 155);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fastObjectListView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1664, 551);
            this.splitContainer1.SplitterDistance = 925;
            this.splitContainer1.TabIndex = 20;
            // 
            // fastObjectListView1
            // 
            this.fastObjectListView1.AllColumns.Add(this.olvColumnId);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnSent);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnRec);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnState);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnIp);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnAddress);
            this.fastObjectListView1.CellEditUseWholeCell = false;
            this.fastObjectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnId,
            this.olvColumnSent,
            this.olvColumnRec,
            this.olvColumnState,
            this.olvColumnIp,
            this.olvColumnAddress});
            this.fastObjectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.fastObjectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastObjectListView1.HideSelection = false;
            this.fastObjectListView1.Location = new System.Drawing.Point(0, 0);
            this.fastObjectListView1.Name = "fastObjectListView1";
            this.fastObjectListView1.ShowGroups = false;
            this.fastObjectListView1.Size = new System.Drawing.Size(925, 551);
            this.fastObjectListView1.TabIndex = 21;
            this.fastObjectListView1.UseCompatibleStateImageBehavior = false;
            this.fastObjectListView1.View = System.Windows.Forms.View.Details;
            this.fastObjectListView1.VirtualMode = true;
            // 
            // _strumpServer
            // 
            this._strumpServer.Certificate = null;
            this._strumpServer.InitialBufferSize = 2;
            this._strumpServer.Port = 1080;
            this._strumpServer.Stats = stats1;
            this._strumpServer.UseSSL = false;
            // 
            // olvColumnId
            // 
            this.olvColumnId.AspectName = "UniqueId";
            this.olvColumnId.Text = "UniqueId";
            // 
            // olvColumnSent
            // 
            this.olvColumnSent.AspectName = "ParcelsSent";
            this.olvColumnSent.Text = "Parcels Sent";
            // 
            // olvColumnRec
            // 
            this.olvColumnRec.AspectName = "ParcelsReceived";
            this.olvColumnRec.Text = "Parcels Received";
            // 
            // olvColumnState
            // 
            this.olvColumnState.AspectName = "State";
            this.olvColumnState.Text = "State";
            this.olvColumnState.Width = 150;
            // 
            // olvColumnIp
            // 
            this.olvColumnIp.AspectName = "IP";
            this.olvColumnIp.Text = "IP";
            this.olvColumnIp.Width = 90;
            // 
            // olvColumnAddress
            // 
            this.olvColumnAddress.AspectName = "Address";
            this.olvColumnAddress.Text = "Hostname";
            this.olvColumnAddress.Width = 125;
            // 
            // timerFlicker
            // 
            this.timerFlicker.Interval = 500;
            this.timerFlicker.Tick += new System.EventHandler(this.timerFlicker_Tick_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1668, 707);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.textBoxConnectPort);
            this.Controls.Add(this.checkBoxLogging);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.buttonTestP);
            this.Controls.Add(this.textBoxConnectIP);
            this.Controls.Add(this.buttonRemoteDesk);
            this.Controls.Add(this.buttonSendClient);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonStartClient);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button buttonStartClient;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSendClient;
        private sonesson_tools.Strump.StrumpServer _strumpServer;
        private System.Windows.Forms.Timer timerLong;
        private System.Windows.Forms.Button buttonRemoteDesk;
        private System.Windows.Forms.TextBox textBoxConnectIP;
        private System.Windows.Forms.Button buttonTestP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelSenderStatusUpdate;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxLogging;
        private System.Windows.Forms.TextBox textBoxConnectPort;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BrightIdeasSoftware.FastObjectListView fastObjectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnId;
        private BrightIdeasSoftware.OLVColumn olvColumnSent;
        private BrightIdeasSoftware.OLVColumn olvColumnRec;
        private BrightIdeasSoftware.OLVColumn olvColumnState;
        private BrightIdeasSoftware.OLVColumn olvColumnIp;
        private BrightIdeasSoftware.OLVColumn olvColumnAddress;
        private System.Windows.Forms.Timer timerFlicker;
    }
}

