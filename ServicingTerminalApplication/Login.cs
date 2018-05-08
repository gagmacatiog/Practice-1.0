using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{
    public partial class Login : Form
    {
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        private bool _user_status = true;
        private int _user_id = 0;
        public int _window = 0;
        public Login()
        {
            InitializeComponent();
            linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            CheckIfThisWindowAllowed();
        }
        private void CheckIfThisWindowAllowed()
        {
            bool allowed = false;
            var macAddr = 
                (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
            SqlConnection con = new SqlConnection(connection_string);
            string query = "select * from Set_Windows where MAC_Address = @param1";
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@param1", macAddr);
            con.Open();
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                _window = (int)rdr["Window"];
                allowed = true;
            }
            con.Close();
            //MessageBox.Show("select * from Set_Windows where MAC_Address = " + macAddr);
            if (!allowed)
            {
                MessageBox.Show("This window is not set up yet. Please contact an administrator.","Unknown Instance");
                Environment.Exit(0);
            }
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
            string Password = "";
            bool IsExist = false;
            SqlConnection con = new SqlConnection(connection_string);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE Username = @Username", con);
                cmd.Parameters.AddWithValue("@Username", textBox1.Text);

                SqlDataReader reader;

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    IsExist = true;
                    Password = (string)reader["Password"];
                    _user_status = (Boolean)reader["Status"];
                    _user_id = (int)reader["id"];
                }
                con.Close();
                if (IsExist)  //if record exis in db , it will return true, otherwise it will return false  
                {
                    if (Cryptography.Decrypt(Password).Equals(textBox2.Text))
                    {
                        MessageBox.Show("Login Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        new Form1( _user_id, _window).Show();
                    }
                    else
                    {
                        MessageBox.Show("Please enter the valid credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox2.Clear();
                    }

                }
                else  //showing the error message if user credential is wrong  
                {
                    MessageBox.Show("Please enter the valid credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox2.Clear();
                }


            }
            catch (SqlException) { MessageBox.Show("Can't connect to local DB");Environment.Exit(0); }
            }
        

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = false;
                textBox_leave(((TextBox)sender), new EventArgs());
                button1.Focus();
                button1_Click(this, new EventArgs());
            }
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
