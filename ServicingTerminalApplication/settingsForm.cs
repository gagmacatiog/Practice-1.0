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
    public partial class settingsForm : Form
    {
        public settingsForm()
        {
            InitializeComponent();
        }

        private void settingsForm_Load(object sender, EventArgs e)
        {

        }

        private void onMouseClick(object sender, EventArgs e)
        {
            if (((PictureBox)sender) == pictureBox1)
            {
                this.Hide();
                new Login().Show();
            }
            else
            {

            }
        }

        private void onHover(object sender, EventArgs e)
        {
            if (((PictureBox)sender) == pictureBox1)
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.outBtn_pressed;
            }
            else if(((PictureBox)sender) == pictureBox2)
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.helpBtn_pressed;
            }else
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.editBtn_pressed;
            }
        }

        private void onMouseLeave(object sender, EventArgs e)
        {
            if (((PictureBox)sender) == pictureBox1)
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.outBtn;
            }
            else if(((PictureBox)sender) == pictureBox2)
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.helpBtn;
            }else
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.editBtn;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox3.Enabled = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            textBox4.Enabled = true;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            textBox5.Enabled = true;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            textBox6.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Save changes

        }
    }
}
