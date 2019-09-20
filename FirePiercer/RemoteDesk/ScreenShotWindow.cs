using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
