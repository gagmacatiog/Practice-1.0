using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;

namespace ServicingTerminalApplication
{
    public partial class createAccount : Form
    {
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;

        public createAccount()
        {
            InitializeComponent();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new Login().Show();
            this.Close();
        }

        private void textBox_enter(object sender, EventArgs e)
        {
            TextBox field = ((TextBox)sender);
            if (field.Text == "Username" || field.Text == "Password" || field.Text == "Email Address" || field.Text == "Full Name")

            {
                field.ForeColor = Color.Black;
                field.Text = "";

                if (field.Name.ToString() == "textBox4")
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
                if (field.Name.ToString() == "textBox2")
                {
                    field.Text = "Email Address";
                }
                else if (field.Name.ToString() == "textBox1")
                {
                    field.Text = "Full Name";
                }
                else if (field.Name.ToString() == "textBox3")
                {
                    field.Text = "Username";
                }
                else
                {
                    field.PasswordChar = '\0';
                    field.Text = "Password";
                }
            }
        }

        private void createAccount_Load(object sender, EventArgs e)
        {
            button1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(checkFields().Equals(true)) {

                registrationProcess();

            }else
            {
                MessageBox.Show("A fields is left empty!", "Fill all fields", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Login().Show();
        }


        private Boolean checkFields()
        {
            Boolean fieldsNotEmpty = true;

            String fullName = Regex.Replace(textBox1.Text, @"\s+", "").ToString();
            String email = Regex.Replace(textBox2.Text, @"\s+", "").ToString();
            String Username = Regex.Replace(textBox3.Text, @"\s+", "").ToString();
            String Password = Regex.Replace(textBox4.Text, @"\s+", "").ToString();

            if (fullName == "Full Name" || email == "Email Address" || Username == "Username" || Password == "Password")
            {
                fieldsNotEmpty = false;
            }
            else if (fullName == "" || email == "" || Username == "" || Password == "")
            {
                fieldsNotEmpty = false;
            }

            return fieldsNotEmpty;
        }

        private void registrationProcess()
        {
            SqlConnection con = new SqlConnection(connection_string);
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into users (FullName, EmailAddress, Username, Password, Status) values (@FullName,@EmailAddress,@Username, @Password, @Status)", con);
            cmd.Parameters.AddWithValue("@FullName", textBox1.Text);
            cmd.Parameters.AddWithValue("@EmailAddress", textBox2.Text);
            cmd.Parameters.AddWithValue("@Username", textBox3.Text);
            cmd.Parameters.AddWithValue("@Password", textBox4.Text);
            cmd.Parameters.AddWithValue("@Status", 1);
            cmd.ExecuteNonQuery();
            con.Close();

            resetFields();

            this.Hide();
            new Login().Show();
        }

        public void resetFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            button1.Focus();
        }
    }
}
