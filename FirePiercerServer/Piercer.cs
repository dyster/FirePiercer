using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
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
        private Dictionary<uint, StrumpEndpoint> _strumpEndpoints;

        public Piercer()
        {
            _tcpServer = new PierceServer {InitialBufferSize = 12, Port = 443, UseSSL = true};

            X509Certificate2 cert =
                new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "pluralsight.pfx", "1234");
            _tcpServer.Certificate = cert;

            _tcpServer.Clients.ClientAdded += (sender, guid) => { Logger.Log("Client Added: " + guid, Severity.Info); };
            _tcpServer.Clients.ClientRemoved += (sender, guid) =>
            {
                Logger.Log("Client Removed: " + guid, Severity.Info);
            };

            _strumpEndpoints = new Dictionary<uint, StrumpEndpoint>();

            _tcpServer.MessageReceived += _tcpServer_MessageReceived;
            _tcpServer.RemoteDeskRequestReceived += OnRemoteDeskRequest;

            _tcpServer.SockParcelReceived += delegate(object o, SockeEventArgs e)
            {
                if (!_strumpEndpoints.ContainsKey(e.Client.ID))
                {
                    var ep = new StrumpEndpoint();
                    _strumpEndpoints.Add(e.Client.ID, ep);
                    Logger.Log("New StrumpEndPoint for client " + e.Client.ID, Severity.Info);

                    ep.SockReturn += parcel =>
                    {
                        var pierceMessage = new PierceMessage(parcel);
                        Send(pierceMessage, e.Client);

                        //_strumpServer.SockIncoming(parcel);
                    };

                    ep.Points.ListChanged += delegate(object sender, ListChangedEventArgs args) { };
                }

                _strumpEndpoints[e.Client.ID].SockOutgoing(e.SockParcel);
            };

            _tcpServer.RoundTripReceived += (o, e) =>
            {
                Logger.Log("Roundtrip! Length " + e.Payload.Length, Severity.Debug);
                var pierceMessage = new PierceMessage(PierceHeader.RoundTrip);
                pierceMessage.Payload = e.Payload;
                Send(pierceMessage, e.Client);
            };
        }


        public Stats Stats => _tcpServer.Stats;

        public List<string> GetTcpPointStatus()
        {
            var list = new List<string>();

            foreach (var keyValuePair in _strumpEndpoints)
            {
                list.AddRange(keyValuePair.Value.GetPointStatus());
            }

            return list;
        }

        private void OnRemoteDeskRequest(object sender, RemoteDeskRequest request)
        {
            // Not linux compatible yet
            //Send(RemoteDeskGraphics.GetScreenShotParcel());
        }

        private void _tcpServer_MessageReceived(object sender, TCPEventArgs e)
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

        public void Send(PierceMessage message, RemoteClientInfo client)
        {
            _tcpServer.Send(message.MakeParcel(), client.Context);
        }
    }
}