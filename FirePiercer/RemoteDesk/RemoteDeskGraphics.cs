using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FirePiercerCommon;
using FirePiercerCommon.RemoteDesk;

namespace FirePiercer.RemoteDesk
{
    public class RemoteDeskGraphics
    {
        public static PierceMessage GetScreenShotParcel()
        {
            var memstream = new MemoryStream();

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }


                bitmap.Save(memstream, ImageFormat.Jpeg);
            }

            var pierceMessage = new PierceMessage(new ImageParcel()
                {JPEG = memstream.ToArray(), Size = bounds.Size, StartPoint = Point.Empty});

            return pierceMessage;
        }
    }
}