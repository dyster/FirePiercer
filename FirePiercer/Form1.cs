using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using sonesson_tools;
using System.ServiceModel;
using FirePiercer.PierceService1;
using FirePiercer.RemoteDesk;
using FirePiercer.ServiceModel;
using sonesson_tools.Generic;
using sonesson_tools.Strump;


namespace FirePiercer
{
    public partial class Form1 : Form //, PierceService1.IService1Callback
    {
        private PierceClient _localpclient;

        //private Service1Client _serviceClient;

        //private FragmentPiper _fragmentPiper = new FragmentPiper();

        private uint _sockparcelseed;

        private byte[] _testBytes = null;

        public Form1()
        {
            InitializeComponent();

            Logger.Instance.LogAdded += (sender, log) =>
            {
                if((checkBoxLogging.Checked && log.Severity == Severity.Debug) || log.Severity != Severity.Debug)
                    Print(log.ToString());
            };

            //using (ServiceHost host = new ServiceHost(
            //    typeof(StringReverser), new Uri("net.tcp://localhost:8000")))
            //{
            //    host.AddServiceEndpoint(typeof(IStringReverser),
            //        new NetTcpBinding(),
            //        "Reverse");
            //
            //
            //    host.Open();
            //
            //    ChannelFactory<IStringReverser> httpFactory =
            //        new ChannelFactory<IStringReverser>(
            //            new NetTcpBinding(),
            //            new EndpointAddress(
            //                "net.tcp://localhost:8000"));
            //
            //
            //
            //    IStringReverser tcpProxy =
            //        httpFactory.CreateChannel();
            //
            //    var reverseString = tcpProxy.ReverseString("1234");
            //    //host.Close();
            //}


            _strumpServer.SockOutgoing = (parcel) =>
            {
                if(checkBoxLogging.Checked)
                    Logger.Log("STRUMP REC: " + parcel, Severity.Debug);
                var pierceMessage = new PierceMessage(parcel);
                _localpclient.Send(pierceMessage);
            };

            _strumpEndpoint.SockReturn += parcel =>
            {
                if (checkBoxLogging.Checked)
                    Logger.Log("ENDPOINT REC: " + parcel, Severity.Debug);
                var pierceMessage = new PierceMessage(parcel);
                _piercer.Send(pierceMessage);
                //_strumpServer.SockIncoming(parcel);
            };


            _piercer.SockParcelReceived += (sender, parcel) =>
            {
                if (checkBoxLogging.Checked)
                    Logger.Log("ENDPOINT SEND: " + parcel, Severity.Debug);
                _strumpEndpoint.SockOutgoing(parcel);
            };

            _piercer.RoundTripReceived += (sender, bytes) =>
            {
                Logger.Log("Roundtrip! Length " + bytes.Length, Severity.Debug);
                var pierceMessage = new PierceMessage(PierceHeader.RoundTrip);
                pierceMessage.Payload = bytes;
                _piercer.Send(pierceMessage);
            };


            _strumpServer.Start();
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


        private void button1_Click(object sender, EventArgs e)
        {
            _piercer.StartServer();


            ThreadPool.QueueUserWorkItem(x =>
            {
                Thread.Sleep(1000);

                MakeLocalClient("127.0.0.1");
            });
        }

        private void buttonStartServer_Click(object sender, EventArgs e)
        {
            _piercer.StartServer();
        }

        private void buttonStartClient_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(x => { MakeLocalClient(textBoxConnectIP.Text); });
        }

        private void buttonSendServer_Click(object sender, EventArgs e)
        {
            _piercer.SendMessage(textBox1.Text);
        }

        private void buttonSendClient_Click(object sender, EventArgs e)
        {
            var pierceMessage = new PierceMessage(PierceHeader.Message) {Message = textBox1.Text};
            
            _localpclient.Send(pierceMessage);
        }

