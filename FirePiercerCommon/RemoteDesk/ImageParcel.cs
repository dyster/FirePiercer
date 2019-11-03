using System;
using System.Drawing;

namespace FirePiercerCommon.RemoteDesk
{
    [Serializable]
    public class ImageParcel
    {
        public Point StartPoint { get; set; }
        public Size Size { get; set; }

        public byte[] JPEG { get; set; }
    }
}