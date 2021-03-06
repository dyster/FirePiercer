﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using FirePiercerCommon;
using sonesson_tools;
using sonesson_tools.Strump;
using sonesson_tools.TCP;

namespace FirePiercerClient
{
    public partial class PierceClient : Component
    {
        public readonly Stats Stats = new Stats();

        private TcpClient _client;
        private bool _connected;
        private uint _id;

        private ConcurrentSender _sender;
        public PierceClient()
        {
            InitializeComponent();
        }

        public PierceClient(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Initialize(string ip, int port, X509Certificate2 cert)
        {
            Ip = ip;
            Port = port;
            Cert = cert;


            //_client = new TcpClient(ip, port) { NoDelay = true, Client = { DontFragment = true } };
        }

        public string Ip { get; private set; }
        public int Port { get; private set; }
        public X509Certificate2 Cert { get; private set; }

        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (_connected != value)
                {
                    _connected = value;
                    OnConnectionStatusChanged();
                }
            }
        }

        public event EventHandler ConnectionStatusChanged;

        public event EventHandler<string> SenderStatusUpdate;

        public void Connect()

        {
            //_client = new TcpClient() { NoDelay = true, Client = { DontFragment = true } };
            _client = new TcpClient();
            Logger.Log("Connecting to " + Ip + ":" + Port, Severity.Info);
            _client.BeginConnect(Ip, Port, ConnectCallback, null);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _client.EndConnect(ar);
            }
            catch (SocketException e)
            {
                Logger.Log(e);
                Reconnect();
                return;
            }

            SslStream ssl = new SslStream(_client.GetStream(), false, ValidateServerCertificate, null);

            if (_sender == null)
            {
                _sender = new ConcurrentSender(ssl);
                _sender.StatusUpdate += (sender, s) => OnSenderStatusUpdate(s);
            }
            else
            {
                _sender.SetStream(ssl);
                if (_sender.Paused)
                    _sender.UnPause();
            }

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var x509Certificate2Collection = new X509Certificate2Collection(Cert);
            try
            {
                ssl.AuthenticateAsClient(Ip, x509Certificate2Collection, SslProtocols.Tls12, false);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Reconnect();
                return;
            }

            Logger.Log("Connected to " + Ip + ":" + Port, Severity.Info);


            var pierceMessage = new PierceMessage(PierceHeader.Handshake);

            byte[] parcel = pierceMessage.MakeParcel();

            var tuple = new Tuple<byte[], SslStream>(parcel, ssl);

            try
            {
                ssl.BeginWrite(parcel, 0, parcel.Length, WriteCallBack, tuple);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Reconnect();
                return;
            }
        }

        private void WriteCallBack(IAsyncResult ar)
        {
            Stats.AddPacket(PacketType.Sent);
            var tuple = (Tuple<byte[], SslStream>)ar.AsyncState;

            tuple.Item2.EndWrite(ar);
            Stats.AddBytes(tuple.Item1.Length, ByteType.Sent);


            StateObject state = new StateObject(tuple.Item2);

            // Begin receiving the data from the remote device.  
            tuple.Item2.BeginRead(state.buffer, 0, StateObject.BufferSize, ReadCallBack, state);
        }

        private void Reconnect()
        {
            _sender?.Pause();
            Connected = false;
            Thread.Sleep(2000);

            Connect();
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            Stats.AddPacket(PacketType.Received);
            var state = (StateObject)ar.AsyncState;

            int bytesRead;
            try
            {
                bytesRead = state.ssl.EndRead(ar);
            }
            catch (IOException e)
            {
                // terminated connection

                Logger.Log("Connection terminated by server", Severity.Warning);
                Reconnect();
                return;
            }

            if (bytesRead == 0)
            {
                Logger.Log("0 bytes read, terminating", Severity.Warning);
                Reconnect();
                return;
            }

            Stats.AddBytes(bytesRead, ByteType.Received);
            state.ms.Write(state.buffer, 0, bytesRead);

            while (!PierceMessage.CheckMessageComplete(state.ms.ToArray()))
            {
                var readbuffer = new byte[1024];
                try
                {
                    bytesRead = state.ssl.Read(readbuffer, 0, readbuffer.Length);
                    Stats.AddBytes(bytesRead, ByteType.Received);
                    state.ms.Write(readbuffer, 0, bytesRead);
                }
                catch (IOException e)
                {
                    Logger.Log("Error while receiving PierceMessage: " + e.Message, Severity.Warning);
                    Reconnect();
                    return;
                }
                catch (SocketException e)
                {
                    Logger.Log("Error while receiving PierceMessage: " + e.Message, Severity.Warning);
                    Reconnect();
                    return;
                }
                catch (ObjectDisposedException e)
                {
                    Logger.Log("Error while receiving PierceMessage: " + e.Message, Severity.Warning);
                    Reconnect();
                    return;
                }


            }

            PierceMessage message = PierceMessage.Parse(state.ms.ToArray());

            //Logger.Log("Client rec: " + message, Severity.Info);

            if (string.IsNullOrEmpty(message.ParseError))
            {
                switch (message.Header)
                {
                    case PierceHeader.HandshakeOK:
                        _id = BitConverter.ToUInt32(message.Payload, 0);
                        Connected = true;
                        Logger.Log("Handshake OK, client id " + _id, Severity.Info);
                        break;
                    case PierceHeader.RemoteDeskRequest:
                        // nein
                        break;
                    case PierceHeader.ScreenShot:
                                                    
                        break;
                        

                    case PierceHeader.Invalid:
                        break;
                    case PierceHeader.Handshake:
                        break;
                    case PierceHeader.Message:
                        break;
                    case PierceHeader.Socks5:
                        var sockParcel = SockParcel.DeSerialize(message.Payload);
                        OnSockParcelReceived(sockParcel);
                        break;
                    case PierceHeader.RoundTrip:
                        OnRoundTripReturn(message.Payload);
                        break;
                    case PierceHeader.Object:
                        OnObjectReceived(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            // start listening again with new object
            StateObject newstate = new StateObject(state.ssl);
            state.ssl.BeginRead(newstate.buffer, 0, StateObject.BufferSize, ReadCallBack, newstate);
        }

        private void OnObjectReceived(PierceMessage message)
        {
            var o = message.DeserializePayload();

            throw new Exception("Unknown object received");
        }


        public event EventHandler<SockParcel> SockParcelReceived;

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Send(PierceMessage message)
        {
            message.SenderId = _id;
            var makeParcel = message.MakeParcel();
            _sender.Send(makeParcel);
            Stats.AddPacket(PacketType.Sent);
            Stats.AddBytes(makeParcel.Length, ByteType.Sent);
        }        

        protected virtual void OnSockParcelReceived(SockParcel e)
        {
            SockParcelReceived?.Invoke(this, e);
        }

        public event EventHandler<byte[]> RoundTripReturn;

        protected virtual void OnRoundTripReturn(byte[] e)
        {
            RoundTripReturn?.Invoke(this, e);
        }

        protected virtual void OnConnectionStatusChanged()
        {
            ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSenderStatusUpdate(string e)
        {
            SenderStatusUpdate?.Invoke(this, e);
        }

        public class StateObject
        {
            // Size of receive buffer.  
            public const int BufferSize = 65534;

            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];

            public MemoryStream ms = new MemoryStream();

            // Client socket.  
            public SslStream ssl = null;

            public StateObject(SslStream stream)
            {
                ssl = stream;
            }
        }
    }
}
