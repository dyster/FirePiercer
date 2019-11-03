using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FirePiercerCommon.RemoteDesk;
using sonesson_tools.Strump;

namespace FirePiercerCommon
{
    public class PierceMessage
    {
        /// <summary>
        /// Empty constructor is private to only allow Parse method to create it
        /// </summary>
        private PierceMessage()
        {
        }

        public PierceMessage(PierceHeader header)
        {
            Header = header;
        }

        public PierceMessage(ImageParcel image)
        {
            Header = PierceHeader.ScreenShot;
            SerializePayload(image);
        }

        public PierceMessage(RemoteDeskRequest request)
        {
            Header = PierceHeader.RemoteDeskRequest;
            SerializePayload(request);
        }

        public PierceMessage(SockParcel sockParcel)
        {
            Header = PierceHeader.Socks5;
            Payload = sockParcel.Serialize();
        }

        public byte[] MakeParcel()
        {
            byte[] payload = null;

            switch (Header)
            {
                case PierceHeader.Invalid:
                    break;
                case PierceHeader.Handshake:
                    payload = new byte[] {0, 1, 0, 0};
                    break;
                case PierceHeader.HandshakeOK:
                    payload = Payload;
                    break;
                case PierceHeader.Message:
                    payload = Encoding.Unicode.GetBytes(this.Message);
                    break;
                case PierceHeader.ScreenShot:
                case PierceHeader.RemoteDeskRequest:
                case PierceHeader.Socks5:
                case PierceHeader.RoundTrip:
                    payload = Payload;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            var list = new List<byte> {0x02, 0x07};
            list.AddRange(BitConverter.GetBytes((short) this.Header));
            if (payload != null)
            {
                int len = payload.Length;
                list.AddRange(BitConverter.GetBytes(len));
                list.AddRange(payload);
                list.AddRange(BitConverter.GetBytes(len));
            }
            else
            {
                list.AddRange(BitConverter.GetBytes(0));
                list.AddRange(BitConverter.GetBytes(0));
            }

            var parcel = list.ToArray();
            return parcel;
        }

        private void SerializePayload(object o)
        {
            var mem = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(mem, o);
            Payload = mem.ToArray();
        }

        public ImageParcel GetImageParcel()
        {
            var mem = new MemoryStream();
            mem.Write(Payload, 0, Payload.Length);
            var binaryFormatter = new BinaryFormatter();
            return (ImageParcel) binaryFormatter.Deserialize(mem);
        }

        /// <summary>
        /// Returns the length of the PierceMessage, or -1 if not valid
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int GetHeaderLength(byte[] bytes)
        {
            if (bytes[0] == 0x02 && bytes[1] == 0x07)
            {
                short int16 = BitConverter.ToInt16(bytes, 2);
                if (Enum.TryParse(int16.ToString(), out PierceHeader header))
                {
                    int len = BitConverter.ToInt32(bytes, 4);


                    return len;
                }

                return -1;
            }

            return -1;
        }

        public static bool CheckMessageComplete(byte[] bytes)
        {
            var headerLength = GetHeaderLength(bytes);
            return bytes.Length == 8 + headerLength + 4;
        }

        public static PierceMessage Parse(byte[] bytes)
        {
            var message = new PierceMessage();

            if (bytes[0] == 0x02 && bytes[1] == 0x07)
            {
                short int16 = BitConverter.ToInt16(bytes, 2);
                if (Enum.TryParse(int16.ToString(), out PierceHeader header))
                {
                    message.Header = header;
                    int len = BitConverter.ToInt32(bytes, 4);


                    message.Payload = new byte[len];


                    if (bytes.Length != 8 + len + 4)
                    {
                        message.ParseError = "Data of incorrect length";
                        return message;
                    }

                    int endlen = BitConverter.ToInt32(bytes, 8 + len);

                    if (len != endlen)
                    {
                        message.ParseError = "Start and End length do not match!";
                        return message;
                    }

                    Array.Copy(bytes, 8, message.Payload, 0, len);

                    switch (header)
                    {
                        case PierceHeader.Invalid:
                            message.ParseError = "Invalid header (0x00)";
                            return message;
                        case PierceHeader.Message:
                            message.Message = Encoding.Unicode.GetString(message.Payload);
                            return message;
                        default:
                            return message;
                    }
                }

                message.ParseError = "Unparsable header";
                return message;
            }

            message.ParseError = "Invalid Magic bytes";
            return message;
        }

        public string ParseError { get; set; }


        public PierceHeader Header { get; set; }

        public DateTime Created { get; set; }

        public byte[] Payload { get; set; }

        /// <summary>
        /// The unique ID of the sender
        /// </summary>
        public uint SenderId { get; set; }

        public string Message { get; set; }


        public override string ToString()
        {
            if (string.IsNullOrEmpty(ParseError))
            {
                string ret = Header.ToString();
                if (Header == PierceHeader.Message)
                    ret += ": " + Message;
                else
                {
                    ret += ": Length " + Payload.Length;
                }
            }

            return "ERROR: " + ParseError;
        }
    }
}