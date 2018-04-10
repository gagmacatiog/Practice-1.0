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
        Form1 mainForm = (Form1)Application.OpenForms["Form1"];
        currentCustomer customerForm = (currentCustomer)Application.OpenForms["currentCustomer"];
        addServicingOffice form_so = (addServicingOffice)Application.OpenForms["addServicingOffice"];
        addTransactionType form_tt = (addTransactionType)Application.OpenForms["addTransactionType"];
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
                var confirmResult = MessageBox.Show("Are you sure to log out?",
                                     "Confirm Logout",
                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    // If 'Yes', do something here.
                    new Login().Show();
                    this.Hide();
                    if (mainForm != null)
                        mainForm.Close();
                    if (customerForm != null)
                        customerForm.Close();
                }
                else
                {
                    // If 'No', do something here.
                }
                
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

        private void button2_Click(object sender, EventArgs e)
        {
            form_so.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            form_tt.Show();
        }
    }
}
