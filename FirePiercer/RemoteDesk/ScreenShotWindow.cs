using System.Drawing;
using System.Windows.Forms;

namespace FirePiercer.RemoteDesk
{
    public partial class ScreenShotWindow : Form
    {
        public ScreenShotWindow(Image img)
        {
            InitializeComponent();

            pictureBox1.Image = img;
            this.Show();
        }
    }
}