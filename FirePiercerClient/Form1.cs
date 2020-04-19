using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using FirePiercerCommon;
using sonesson_tools;
using sonesson_tools.Strump;
using sonesson_tools.TCP;

namespace FirePiercerClient
{
    public partial class Form1 : Form
    {
        private PierceClient _pierceClient;
        private StrumpServer _strumpServer;

        private FastObjectListView _listview;
        private OLVColumn olvColumnId;
        private OLVColumn olvColumnSent;
        private OLVColumn olvColumnRec;
        private OLVColumn olvColumnState;
        private OLVColumn olvColumnIp;
        private OLVColumn olvColumnAddress;
        public Form1()
        {
            InitializeComponent();

            _listview = new FastObjectListView();
            olvColumnId = new OLVColumn();
            olvColumnSent = new OLVColumn();
            olvColumnRec = new OLVColumn();
            olvColumnState = new OLVColumn();
            olvColumnIp = new OLVColumn();
            olvColumnAddress = new OLVColumn();

            _listview.AllColumns.Add(olvColumnId);
            _listview.AllColumns.Add(olvColumnSent);
            _listview.AllColumns.Add(olvColumnRec);
            _listview.AllColumns.Add(olvColumnState);
            _listview.AllColumns.Add(olvColumnIp);
            _listview.AllColumns.Add(olvColumnAddress);
            _listview.CellEditUseWholeCell = false;
            _listview.Columns.AddRange(new ColumnHeader[] {
                olvColumnId,
                olvColumnSent,
                olvColumnRec,
                olvColumnState,
                olvColumnIp,
                olvColumnAddress});
            _listview.Cursor = Cursors.Default;
            _listview.Dock = DockStyle.Fill;
            _listview.HideSelection = false;
            _listview.Location = new Point(0, 0);
            _listview.Name = "fastObjectListView1";
            _listview.ShowGroups = false;
            _listview.Size = new Size(925, 551);
            _listview.TabIndex = 21;
            _listview.UseCompatibleStateImageBehavior = false;
            _listview.View = View.Details;
            _listview.VirtualMode = true;

            // 
            // olvColumnId
            // 
            olvColumnId.AspectName = "UniqueId";
            olvColumnId.Text = "UniqueId";
            // 
            // olvColumnSent
            // 
            olvColumnSent.AspectName = "ParcelsSent";
            olvColumnSent.Text = "Parcels Sent";
            // 
            // olvColumnRec
            // 
            olvColumnRec.AspectName = "ParcelsReceived";
            olvColumnRec.Text = "Parcels Received";
            // 
            // olvColumnState
            // 
            olvColumnState.AspectName = "State";
            olvColumnState.Text = "State";
            olvColumnState.Width = 150;
            // 
            // olvColumnIp
            // 
            olvColumnIp.AspectName = "IP";
            olvColumnIp.Text = "IP";
            olvColumnIp.Width = 90;
            // 
            // olvColumnAddress
            // 
            olvColumnAddress.AspectName = "Address";
            olvColumnAddress.Text = "Hostname";
            olvColumnAddress.Width = 125;

            _listview.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(_listview);

            _pierceClient = new PierceClient();

            _strumpServer = new StrumpServer();
            _strumpServer.Certificate = null;
            _strumpServer.InitialBufferSize = 2;
            _strumpServer.Port = 1080;
            _strumpServer.Stats = new Stats();
            _strumpServer.UseSSL = false;

            _listview.SetObjects(new List<SOCKSRequest>());


            Logger.Instance.LogAdded += (sender, log) =>
            {
                if ((checkBoxLogging.Checked && log.Severity == Severity.Debug) || log.Severity != Severity.Debug)
                    Print(log.ToString());
            };


            _strumpServer.SockOutgoing = parcel =>
            {
                //if (parcel.Parcel != null && (checkBoxLogging.Checked && parcel.Parcel.Length > 5000))
                //    Logger.Log("STRUMP REC: " + parcel, Severity.Debug);
                var pierceMessage = new PierceMessage(parcel);
                while (!_pierceClient.Connected)
                    Thread.Sleep(1000);
                _pierceClient.Send(pierceMessage);
            };

            _strumpServer.ConnectionAdded += (sender, request) => { _listview.AddObject(request); };
            _strumpServer.ConnectionRemoved += (sender, request) => { _listview.RemoveObject(request); };
        }

