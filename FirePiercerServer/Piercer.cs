﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            _tcpServer.Clients.ListChanged += ClientsOnListChanged;
            _strumpEndpoints = new Dictionary<uint, StrumpEndpoint>();
            
            _tcpServer.MessageReceived += _tcpServer_MessageReceived;
            _tcpServer.RemoteDeskRequestReceived += OnRemoteDeskRequest;

            _tcpServer.SockParcelReceived += delegate(object o, SockeEventArgs e)
            {
                Logger.Log("ENDPOINT SEND: " + e.SockParcel, Severity.Debug);

                if (!_strumpEndpoints.ContainsKey(e.Client.ID))
                {
                    var ep = new StrumpEndpoint();
                    _strumpEndpoints.Add(e.Client.ID, ep);
                    
                    ep.SockReturn += parcel =>
                    {
                        Logger.Log("ENDPOINT REC: " + parcel, Severity.Debug);
                        var pierceMessage = new PierceMessage(parcel);
                        Send(pierceMessage, e.Client);
                        //_strumpServer.SockIncoming(parcel);
                    };
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

        private void ClientsOnListChanged(object sender, ListChangedEventArgs e)
        {
            string log = "Client Status: ";
            switch (e.ListChangedType)
            {
                case ListChangedType.Reset:
                    log += "List has been reset";
                    break;
                case ListChangedType.ItemAdded:
                    log += "New " + _tcpServer.Clients[e.NewIndex];
                    break;
                case ListChangedType.ItemDeleted:
                    log += "Deleted " + _tcpServer.Clients[e.NewIndex];
                    break;
                case ListChangedType.ItemMoved:
                    // not interesting
                    break;
                case ListChangedType.ItemChanged:
                    log += "Changed? " + _tcpServer.Clients[e.NewIndex];
                    break;
                case ListChangedType.PropertyDescriptorAdded:
                    log += "PropertyDescriptorAdded? " + _tcpServer.Clients[e.NewIndex];
                    break;
                case ListChangedType.PropertyDescriptorDeleted:
                    log += "PropertyDescriptorDeleted " + _tcpServer.Clients[e.NewIndex];
                    break;
                case ListChangedType.PropertyDescriptorChanged:
                    log += e.PropertyDescriptor.Name + " changed to " + e.PropertyDescriptor.GetValue(_tcpServer.Clients[e.NewIndex]) + " on " + _tcpServer.Clients[e.NewIndex];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Logger.Log(log, Severity.Info);
            
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

        public void Send(PierceMessage message, RemoteClientInfo client)
        {
            _tcpServer.Send(message.MakeParcel(), client.Context);
        }


        public Stats Stats => _tcpServer.Stats;
    }
}