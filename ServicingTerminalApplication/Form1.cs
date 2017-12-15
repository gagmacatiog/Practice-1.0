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
        static int Servicing_Office = 1;
        static int modeCounter = 0;
        private static string id = string.Empty;
        private static string s_id = string.Empty;
        private static string full_name = string.Empty;
        private static string transaction_type = string.Empty;
        private static string type = string.Empty;
        Form3 fnf = new Form3();
        public Form1()
        {
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
            this.TopMost = true;

            fnf.FirstNameUpdated += Fnf_FirstNameUpdated;
            fnf.Show();


        }
        private void Next() {
            //Updates the information of the next user to be served

            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                SqlCommand cmd2 = con.CreateCommand();
                SqlCommand cmd3 = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd2.CommandType = CommandType.Text;

                String query = "select * from Queue_Info where Servicing_Office = @Servicing_Office";
                String query2 = "select * from Main_Queue where Servicing_Office = @Servicing_Office";

                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Servicing_Office", Servicing_Office);
                cmd2 = new SqlCommand(query2, con);
                cmd2.Parameters.AddWithValue("@Servicing_Office", Servicing_Office);

                //Transferring previous customer to logs



                //Selecting Customer



                SqlDataReader rdr;
                SqlDataReader rdr2;
                rdr = cmd.ExecuteReader();
                int rowCount = 0;
                while (rdr.Read())
                { rowCount++; { if (rowCount > 0) { break; } } }
                if (rowCount > 0)
                {
                    string Current_Number = rdr["Current_Number"].ToString();
                    string Mode = rdr["Mode"].ToString();
                    string Status = rdr["Status"].ToString();
                    modeCounter = (int)rdr["Counter"];
                }
                else
                {
                    MessageBox.Show("No queueing data found. Please start an instance of a serving kiosk first.");
                }

            }
            con.Close();



        }
        private void Get_Information_Queue() {
            //Updates the information of the Queue_Info respective to this Servicing_Office

        }
        private void Fnf_FirstNameUpdated(object sender, updateForm e)
        {
            if(e != null && e.id != null)
            {   fnf.Label2 = e.id;
                fnf.Label4 = e.s_id;
                fnf.Label6 = e.full_name;
                fnf.Label8 = e.transaction_type;
                fnf.Label10 = e.type;

            }
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
            //int ServicingOffice = 1;
            //SqlConnection con = new SqlConnection(connection_string);
            //using (con)
            //{
            //    SqlCommand cmd = new SqlCommand();
            //    SqlDataReader rdr;

            //    cmd.CommandText = "WalkIn_To_Main";
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("ServicingOffice", ServicingOffice);
            //    cmd.Connection = con;

            //    con.Open();

            //    rdr = cmd.ExecuteReader();
            //    // Data is accessible through the DataReader object here.
            //    while (rdr.Read())
            //    {
            //        // get the results of each column
            //        // string id = (string)rdr["id"];
            //        id = rdr["id"].ToString();
            //        type = (string)rdr["Type"];
            //        s_id = (string)rdr["Student_No"];
            //        full_name = rdr["Full_name"].ToString();
            //        transaction_type = (string)rdr["Transaction_Type"];


            //    }
            //    con.Close();

            //}
            
            //updateForm nuea = new updateForm();
            //nuea.id = id;
            //nuea.type = type;
            //nuea.s_id = s_id;
            //nuea.full_name = full_name;
            //nuea.transaction_type = transaction_type;
            //fnf.OnFirstNameUpdated(nuea);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}
