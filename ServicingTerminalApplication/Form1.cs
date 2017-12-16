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
        //private static int
        DataTable table_Modes;
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
            Console.Write("\n Initializing form... Calling getModes() to set table_Modes \n ");
            table_Modes = getModes();

        }

        private DataTable getModes(){
            DataTable modeList = new DataTable();
            modeList.Columns.Add("Mode_Number",typeof(int));
            modeList.Columns.Add("Mode_Name",typeof(string));
            modeList.Columns.Add("Min",typeof(int));
            modeList.Columns.Add("Max",typeof(int));
            modeList.Columns.Add("Count_WalkIn",typeof(int));


            SqlConnection con = new SqlConnection(connection_string);
            using (con) {
                con.Open();
                SqlCommand m_cmd = con.CreateCommand();
                SqlDataReader m_rdr;

                String m_q = "select * from Mode";
                m_cmd = new SqlCommand(m_q, con);

                m_rdr = m_cmd.ExecuteReader();
                while (m_rdr.Read()) {
                    modeList.Rows.Add(
                        (int)m_rdr["Mode_Number"],
                        (string)m_rdr["Mode_Name"],
                        (int)m_rdr["Min"],
                        (int)m_rdr["Max"],
                        (int)m_rdr["Count_WalkIn"]);
                    Console.Write(" getModes -> Added a row! ");
                }
                con.Close();
            }
            Console.Write(" \n returning modeList... \n ");
            return modeList;
        }
        private void Next() {
            //Called when next button is clicked

            //declare variables to be used on functions
            SqlConnection con = new SqlConnection(connection_string);

            Console.Write(" \n ** Calling function Set_Information_Queue() ** \n ");
            //This function increments queue and retrives information about the queue
            Set_Information_Queue(con);
            //using (con)
            //{
            //    con.Open();
            //    SqlCommand cmd = con.CreateCommand();
            //    SqlCommand cmd2 = con.CreateCommand();
            //    SqlCommand cmd3 = con.CreateCommand();
            //    cmd.CommandType = CommandType.Text;
            //    cmd2.CommandType = CommandType.Text;

            //    String query = "select * from Queue_Info where Servicing_Office = @Servicing_Office";
            //    String query2 = "select * from Main_Queue where Servicing_Office = @Servicing_Office";

            //    cmd = new SqlCommand(query, con);
            //    cmd.Parameters.AddWithValue("@Servicing_Office", Servicing_Office);
            //    cmd2 = new SqlCommand(query2, con);
            //    cmd2.Parameters.AddWithValue("@Servicing_Office", Servicing_Office);


            //    SqlDataReader rdr;
            //    SqlDataReader rdr2;

            //    //Transferring previous customer to logs
            //    rdr2 = cmd2.ExecuteReader();
            //    while (rdr2.Read()) {
            //        String c_qn = (int)rdr2["Queue_Number"];
            //        int c_so = Servicing_Office;
            //        String c_sn = rdr2["Student_No"].ToString();
            //        String c_tt = rdr2["Transaction_Type"]
            //    }


            //    //Selecting Customer




            //    rdr = cmd.ExecuteReader();
            //    int rowCount = 0;
            //    while (rdr.Read())
            //    { rowCount++; { if (rowCount > 0) { break; } } }
            //    if (rowCount > 0)
            //    {
            //        string q_cn = rdr["Current_Number"].ToString();
            //        string q_m = rdr["Mode"].ToString();
            //        string q_s = rdr["Status"].ToString();
            //        modeCounter = (int)rdr["Counter"];
            //    }
            //    else
            //    {
            //        MessageBox.Show("No queueing data found. Please start an instance of a serving kiosk first.");
            //    }

            //}
            //con.Close();



        }
        private void Set_Information_Queue(SqlConnection con) {
            //Retrieves information on the queue respective to its own Servicing Office
            using (con)
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                SqlCommand cmd2 = con.CreateCommand();
                SqlCommand cmd3 = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd2.CommandType = CommandType.Text;

                String query = "select * from Queue_Info where Servicing_Office = @Servicing_Office";
                String query2 = "update Queue_Info set Current_Number = @q_cn, Counter = @q_cntr where Servicing_Office = @Servicing_Office";

                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Servicing_Office", Servicing_Office);
                cmd2 = new SqlCommand(query2, con);
                SqlDataReader rdr;

                //Selecting Customer

                rdr = cmd.ExecuteReader();
                int rowCount = 0, tempCounter = 0;
                while (rdr.Read())
                { rowCount++; { if (rowCount > 0) { break; } } }
                if (rowCount > 0)
                {
                    //Retrieve queue info and set to variables
                    int q_cn = (int)rdr["Current_Number"];
                    int q_m = (int)rdr["Mode"];
                    string q_s = rdr["Status"].ToString();
                    tempCounter = (int)rdr["Counter"];

                    //Increment goes here
                    Console.Write(" \n incrementing q_cn [Queue_Current_Number \n ");
                    q_cn++;
                    Console.Write(" \n ** Calling Mode_Check with con and tempCounter (current mode counter) ** \n ");
                    Mode_Check(con,tempCounter);

                    cmd2.Parameters.AddWithValue("@q_cn", q_cn);
                    cmd2.Parameters.AddWithValue("@q_cntr", modeCounter);
                    cmd2.Parameters.AddWithValue("@Servicing_Office", Servicing_Office);
                    Console.Write("Writing to database...");
                    cmd2.ExecuteNonQuery();

                }
                else
                {
                    MessageBox.Show("No queueing data found. Please start an instance of a serving kiosk first.");
                }

            }
            con.Close();

        }
        private void Transfer_Customer_Logs() { }
        private void Transfer_Customer_Office() { }
        private void Mode_Check(SqlConnection con,int retrieved_counter) {
            int x = 0, min = 0, max = 0, current_queue_number = 0, count_walkin = 0;
            string mode_name = "xd";
            int mode_number = 0;

            current_queue_number = return_on_queue(con);


            foreach (DataRow row in table_Modes.Rows)
            {
                mode_number = (int)row["Mode_Number"];
                mode_name = row["Mode_Name"].ToString();
                min = (int)row["Min"];
                max = (int)row["Max"];
                count_walkin = (int)row["Count_WalkIn"];
                Console.Write(" [[" + current_queue_number + "]] >= " + min + " && <= " + max);
                Console.Write("wew"+mode_name);
                if (current_queue_number >= min && current_queue_number <= max) {
                    break;
                }
            }
            if (retrieved_counter < count_walkin)
            {
                // increment, continue accepting walk-ins
                Console.Write(" \n ** Mode_Check() -> Incrementing modeCounter ** \n ");
                modeCounter++;
            }
            else
            {
                Console.Write(" \n * Mode_Check() -> Resetting modeCounter ** \n ");
                modeCounter = 0;
                // reset the counter, add a code (not here) to pick a mobile user
            }
            //return x;
        }
        private int return_on_queue(SqlConnection con)
        {
            int a = 0;

            SqlCommand cmd3 = con.CreateCommand();
            SqlDataReader rdr2;
            cmd3.CommandText = "return_total_queue";
            cmd3.CommandType = CommandType.StoredProcedure;
            cmd3.Parameters.AddWithValue("ServicingOffice", Servicing_Office);
            cmd3.Connection = con;
            rdr2 = cmd3.ExecuteReader();
            while (rdr2.Read()) {
                Console.Write("AAAAAAAAAAAAAAAAAAAAAAA"+(int)rdr2["a"]);
                a = (int)rdr2["a"];
            }

            return a;
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
            Next();
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
