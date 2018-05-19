using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{
    public partial class settingsForm : Form
    {
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        Form1 mainForm = (Form1)Application.OpenForms["Form1"];
        currentCustomer customerForm = (currentCustomer)Application.OpenForms["currentCustomer"];
        addServicingOffice form_so = (addServicingOffice)Application.OpenForms["addServicingOffice"];
        addTransactionType form_tt = (addTransactionType)Application.OpenForms["addTransactionType"];

        public settingsForm()
        {
            InitializeComponent();
            TopMost = true;
        }
        private List<_Servicing_Office> LIST_getServicingOffices()
        {

            List<_Servicing_Office> dataSource = new List<_Servicing_Office>();
            // List possible Servicing Offices
            SqlConnection con = new SqlConnection(connection_string);
            string retrieve_servicing_offices = "select * from Servicing_Office";
            SqlDataReader _rdr;
            SqlCommand __cmd = new SqlCommand(retrieve_servicing_offices, con);

            try
            {
                con.Open();
                _rdr = __cmd.ExecuteReader();
                while (_rdr.Read())
                {
                    dataSource.Add(new _Servicing_Office()
                    {
                        Name = (string)_rdr["Name"],
                        Address = (string)_rdr["Address"],
                        id = (int)_rdr["ID"]
                    });
                }
                con.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Can't connect to local DB!");
                Environment.Exit(0);
            }
            return dataSource;
        }
        private List<_Transaction_Type> LIST_getTransactionType()
        {

            List<_Transaction_Type> dataSource = new List<_Transaction_Type>();
            // List possible Servicing Offices
            SqlConnection con = new SqlConnection(connection_string);
            string retrieve_servicing_offices = "select * from Transaction_Type";
            SqlDataReader _rdr;
            SqlCommand __cmd = new SqlCommand(retrieve_servicing_offices, con);

            try
            {
                con.Open();
                _rdr = __cmd.ExecuteReader();
                while (_rdr.Read())
                {
                    dataSource.Add(new _Transaction_Type()
                    {
                        Transaction_Name = (string)_rdr["Transaction_Name"],
                        id = (int)_rdr["ID"]
                    });
                }
                con.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Can't connect to local DB!");
            }
            return dataSource;
        }
        private List<_Users> LIST_getUsers()
        {
            List<_Users> dataSource = new List<_Users>();
            // List possible Servicing Offices
            SqlConnection con = new SqlConnection(connection_string);
            string retrieve_servicing_offices = "select * from Users";
            SqlDataReader _rdr;
            SqlCommand __cmd = new SqlCommand(retrieve_servicing_offices, con);
            
            if (mainForm != null)
                if(mainForm.user_type == 2)
            try
            {
                con.Open();
                _rdr = __cmd.ExecuteReader();
                while (_rdr.Read())
                {
                    dataSource.Add(new _Users()
                    {
                        FullName = (string)_rdr["FullName"],
                        id = (int)_rdr["ID"]
                    });
                }
                con.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Can't connect to local DB!");
            }

            return dataSource;
        }
        

        private void settingsForm_Load(object sender, EventArgs e)
        {

        }

       

        private void onHover(object sender, EventArgs e)
        {
            //if (((PictureBox)sender) == pictureBox1)
            //{
            //    ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.outBtn_pressed;
            //}
            //else
            //{
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.editBtn_pressed;
            //}
        }

        private void onMouseLeave(object sender, EventArgs e)
        {
            //if (((PictureBox)sender) == pictureBox1)
            //{
            //    ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.outBtn;
            //}
            //else
            //{
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.editBtn;
            //}
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (textBox3.Enabled == false)
                textBox3.Enabled = true;
            else
                textBox3.Enabled = false;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (textBox4.Enabled == false)
                textBox4.Enabled = true;
            else
                textBox4.Enabled = false;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (textBox5.Enabled == false)
                textBox5.Enabled = true;
            else
                textBox5.Enabled = false;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (textBox6.Enabled == false)
                textBox6.Enabled = true;
            else
                textBox6.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Save changes
            // Check textBoxes length if OK
            if (OkFieldLength())
            {
                
                if (mainForm != null)
                {
                    SqlConnection con = new SqlConnection(connection_string);

                    // Check if password is correct

                    SqlCommand _check_cmd = new SqlCommand("SELECT Password FROM users WHERE id = @param1", con);
                    _check_cmd.Parameters.AddWithValue("@param1", mainForm.user_id);
                    SqlDataReader reader;
                    string Password = "";

                    try
                    {
                        con.Open();
                        reader = _check_cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            Password = (string)reader["Password"];
                        }
                        if (Cryptography.Decrypt(Password).Equals(textBox6.Text))
                        {
                            string query = "update Users set ";
                            SqlCommand _cmd = new SqlCommand();

                            _cmd.Connection = con;

                            if (!(string.IsNullOrEmpty(textBox3.Text)))
                            { query += " FullName = @param1"; _cmd.Parameters.AddWithValue("@param1", textBox3.Text); }
                            if (!(string.IsNullOrEmpty(textBox4.Text)))
                            { query += " Password = @param2"; _cmd.Parameters.AddWithValue("@param2", Cryptography.Encrypt(textBox4.Text.ToString())); }
                            if (!(string.IsNullOrEmpty(textBox5.Text)))
                            { query += " Email = @param3"; _cmd.Parameters.AddWithValue("@param3", textBox5.Text); }

                            query += " where id = @param4";
                            _cmd.Parameters.AddWithValue("@param4", mainForm.user_id);
                            _cmd.CommandText = query;
                            _cmd.ExecuteNonQuery();

                            textBox3.Clear();
                            textBox4.Clear();
                            textBox5.Clear();
                            textBox6.Clear();

                            MessageBox.Show("Change Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                        }
                        else
                        {
                            MessageBox.Show("Please enter the valid credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("DB error! {0}", ex.Message);
                    }
                }

            }

        }
        private bool OkFieldLength()
        {
            if (textBox3.Enabled == true)
                if (textBox3.TextLength < 10 || textBox3.TextLength > 100)
                { MessageBox.Show("Full Name should be 10 - 100 characters."); return false; }
            if (textBox4.Enabled == true)
                if (textBox4.TextLength < 7 && textBox4.TextLength > 50)
                { MessageBox.Show("Password should be 8 - 50 characters."); return false; }
            if (textBox5.Enabled == true)
                if (textBox5.TextLength < 10 && textBox5.TextLength > 100)
                { MessageBox.Show("Email must be 10 - 100 characters."); return false; }
            if (textBox3.Enabled == false && textBox4.Enabled == false && textBox5.Enabled == false)
            { MessageBox.Show("Please select the item/s that you want to change."); return false; }
            if (textBox6.Enabled == false)
            { MessageBox.Show("Write your old password first to confirm changes!","Old Password disabled"); return false; }
            if (textBox6.Enabled == true)
                if (textBox6.TextLength < 7 && textBox6.TextLength > 50)
                { MessageBox.Show("Old password should be 8 - 50 characters."); return false; }
            return true;
        }
        
        
    }
}
