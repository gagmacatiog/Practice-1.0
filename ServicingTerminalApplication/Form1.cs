using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{
    public partial class Form1 : Form
    {
        bool clickCheck = false;
        Form2 form2 = new Form2();
        Form3 f3 = (Form3)Application.OpenForms["form3"];
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        static int ServicingOffice;
        //public string id { get; }
        //public string s_id { get; }
        //public string full_name { get; }
        //public string transaction_type { get; }
        //public string type { get; }
        private static string id = string.Empty;
        private static string s_id = string.Empty;
        private static string full_name = string.Empty;
        private static string transaction_type = string.Empty;
        private static string type = string.Empty;

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
                form2.Close();
                clickCheck = false;
            }
            else {
                form2 = new Form2();
                form2.Show();
                clickCheck = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int ServicingOffice = 1;
            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rdr;

                cmd.CommandText = "WalkIn_To_Main";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ServicingOffice", ServicingOffice);
                cmd.Connection = con;

                con.Open();

                rdr = cmd.ExecuteReader();
                // Data is accessible through the DataReader object here.
                while (rdr.Read())
                {
                    // get the results of each column
                    // string id = (string)rdr["id"];
                    id = rdr["id"].ToString();
                    type = (string)rdr["Type"];
                    s_id = (string)rdr["Student_No"];
                    full_name = rdr["Full_name"].ToString();
                    transaction_type = (string)rdr["Transaction_Type"];


                }
                con.Close();

            }

            f3.LabelText = "aa";
            f3.Refresh();
            f3.LabelText = "fff";
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}
