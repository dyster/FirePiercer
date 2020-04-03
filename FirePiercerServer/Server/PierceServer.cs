using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using FirePiercerCommon;
using FirePiercerCommon.RemoteDesk;
using sonesson_tools;
using sonesson_tools.Strump;
using sonesson_tools.TCP;

namespace FirePiercer
{
    public class PierceServer : TCPServer
    {
        //private readonly HashSet<uint> _clientIds = new HashSet<uint>();

        public event EventHandler<RemoteDeskRequest> RemoteDeskRequestReceived;

        protected virtual void OnRemoteDeskRequestReceived(RemoteDeskRequest e)
        {
            RemoteDeskRequestReceived?.Invoke(this, e);
        }

        public event EventHandler<SockeEventArgs> SockParcelReceived;

        public event EventHandler<RoundTripEventArgs> RoundTripReceived;

        //public BindingList<RemoteClientInfo> RemoteClientInfos { get; set; } = new BindingList<RemoteClientInfo>();

        public ConcurrentDictionary<uint, RemoteClientInfo> RemoteClientList { get; set; } = new ConcurrentDictionary<uint, RemoteClientInfo>();

        public override byte[] ParseIncomingSSL(int read, byte[] initialBuffer, SslStream stream, ClientContext context)
        {
            Stats.AddBytes(read, ByteType.Received);
            Stats.AddPacket(PacketType.Received);

            if (read != this.InitialBufferSize)
            {
                Logger.Log("Initial buffer incorrect size", Severity.Warning);
                return null;
            }

            if (initialBuffer[0] == 0x02 && initialBuffer[1] == 0x07)
            {
                short int16 = BitConverter.ToInt16(initialBuffer, 2);
                if (Enum.TryParse(int16.ToString(), out PierceHeader header))
                {
                    int len = BitConverter.ToInt32(initialBuffer, 8);

                    //Logger.Log(header.ToString(), Severity.Info);
                    //Logger.Log("Length: " + len, Severity.Info);

                    var list = new List<byte>();
                    list.AddRange(initialBuffer);

                    var payload = new byte[len];

                    //int readback = stream.Read(payload, 0, len);

                    int bytesread = 0;
                    do
                    {
                        int readback = stream.Read(payload, bytesread, len - bytesread);
                        Stats.AddBytes(readback, ByteType.Received);
                        bytesread += readback;
                    } while (bytesread < len);


                    list.AddRange(payload);


                    if (bytesread != len)
                    {
                        Logger.Log("Full message not read or too much read!", Severity.Warning);
                        return null;
                    }

                    var endbit = new byte[4];
                    var endread = stream.Read(endbit, 0, 4);
                    Stats.AddBytes(endread, ByteType.Received);
                    if (endread != 4)
                    {
                        Logger.Log("End bit is wrong length!", Severity.Warning);
                        return null;
                    }

                    list.AddRange(endbit);

                    PierceMessage message = PierceMessage.Parse(list.ToArray());

                    //Logger.Log("Message: " + message, Severity.Info);


                    //Logger.Log(BitConverter.ToString(payload), Severity.Info);

                    switch (message.Header)
                    {
                        case PierceHeader.Invalid:
                            Logger.Log("Invalid header received (0x00)", Severity.Warning);
                            return null;
                        case PierceHeader.Handshake:
                            Logger.Log("Handshake received, version " + BitConverter.ToString(payload), Severity.Info);

                            uint id = (uint) new Random().Next();
                            while (RemoteClientList.ContainsKey(id))
                                id = (uint) new Random().Next();

                            if(!RemoteClientList.TryAdd(id, new RemoteClientInfo(id, context)))
                                throw new Exception("Conflicting key on creation, this is impossible");
                            

                            var pierceMessage = new PierceMessage(PierceHeader.HandshakeOK);
                            pierceMessage.Payload = BitConverter.GetBytes(id);
                            byte[] bytes = pierceMessage.MakeParcel();
                            stream.Write(bytes);

                            // return back to server for stat collection
                            return list.ToArray();

                        case PierceHeader.HandshakeOK:
                            break;
                        case PierceHeader.Message:
                            break;
                        case PierceHeader.ScreenShot:

                            break;
                        case PierceHeader.RemoteDeskRequest:
                            OnRemoteDeskRequestReceived(null);
                            break;
                        case PierceHeader.Socks5:
                            var sockParcel = SockParcel.DeSerialize(message.Payload);
                            if (RemoteClientList.ContainsKey(message.SenderId))
                            {
                                OnSockParcelReceived(RemoteClientList[message.SenderId], sockParcel);
                            }
                            else
                            {
                                Logger.Log(message.SenderId + " is an unrecognized Client ID", Severity.Warning);
                            }
                            
                            break;
                        case PierceHeader.RoundTrip:
                            
                            if (RemoteClientList.ContainsKey(message.SenderId))
                            {
                                OnRoundTripReceived(RemoteClientList[message.SenderId], message.Payload);
                            }
                            else
                            {
                                Logger.Log(message.SenderId + " is an unrecognized Client ID", Severity.Warning);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Logger.Log("Invalid header received", Severity.Warning);
                    return null;
                }
            }
            else
            {
                Logger.Log("Invalid magic bytes received", Severity.Warning);
                return null;
            }

            return initialBuffer;
        }


        protected virtual void OnSockParcelReceived(RemoteClientInfo client, SockParcel parcel)
        {
            SockParcelReceived?.Invoke(this, new SockeEventArgs(){Client = client, SockParcel = parcel});
        }

        protected virtual void OnRoundTripReceived(RemoteClientInfo client, byte[] payload)
        {
            RoundTripReceived?.Invoke(this, new RoundTripEventArgs(){Client = client, Payload = payload});
        }
    }

    public class SockeEventArgs : EventArgs
    {
        public RemoteClientInfo Client { get; set; }
        public SockParcel SockParcel { get; set; }
    }

    public class RoundTripEventArgs : EventArgs
    {
        public RemoteClientInfo Client { get; set; }
        public byte[] Payload { get; set; }
    }

    public class RemoteClientInfo
    {
        public RemoteClientInfo(uint id, ClientContext context)
        {
            ID = id;
            Created = DateTime.Now;
            Context = context;
        }

        public uint ID { get; }
        public DateTime Created { get; set; }
        public ClientContext Context { get; set; }

        public override string ToString()
        {
            return ID.ToString();
        }
        
    }
}