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
    public partial class addTransactionType : Form
    {
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        public addTransactionType()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 9;
            textBox3.ScrollBars = ScrollBars.Vertical;
        }
        public List<_Servicing_Office> getServicingOffices()
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

        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button3.Enabled = true;
            button1.Enabled = false;
            int count = int.Parse(comboBox1.Items[comboBox1.SelectedIndex].ToString());
            
            for (int x = count; x >= 1; x--)
            {

                FlowLayoutPanel p = new FlowLayoutPanel();
                p.Name = "panel" + x;
                p.Size = new Size(329, 40);
                p.BackColor = Color.White;
                p.Padding = new Padding(5);

                Label label = new Label();
                label.Name = "label" + x;
                label.AutoSize = true;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = "Servicing Office " + x + ": ";
                label.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular);
                label.Margin = new Padding(5);

                ComboBox cBox = new ComboBox();
                cBox.Name = "comboBox" + x;
                cBox.Size = new Size(150, 50);


                //List<_Servicing_Office> adataSource = new List<_Servicing_Office>();

                //adataSource.Add(new _Servicing_Office() { Name = "Cashier", Address = "Cashier" });
                //adataSource.Add(new _Servicing_Office() { Name = "Registrar", Address = "Registrar" });
                //adataSource.Add(new _Servicing_Office() { Name = "Student Accounts", Address = "Student Accounts" });
                //adataSource.Add(new _Servicing_Office() { Name = "Book keeper", Address = "Book keeper" });

                cBox.DataSource = getServicingOffices();
                cBox.DisplayMember = "Name";
                cBox.ValueMember = "ID";
                cBox.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);

                cBox.DropDownStyle = ComboBoxStyle.DropDownList;

                p.Controls.Add(label); //p = new FlowLayoutPanel();
                p.Controls.Add(cBox);
                p.Invalidate();

                flowLayoutPanel1.Controls.Add(p);
                flowLayoutPanel1.Controls.SetChildIndex(p, 0);  // this moves the new one to the top!
                                                                // this is just for fun:
                                                                

                flowLayoutPanel1.Invalidate();
            }
            

        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            // Reset Button
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Height = 0;
            addTransactionType.ActiveForm.Height = 260;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Finish Button
            SqlConnection con = new SqlConnection(connection_string);
            SqlTransaction transaction;
            string Transaction_Name = string.Empty;
            string Transaction_Description = string.Empty;
            string Transaction_ShortName = string.Empty;
            int Pattern_Max = flowLayoutPanel1.Controls.Count;
            int new_id = 0;
            int Pattern_Current = 1;
            // Transaction Information
            Transaction_Name = textBox1.Text;
            Transaction_ShortName = textBox2.Text;
            Transaction_Description = textBox3.Text;

            con.Open();
            transaction = con.BeginTransaction("newTransactionType");

            // Write to Transaction_Type
            string _query = "insert into Transaction_Type (Transaction_Name,Description,Pattern_Max,Short_Name) OUTPUT Inserted.ID " +
                "values (@param1,@param2,@param3,@param4) ";
            SqlCommand _cmd = new SqlCommand(_query, con);
            _cmd.Parameters.AddWithValue("@param1", Transaction_Name);
            _cmd.Parameters.AddWithValue("@param2", Transaction_Description);
            _cmd.Parameters.AddWithValue("@param3", Pattern_Max);
            _cmd.Parameters.AddWithValue("@param4", Transaction_ShortName);
            try
            {
                _cmd.Transaction = transaction;

                new_id = (int)_cmd.ExecuteScalar();

                // Write to Transaction_List
                string __query = "insert into Transaction_List (Transaction_ID,Servicing_Office,Pattern_No) " +
                    "values (@param5,@param6,@param7)";
                _cmd.CommandText = __query;
                foreach (FlowLayoutPanel a in flowLayoutPanel1.Controls.OfType<FlowLayoutPanel>())
                    foreach (ComboBox b in a.Controls.OfType<ComboBox>())
                    {
                        _cmd.Parameters.AddWithValue("@param5", new_id);
                        _cmd.Parameters.AddWithValue("@param6", b.SelectedValue);
                        _cmd.Parameters.AddWithValue("@param7", Pattern_Current);
                        Pattern_Current++;
                        _cmd.ExecuteNonQuery();
                        _cmd.Parameters.Clear();

                    }
                transaction.Commit();
                con.Close();
            }
            catch (SqlException ex) {
                transaction.Rollback();
                MessageBox.Show("Message {0}"+ex.Message);
            }

            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Height = 0;
            addTransactionType.ActiveForm.Height = 260;
        }
    }
}
