using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using sonesson_tools;
using FirePiercer.RemoteDesk;
using FirePiercerCommon;
using FirePiercerCommon.RemoteDesk;


namespace FirePiercer
{
    public partial class Form1 : Form //, PierceService1.IService1Callback
    {
        private PierceClient _pierceClient;

        //private Service1Client _serviceClient;

        //private FragmentPiper _fragmentPiper = new FragmentPiper();

        private uint _sockparcelseed;

        private byte[] _testBytes = null;

        public Form1()
        {
            InitializeComponent();

            Logger.Instance.LogAdded += (sender, log) =>
            {
                if ((checkBoxLogging.Checked && log.Severity == Severity.Debug) || log.Severity != Severity.Debug)
                    Print(log.ToString());
            };


            _strumpServer.SockOutgoing = (parcel) =>
            {
                if (parcel.Parcel != null && (checkBoxLogging.Checked && parcel.Parcel.Length > 5000))
                    Logger.Log("STRUMP REC: " + parcel, Severity.Debug);
                var pierceMessage = new PierceMessage(parcel);
                while(!_pierceClient.Connected)
                    Thread.Sleep(1000);
                _pierceClient.Send(pierceMessage);
            };
            

            


            //var socksListener = new Org.Mentalis.Proxy.Socks.SocksListener(1081);
            //socksListener.Start();
        }


        private delegate void VoidStringDelegate(string str);

        private void Print(string log)
        {
            if (this.InvokeRequired)
                this.Invoke(new VoidStringDelegate(Print), log);
            else
            {
                listBox1.Items.Add(log);
                listBox1.SelectedIndex = -1;
            }
        }
        

        private void buttonStartClient_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(x => { MakeLocalClient(textBoxConnectIP.Text, textBoxConnectPort.Text); });
        }
        
        private void buttonSendClient_Click(object sender, EventArgs e)
        {
            var pierceMessage = new PierceMessage(PierceHeader.Message) {Message = textBox1.Text};

            _pierceClient.Send(pierceMessage);
        }

        private void MakeLocalClient(string ip, string port)
        {
            X509Certificate2 cert = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "pluralsight.pfx", "1234");
            _pierceClient = new PierceClient(ip, int.Parse(port), cert);
            _pierceClient.ImageRecieved += PierceClientOnImageRecieved;

            _pierceClient.SockParcelReceived += (sender, parcel) =>
            {

                if (parcel.Parcel != null && (checkBoxLogging.Checked && parcel.Parcel.Length > 5000))
                    Logger.Log("STRUMP SEND: " + parcel, Severity.Debug);
                _strumpServer.SockIncoming(parcel);
            };

            _pierceClient.RoundTripReturn += (sender, bytes) =>
            {
                Logger.Log("RoundTrip Return, match: " + _testBytes.SequenceEqual(bytes), Severity.Info);
            };

            _pierceClient.ConnectionStatusChanged += (sender, args) =>
            {
                if (_pierceClient.Connected && !_strumpServer.Running)
                {
                    _strumpServer.Start();
                }
            };

            _pierceClient.Connect();
        }

        private void PierceClientOnImageRecieved(object sender, ImageParcel e)
        {
            Image img;
            using (var ms = new MemoryStream(e.JPEG))
            {
                img = Image.FromStream(ms);
            }

            new ScreenShotWindow(img);
        }

        private void timerFlicker_Tick(object sender, EventArgs e)
        {
            //labelStrumpStats.Text = _strumpServer.Stats.ToString() + " frag " + _fragmentPiper.Count;
            
            if (_pierceClient != null) label2.Text = "PierceClient: " + _pierceClient.Stats;
            if (_strumpServer != null) label3.Text = "StrumpServer: " + _strumpServer.Stats;
            if (_strumpEndpoint != null) label4.Text = "StrumpClient: " + _strumpEndpoint.Stats;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timerFlicker.Enabled = true;
        }

        private void ButtonRemoteDesk_Click(object sender, EventArgs e)
        {
            var pierceMessage = new PierceMessage(PierceHeader.RemoteDeskRequest);

            _pierceClient.Send(pierceMessage);
        }

        private void ButtonTestP_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                var bytes = new byte[102400];
                new Random().NextBytes(bytes);
                var pierceMessage = new PierceMessage(PierceHeader.RoundTrip) {Payload = bytes};
                _testBytes = bytes;
                _pierceClient.Send(pierceMessage);
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void ListBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            var s = listBox1.Items[e.Index].ToString();

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

        private void textBoxConnectPort_TextChanged(object sender, EventArgs e)
        {

        }
    }
}