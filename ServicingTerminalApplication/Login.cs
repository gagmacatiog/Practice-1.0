using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{
    public partial class Login : Form
    {
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        public Login()
        {
            InitializeComponent();
            linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
        }

        private void textBox_enter(object sender, EventArgs e)
        {
            TextBox field = ((TextBox)sender);
            if (field.Text == "Username" || field.Text == "Password")
            {
                field.ForeColor = Color.Black;
                field.Text = "";

                if (field.Name.ToString() == "textBox2")
                {
                    field.PasswordChar = '\u25CF';

                }
            }
        }

        private void textBox_leave(object sender, EventArgs e)
        {
            TextBox field = ((TextBox)sender);
            if (field.Text == "")
            {
                field.ForeColor = Color.Silver;
                if (field.Name.ToString() == "textBox1")
                {
                    field.Text = "Username";
                } else
                {
                    field.PasswordChar = '\0';
                    field.Text = "Password";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String Username = Regex.Replace(textBox1.Text, @"\s+", "").ToString();
            String Password = Regex.Replace(textBox2.Text, @"\s+", "").ToString();

            if (Username == "Username" || Password == "Password")
            {
                if (Username == "" || Password == "")
                {
                    spaceErrorInput();
                }
                else
                {
                    MessageBox.Show("Fill the username/password!", "Invalid username/password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            else if (Username == "" && Password == "")
            {
                spaceErrorInput();
            }

            else
            {
                loginProcess();
            }

        }

        private void spaceErrorInput()
        {
            MessageBox.Show("Filling with space is invalid!", "Invalid username/password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            resetFields();
        }

        public void resetFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox_leave(textBox1, new EventArgs());
            textBox_leave(textBox2, new EventArgs());
            button1.Focus();
        }

        public void loginProcess()
        {
            SqlConnection con = new SqlConnection(connection_string);
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE Username = @Username AND Password = @Password", con);
            cmd.Parameters.AddWithValue("@Username", textBox1.Text);
            cmd.Parameters.AddWithValue("@Password", textBox2.Text);

            SqlDataReader reader;

            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                MessageBox.Show("Welcome "+(string)reader["FullName"]);
                this.Hide();
                new addTransactionType().Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
            con.Close();
        }
        

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = false;
                e.Handled = true;
                textBox_leave(((TextBox)sender), new EventArgs());
                button1.Focus();
                button1_Click(this, new EventArgs());
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox2.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ((PictureBox)sender).Focus();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Forgot Password
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Create Account

            this.Hide();
            new createAccount().Show();
        }
    }
}
