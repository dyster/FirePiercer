using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirePiercer.RemoteDesk;
using sonesson_tools;
using sonesson_tools.Strump;
using sonesson_tools.TCP;

namespace FirePiercer.Piercer
{
    public partial class Piercer : Component
    {
        public Piercer()
        {
            InitializeComponent();

            _tcpServer.MessageReceived += _tcpServer_MessageReceived;
            _tcpServer.RemoteDeskRequestReceived += OnRemoteDeskRequest;
        }

        public Piercer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            _tcpServer.MessageReceived += _tcpServer_MessageReceived;
            _tcpServer.RemoteDeskRequestReceived += OnRemoteDeskRequest;
            _tcpServer.SockParcelReceived += TcpServerOnSockParcelReceived;
            _tcpServer.RoundTripReceived += TcpServerRoundTripReceived;
        }

        private void TcpServerRoundTripReceived(object sender, byte[] e)
        {
            OnRoundTripReceived(e);
        }

        private void TcpServerOnSockParcelReceived(object sender, SockParcel e)
        {
            OnSockParcelReceived(e);
        }

        public event EventHandler<SockParcel> SockParcelReceived;

        public event EventHandler<byte[]> RoundTripReceived; 
        

        private void OnRemoteDeskRequest(object sender, RemoteDeskRequest request)
        {
            Send(RemoteDeskGraphics.GetScreenShotParcel());
        }

        private void _tcpServer_MessageReceived(object sender, sonesson_tools.TCP.TCPEventArgs e)
        {

            //Logger.Log("message received", Severity.Info);
        }

        public void StartServer()
        {
            _tcpServer.Start();
        }

        /// <summary>
        /// Sends a message to all connected nodes
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendMessage(string message)
        {
            var pierceMessage = new PierceMessage(PierceHeader.Message);
            pierceMessage.Message = message;
            byte[] parcel = pierceMessage.MakeParcel();

            _tcpServer.Send(parcel);
        }

        public void Send(PierceMessage message)
        {
            _tcpServer.Send(message.MakeParcel());
        }

        protected virtual void OnSockParcelReceived(SockParcel e)
        {
            SockParcelReceived?.Invoke(this, e);
        }

        protected virtual void OnRoundTripReceived(byte[] e)
        {
            RoundTripReceived?.Invoke(this, e);
        }

        public Stats Stats => _tcpServer.Stats;
    }
}