        private delegate void VoidStringDelegate(string str);

        private void Print(string log)
        {
            if (InvokeRequired)
                Invoke(new VoidStringDelegate(Print), log);
            else
            {
                listBoxLog.Items.Add(log);
                listBoxLog.SelectedIndex = -1;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            Settings1.Default.ClientAddress = textBoxAddress.Text;
            Settings1.Default.ClientPort = Int32.Parse(textBoxPort.Text);
            Settings1.Default.Save();

            

             MakeLocalClient(Settings1.Default.ClientAddress, Settings1.Default.ClientPort);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxAddress.Text = Settings1.Default.ClientAddress;
            textBoxPort.Text = Settings1.Default.ClientPort.ToString();
            timerFlicker.Enabled = true;
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings1.Default.Save();
        }

        private void timerFlicker_Tick(object sender, EventArgs e)
        {
            var filteredObjects = _listview.FilteredObjects;

            foreach (var filteredObject in filteredObjects)
            {
                _listview.RefreshObject(filteredObject);
            }

            if (_pierceClient != null) labelPierceStatus.Text = "PierceClient: " + _pierceClient.Stats;
            if (_strumpServer != null) labelStrumpStatus.Text = "StrumpServer: " + _strumpServer.Stats;
        }

        private void MakeLocalClient(string ip, int port)
        {
            X509Certificate2 cert =
                new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "pluralsight.pfx", "1234");
            _pierceClient.Initialize(ip, port, cert);
            
            _pierceClient.SockParcelReceived += (sender, parcel) =>
            {
                //if (parcel.Parcel != null && (checkBoxLogging.Checked && parcel.Parcel.Length > 5000))
                //    Logger.Log("STRUMP SEND: " + parcel, Severity.Debug);
                _strumpServer.SockIncoming(parcel);
            };            

            _pierceClient.ConnectionStatusChanged += (sender, args) =>
            {
                if (_pierceClient.Connected && !_strumpServer.Running)
                {
                    _strumpServer.Start();
                }
                else if (!_pierceClient.Connected)
                {
                    Logger.Log("Pierce Client disconnected, closing strump server", Severity.Warning);
                    _strumpServer.Stop();
                }
            };

            //_pierceClient.SenderStatusUpdate += (sender, s) => SenderStatusUpdate(s);

            _pierceClient.Connect();
        }

        private void listBoxLog_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            var s = listBoxLog.Items[e.Index].ToString();

            e.DrawBackground();
            Graphics g = e.Graphics;

            // draw the background color you want
            // mine is set to olive, change it to whatever you want

            if (s.Contains("Debug"))
            {
                g.FillRectangle(new SolidBrush(Color.Olive), e.Bounds);
                g.DrawString(s, e.Font, new SolidBrush(Color.Black), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            else if (s.Contains("Warning"))
            {
                g.FillRectangle(new SolidBrush(Color.DarkOrange), e.Bounds);
                g.DrawString(s, e.Font, new SolidBrush(Color.Black), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            else if (s.Contains("Error"))
            {
                g.FillRectangle(new SolidBrush(Color.DarkRed), e.Bounds);
                g.DrawString(s, e.Font, new SolidBrush(Color.White), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                g.DrawString(s, e.Font, new SolidBrush(Color.Black), new PointF(e.Bounds.X, e.Bounds.Y));
            }

            // draw the text of the list item, not doing this will only show
            // the background color
            // you will need to get the text of item to display


            e.DrawFocusRectangle();
        }
    }
}
