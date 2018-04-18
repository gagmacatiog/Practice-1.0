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
    public partial class OfficeConfirmation : Form
    {
        private int user_type, user_id;

        public OfficeConfirmation(int _a_user_type, int _a_user_id)
        {
            InitializeComponent();
            this.user_type = _a_user_type;
            this.user_id = _a_user_id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Fill the text field!", "Invalid value.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }else
            {
                this.Hide();
                new Form1(user_type, user_id, textBox1.Text).Show();
            }
        }
    }
}
