using System;
using System.Drawing;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{
    public partial class Form1 : Form
    {
        bool clickCheck = false;
        private Form2 form = new Form2();

        public Form1()
        {
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
            this.TopMost = true;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
                  
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            if (clickCheck == true)
            {
                form.Close();
                clickCheck = false;
            }
            else {
                form = new Form2();
                form.Show();
                clickCheck = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}
