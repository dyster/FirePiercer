using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirePiercer.RemoteDesk
{
    [Serializable]
    public class ImageParcel
    {
        public Point StartPoint { get; set; }
        public Size Size { get; set; }

        public byte[] JPEG { get; set; }
    }
}
