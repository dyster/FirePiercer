using System;
using System.Collections.Generic;
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
        private readonly HashSet<uint> _clientIds = new HashSet<uint>();

        public event EventHandler<RemoteDeskRequest> RemoteDeskRequestReceived;

        protected virtual void OnRemoteDeskRequestReceived(RemoteDeskRequest e)
        {
            RemoteDeskRequestReceived?.Invoke(this, e);
        }

        public event EventHandler<SockParcel> SockParcelReceived;

        public event EventHandler<byte[]> RoundTripReceived;

        public override byte[] ParseIncomingSSL(int read, byte[] initialBuffer, SslStream stream, ClientContext context)
        {
            Stats.AddBytes(read, ByteType.Received);
            Stats.AddPacket(PacketType.Received);

            if (read != 8)
            {
                Logger.Log("Initial buffer incorrect size", Severity.Warning);
                return null;
            }

            if (initialBuffer[0] == 0x02 && initialBuffer[1] == 0x07)
            {
                short int16 = BitConverter.ToInt16(initialBuffer, 2);
                if (Enum.TryParse(int16.ToString(), out PierceHeader header))
                {
                    int len = BitConverter.ToInt32(initialBuffer, 4);

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
                            while (_clientIds.Contains(id))
                                id = (uint) new Random().Next();
                            _clientIds.Add(id);

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
                            OnSockParcelReceived(sockParcel);
                            break;
                        case PierceHeader.RoundTrip:
                            OnRoundTripReceived(message.Payload);
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


        protected virtual void OnSockParcelReceived(SockParcel e)
        {
            SockParcelReceived?.Invoke(this, e);
        }

        protected virtual void OnRoundTripReceived(byte[] e)
        {
            RoundTripReceived?.Invoke(this, e);
        }
    }

    public class RemoteClientInfo
    {
        public uint ID { get; set; }
    }
}