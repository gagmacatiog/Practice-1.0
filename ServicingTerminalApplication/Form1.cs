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
        static int Servicing_Office = 2;
        static int window = 1;
        static int modeCounter = 0;
        private static string id = string.Empty;
        private static string s_id = string.Empty;
        private static string full_name = string.Empty;
        private static string transaction_type = string.Empty;
        private static string type = string.Empty;
        private static int transaction_type_id;
        private static int _pattern_max;
        private static int _pattern_current;
        //private static int
        DataTable table_Modes;
        DataTable table_Transactions;
        DataTable table_Transactions_List;
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
            table_Transactions = getTransactionInfo();
            table_Transactions_List = getTransactionList();
            transaction_type_id = 0;
            _pattern_max = 0;
            _pattern_current = 0;

        }
        private bool checkIfNextCustomerExist(int q_cn)
        {
            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                bool a = false;
                String _query1 = "select TOP 1 Servicing_Office from Main_Queue where Queue_Number = @qn and Servicing_Office = @sn";
                SqlCommand _cmd1 = new SqlCommand(_query1, con);
                _cmd1.Parameters.AddWithValue("@sn", Servicing_Office);
                _cmd1.Parameters.AddWithValue("@qn", q_cn);
                object r = _cmd1.ExecuteScalar();
                if (r != null) a = true;
                else
                {
                    //code to handle the null case here...
                    MessageBox.Show("False");
                }
                con.Close();
                return a;

            }
        }
        private DataTable getTransactionList()
        {
            DataTable transactionList = new DataTable();
            transactionList.Columns.Add("Transaction_ID", typeof(int));
            transactionList.Columns.Add("Servicing_Office", typeof(int));
            transactionList.Columns.Add("Pattern_No", typeof(int));

            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                SqlCommand t_cmd = con.CreateCommand();
                SqlDataReader t_rdr;

                String t_q = "select * from Transaction_List";
                t_cmd = new SqlCommand(t_q, con);

                t_rdr = t_cmd.ExecuteReader();
                while (t_rdr.Read())
                {
                    transactionList.Rows.Add(
                       (int)t_rdr["Transaction_ID"],
                       (int)t_rdr["Servicing_Office"],
                       (int)t_rdr["Pattern_No"]);
                    Console.Write(" getTransactions -> Added a row! ");
                }
                con.Close();
            }
            Console.Write(" \n returning transactionList... \n ");
            return transactionList;
        }
        private int nextServicingOffice(int _pattern_no, int _transaction_id)
        {
            int a = 0;
            int temp_pattern_no = 0;
            int temp_transaction_id = 0;
            foreach (DataRow row in table_Transactions_List.Rows)
            {
                temp_pattern_no = (int)row["Pattern_No"];
                temp_transaction_id = (int)row["Transaction_ID"];
                Console.Write(" \n retrieving the next servicing office ");
                if (_transaction_id == temp_transaction_id && temp_pattern_no == _pattern_no)
                {
                    a = (int)row["Servicing_Office"];
                    break;
                }
            }
            return a;
        }
        private DataTable getTransactionInfo()
        {
            // *This function is only to be used once every program run.
            //  Retrieves list of possible Transaction Names from the database
            //  and stores them on the program.
            DataTable transactionType = new DataTable();
            transactionType.Columns.Add("Transaction_Type", typeof(int));
            transactionType.Columns.Add("Transaction_Name", typeof(string));
            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                SqlCommand m_cmd = con.CreateCommand();
                SqlDataReader m_rdr;

                String m_q = "select * from Transaction_Type";
                m_cmd = new SqlCommand(m_q, con);

                m_rdr = m_cmd.ExecuteReader();
                while (m_rdr.Read())
                {
                    transactionType.Rows.Add(
                        (int)m_rdr["id"],
                        (string)m_rdr["Transaction_Name"]);
                    Console.Write(" getTransactions -> Added a row! ");
                }
                con.Close();
            }
            Console.Write(" \n returning transactionType... \n ");
            return transactionType;
        }

        private DataTable getModes(){
            // *This function is only to be used once every program run.
            //  Retrieves list of possible queueing modes from the database
            //  and stores them on the program.
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
                String query2 = "update Queue_Info set Current_Number = @q_cn, Counter = @q_cntr, Window = @q_w where Servicing_Office = @Servicing_Office";

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
                    // Check if there is people on queue

                    // Retrieve queue info and set to variables
                    int q_cn = (int)rdr["Current_Number"];
                    int q_m = (int)rdr["Mode"];
                    string q_s = rdr["Status"].ToString();
                    tempCounter = (int)rdr["Counter"];

                    if (q_cn < getQueueNumber(con, Servicing_Office))
                    {
                        //Increment goes here
                        Console.Write(" \n incrementing q_cn [Queue_Current_Number] \n ");
                        q_cn++;
                        Console.Write(" \n ** Calling Mode_Check with con and tempCounter (current mode counter) ** \n ");
                        Mode_Check(con, tempCounter);

                        cmd2.Parameters.AddWithValue("@q_cn", q_cn);
                        cmd2.Parameters.AddWithValue("@q_cntr", modeCounter);
                        cmd2.Parameters.AddWithValue("@Servicing_Office", Servicing_Office);
                        cmd2.Parameters.AddWithValue("@q_w", window);
                        Console.Write("Writing to database...");
                        cmd2.ExecuteNonQuery();
                        setCustomerInformation(con, (q_cn-1));
                    }
                    else { MessageBox.Show("No customers on queue."); }

                }
                else
                {
                    MessageBox.Show("No queueing data found. Please start an instance of a serving kiosk first.");
                }

            }
            con.Close();

        }
        private int getQueueNumber(SqlConnection con, int q_so)
        {
            // retrieves queue number
            int res = 0;

            SqlCommand cmd3;
            String query = "select Current_Queue from Queue_Info where Servicing_Office = @Servicing_Office";
            cmd3 = new SqlCommand(query, con);
            cmd3.Parameters.AddWithValue("@Servicing_Office", q_so);
            //SqlDataReader rdr2;
            //rdr2 = cmd3.ExecuteReader();
            //while (rdr2.Read()) { res = (int)rdr2["Current_Queue"]; }
            res = (int)cmd3.ExecuteScalar();
            Console.Write("--RETURNING-> getQueueNumber[" + res + "]");
            
            return res;
        }
        private void setCustomerInformation(SqlConnection con, int q_cn) {

            // Called when next button is clicked.
            if (checkIfNextCustomerExist(q_cn))
            {
                // Calls consecutive functions for the next customer to be served.

                SqlCommand cmd4, _cmd4, _cmd0;
                String query = "select TOP 1 Queue_Number,Type,Student_No,Full_name,Transaction_Type,Pattern_Current,Pattern_Max from Main_Queue where Queue_Number = @q_cn and Servicing_Office = @sn";
                String _query_insert = "update Main_Queue set Servicing_Office = @q_nso, Pattern_Current = Pattern_Current + 1 where Queue_Number = @q_cn and Servicing_Office = @sn";
                String _query_delete = "delete from Main_Queue where Queue_Number = @q_cn and Servicing_Office = @sn";
                //Console.Write(" \n \n deleting from main queue qn = " + q_cn + " ;; sn = " + Servicing_Office);
                String _query_delete_queue_pattern = "delete from Queue_Transaction where Main_Queue_ID = @q_cn and Pattern_No = @q_pn";
                cmd4 = new SqlCommand(query, con);

                cmd4.Parameters.AddWithValue("@q_cn", q_cn);
                cmd4.Parameters.AddWithValue("@sn", Servicing_Office);
                SqlDataReader rdr3;
                rdr3 = cmd4.ExecuteReader();
                if (rdr3.Read())
                {
                    MessageBox.Show("HELLO! there is a Servicing_Office #2");
                    id = rdr3["Queue_Number"].ToString();
                    type = ((Boolean)rdr3["Type"] == false) ? "Student" : "Guest";
                    s_id = rdr3["Student_No"].ToString();
                    full_name = rdr3["Full_name"].ToString();
                    _pattern_max = (int)rdr3["Pattern_Max"];
                    _pattern_current = (int)rdr3["Pattern_Current"];
                    transaction_type_id = (int)rdr3["Transaction_Type"];
                    foreach (DataRow row in table_Transactions.Rows)
                    {
                        int _id = (int)row["Transaction_Type"];
                        if (_id == transaction_type_id)
                        {
                            transaction_type = (String)row["Transaction_Name"];
                        }
                    }
                    if (_pattern_current <= _pattern_max)
                    {
                        _cmd4 = new SqlCommand(_query_insert, con);
                        _cmd4.Parameters.AddWithValue("@q_cn", q_cn);
                        _cmd4.Parameters.AddWithValue("@sn", Servicing_Office);
                        _cmd4.Parameters.AddWithValue("@q_nso", nextServicingOffice(_pattern_current, transaction_type_id));
                        _cmd4.ExecuteNonQuery();
                    }
                    else
                    {
                        _cmd4 = new SqlCommand(_query_delete, con);
                        _cmd4.Parameters.AddWithValue("@q_cn", q_cn);
                        _cmd4.ExecuteNonQuery();
                    }
                    Console.Write("\n\n using _cmd0 -- q_cn = " + q_cn + " _pattern_current = " + _pattern_current);
                    _cmd0 = new SqlCommand(_query_delete_queue_pattern, con);
                    _cmd0.Parameters.AddWithValue("@q_cn", q_cn);
                    _cmd0.Parameters.AddWithValue("@q_pn", _pattern_current);
                    _cmd0.ExecuteNonQuery();

                    updateForm nuea = new updateForm();
                    nuea.id = id;
                    nuea.type = type;
                    nuea.s_id = s_id;
                    nuea.full_name = full_name;
                    nuea.transaction_type = transaction_type;
                    fnf.OnFirstNameUpdated(nuea);
                }

                //nuea.id = "wew";
                //nuea.type = "wew";
                //nuea.s_id = "wew";
                //nuea.full_name = "wew";
                //nuea.transaction_type = "wew";
                //fnf.OnFirstNameUpdated(nuea);
                Console.Write("updating using fnf...");

            }else { MessageBox.Show("Nibba doesn't exist."); }
        }
        private void Transfer_Customer_Logs() { }
        private void Transfer_Customer_Office() { }
        private void Mode_Check(SqlConnection con,int retrieved_counter) {
            int min = 0, max = 0, current_queue_number = 0, count_walkin = 0;
            string mode_name = "xd";
            int mode_number = 0;

            current_queue_number = return_on_queue(con);

            foreach (DataRow row in table_Modes.Rows)
            {
                // checks what mode is to be used when serving customers on queue
                // January 30, 2018 -- please recheck if working on different [Servicing Offices]
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
            // counts how many customers on queue at a [Servicing Office]
            int a = 0;
            String query4 = "select count(*) as a from Main_Queue where Servicing_Office = @sn";
            SqlCommand cmd3 = new SqlCommand(query4, con);
            SqlDataReader rdr2;
            cmd3.Parameters.AddWithValue("@sn", Servicing_Office);
            rdr2 = cmd3.ExecuteReader();
            while (rdr2.Read()) { a = (int)rdr2["a"]; }
            // execute return_total_queue
            return a;
        }
        private void Fnf_FirstNameUpdated(object sender, updateForm e)
        {
            // to be used to show data on form #3
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
