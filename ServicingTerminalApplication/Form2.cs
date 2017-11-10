using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      (workingArea.Bottom - Size.Height)-39);
            this.TopMost = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