        private void MakeLocalClient(string ip)
        {
            X509Certificate2 cert = new X509Certificate2("pluralsight.pfx", "1234");
            _localpclient = new PierceClient(ip, 443, cert);
            _localpclient.ImageRecieved += LocalpclientOnImageRecieved;
            _localpclient.SockParcelReceived += (sender, parcel) =>
            {
                if (checkBoxLogging.Checked)
                    Logger.Log("STRUMP SEND: " + parcel, Severity.Debug);
                _strumpServer.SockIncoming(parcel);
            };
            _localpclient.RoundTripReturn += (sender, bytes) =>
            {
                Logger.Log("RoundTrip Return, match: " + _testBytes.SequenceEqual(bytes), Severity.Info);
            };
        }

        private void LocalpclientOnImageRecieved(object sender, ImageParcel e)
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
            if (_piercer != null) label1.Text = "PierceServer: " + _piercer.Stats;
            if (_localpclient != null) label2.Text = "PierceClient: " + _localpclient.Stats;
            if (_strumpServer != null) label3.Text = "StrumpServer: " + _strumpServer.Stats;
            if (_strumpEndpoint != null) label4.Text = "StrumpClient: " + _strumpEndpoint.Stats;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timerFlicker.Enabled = true;
        }

        /*
        private void buttonWCFConnect_Click(object sender, EventArgs e)
        {
            var context = new InstanceContext(null, this);
            _serviceClient = new PierceService1.Service1Client(context);


            //create a unique callback address so multiple clients can run on one machine
            WSDualHttpBinding binding = (WSDualHttpBinding)_serviceClient.Endpoint.Binding;
            binding.MaxReceivedMessageSize = 67108864;
            //string clientcallbackaddress = binding.ClientBaseAddress.AbsoluteUri;
            //clientcallbackaddress += Guid.NewGuid().ToString();
            //binding.ClientBaseAddress = new Uri(clientcallbackaddress);

            _serviceClient.Endpoint.Address = new EndpointAddress($"http://{textBoxServerIP.Text}:{textBoxServerPort.Text}/Design_Time_Addresses/PierceService/Service1/");


            var clientInfo = new ClientInfo() {HostName = System.Environment.MachineName};
            //_serviceClient.Endpoint.Address = 
            
            clientInfo = _serviceClient.Subscribe(clientInfo);

            Logger.Log("Succesfully subscribed: " + clientInfo.Subscribed, Severity.Info);

            //var returnstring = client.GetData("12334");
            

            //Logger.Log(returnstring, Severity.Info);
        }
        
             private void buttonTest_Click(object sender, EventArgs e)
        {
            var parcel = new SockParcel
            {
                UniqueId = 5,
                Parcel = new byte[40000]
            };
            var ran = new Random();
            ran.NextBytes(parcel.Parcel);

            var fragments = FragmentPiper.Fragment(parcel, 4000, parcel.UniqueId);

            foreach (var fragment in fragments)
            {
                //_serviceClient.SockSendAsync(fragment);
            }
        }

            public void Message(string msg)
        {
            Logger.Log(msg, Severity.Info);
        }

        public void SockReturn(Fragment fragment)
        {
            var reconstruct = _fragmentPiper.Reconstruct(fragment);
            if (reconstruct != null)
            {
                var parcel = (SockParcel) reconstruct;
                _strumpServer.SockIncoming(parcel);
                //if(parcel.Parcel.Length > 4000)
                //    Logger.Log("STRUMP: " + Functions.PrettyPrintSize(parcel.Parcel.Length) + " received from pipe", Severity.Info);
            }
            
            
            
        }
             
             */


        private void ButtonRemoteDesk_Click(object sender, EventArgs e)
        {
            var pierceMessage = new PierceMessage(PierceHeader.RemoteDeskRequest);
            
            _localpclient.Send(pierceMessage);
        }

        private void ButtonTestP_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                var bytes = new byte[102400];
                new Random().NextBytes(bytes);
                var pierceMessage = new PierceMessage(PierceHeader.RoundTrip) {Payload = bytes};
                _testBytes = bytes;
                _localpclient.Send(pierceMessage);
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void ListBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
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
    }
}