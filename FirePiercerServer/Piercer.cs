using FirePiercer;
using FirePiercerCommon;
using FirePiercerCommon.RemoteDesk;
using sonesson_tools;
using sonesson_tools.Strump;
using sonesson_tools.TCP;

namespace FirePiercerServer
{
    public class Piercer
    {
        private readonly PierceServer _tcpServer;
        private StrumpEndpoint _strumpEndpoint;

        public Piercer()
        {
            _tcpServer = new PierceServer {InitialBufferSize = 8, Port = 443, UseSSL = true};
            _strumpEndpoint = new StrumpEndpoint();

            _strumpEndpoint.SockReturn += parcel =>
            {
                Logger.Log("ENDPOINT REC: " + parcel, Severity.Debug);
                var pierceMessage = new PierceMessage(parcel);
                Send(pierceMessage);
                //_strumpServer.SockIncoming(parcel);
            };

            _tcpServer.MessageReceived += _tcpServer_MessageReceived;
            _tcpServer.RemoteDeskRequestReceived += OnRemoteDeskRequest;

            _tcpServer.SockParcelReceived += delegate(object o, SockParcel parcel)
            {
                Logger.Log("ENDPOINT SEND: " + parcel, Severity.Debug);
                _strumpEndpoint.SockOutgoing(parcel);
            };

            _tcpServer.RoundTripReceived += (o, bytes) =>
            {
                Logger.Log("Roundtrip! Length " + bytes.Length, Severity.Debug);
                var pierceMessage = new PierceMessage(PierceHeader.RoundTrip);
                pierceMessage.Payload = bytes;
                Send(pierceMessage);
            };
        }


        private void OnRemoteDeskRequest(object sender, RemoteDeskRequest request)
        {
            // Not linux compatible yet
            //Send(RemoteDeskGraphics.GetScreenShotParcel());
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


        public Stats Stats => _tcpServer.Stats;
    }
}