using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ServicingTerminalApplication
{
    public partial class Form2 : Form
    {
        //Constants
        const int AW_SLIDE = 0X40000;
        const int AW_VER_POSITIVE = 0X4;
        const int AW_VER_NEGATIVE = 0X8;
        const int AW_BLEND = 0X80000;
        const int AW_HIDE = 0x00010000;
        const int AW_ACTIVATE = 0x00020000;

        [DllImport("user32")]
        static extern bool AnimateWindow(IntPtr hwnd, int time, int flags);
        public Form2()
        {
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      (workingArea.Bottom - Size.Height) - 36);
            this.TopMost = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            //Animate form
            AnimateWindow(this.Handle, 500, AW_SLIDE | AW_VER_NEGATIVE);
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //Animate form
            AnimateWindow(this.Handle, 500, AW_ACTIVATE | AW_SLIDE | AW_VER_POSITIVE | AW_HIDE);

            base.OnClosing(e);
        }
    }
}
