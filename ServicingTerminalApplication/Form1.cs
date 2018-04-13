using Firebase.Auth;
using Firebase.Auth.Payloads;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{
    
    public partial class Form1 : Form
    {
        #region APP INIT VARIABLES
        //Form3 f3 = (Form3)Application.OpenForms["form3"];
        Form f3 = (currentCustomer)Application.OpenForms["currentCustomer"];
        currentCustomer fnf = new currentCustomer();
        settingsForm frmSettings = new settingsForm();
        public int user_type { get; set; } = 0;
        public int user_id { get; set; } = 0;
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        static int PROGRAM_Servicing_Office = 1;
        static int PROGRAM_window = 2;
        private static int PROGRAM_modeCounter = 0;
        private static string PROGRAM_Servicing_Office_Name = string.Empty;
        private static string id = string.Empty;
        private static string s_id = string.Empty;
        private static string full_name = string.Empty;
        private static string transaction_type = string.Empty;
        private static string type = string.Empty;
        private static string _customer_queue_number = string.Empty;
        //private static int transaction_type_id;
        //private static int _pattern_max;
        //private static int _pattern_current;
        private static string w_temp_run = string.Empty;
        private static bool DoneServingRealCustomer = false;
        DataTable table_Modes;
        DataTable table_Transactions;
        DataTable table_Transactions_List;
        DataTable table_Servicing_Office;

        Stopwatch _SERVING_TIME = new Stopwatch();

        _Main_Queue Next_Customer = new _Main_Queue();
        _Main_Queue Previous_Customer = new _Main_Queue();
        _Main_Queue No_Customer = new _Main_Queue
        {
            Queue_Number = 0,
            Full_Name = "NULL",
            Servicing_Office = 0,
            Transaction_Type = 0,
            Type = "NULL",
            Customer_Queue_Number = "NULL"
        };
        #endregion
        public Form1(int _a_user_type, int _a_user_id)
        {
            #region CONSTRUCTOR
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
            this.TopMost = true;
            fnf.FirstNameUpdated += Fnf_FirstNameUpdated;
            fnf.Show();
            //Console.Write("\n Initializing form... Calling getModes() to set table_Modes \n ");
            table_Modes = getModes();
            table_Transactions = getTransactionInfo();
            table_Transactions_List = getTransactionList();
            table_Servicing_Office = getServicingOfficeList();
            //transaction_type_id = 0;
            //_pattern_max = 0;
            //_pattern_current = 0;
            w_temp_run += "@ 0Program started. ";
            w_temp_run += "@ 0All Datatables have been generated.";
            Previous_Customer = No_Customer;
            setThisServicingOfficeName();
            AddThisServicingTerminal();
            user_type = _a_user_type;
            user_id = _a_user_id;
            #endregion
        }
        #region METHODS
        private void setThisServicingOfficeName()
        {
            foreach (DataRow row in table_Servicing_Office.Rows)
            {
                int temp_id = (int)row["ID"];
                if (temp_id == PROGRAM_Servicing_Office)
                {
                    PROGRAM_Servicing_Office_Name = (string)row["Name"];
                    break;
                }
            }
        }
        private void AddThisServicingTerminal()
        {
            SqlConnection con = new SqlConnection(connection_string);
            try { con.Open(); }
            catch (SqlException e) { MessageBox.Show("Local Connection problem. This app will now close.");
                Environment.Exit(0);
            }
            //Check if same window and servicing office exists
            string QUERY_CheckIfWindowExists = "select TOP 1 * from Servicing_Terminal where Servicing_Office = @param_so and Window = @param_window ";
            SqlCommand check_svc_tmnl = new SqlCommand(QUERY_CheckIfWindowExists, con);

            check_svc_tmnl.Parameters.AddWithValue("@param_so",PROGRAM_Servicing_Office);
            check_svc_tmnl.Parameters.AddWithValue("@param_window", PROGRAM_window);
            
            object r = check_svc_tmnl.ExecuteScalar();
            if (r == null)
            {
                string QUERY_AddServicingTerminal = "insert into Servicing_Terminal (Name, Customer_Queue_Number, Servicing_Office, Window) values" +
                    "(@param0,@param1,@param2,@param3)";
                SqlCommand add_svc_tmnl = new SqlCommand(QUERY_AddServicingTerminal, con);
                
                add_svc_tmnl.Parameters.AddWithValue("@param0", PROGRAM_Servicing_Office_Name);
                add_svc_tmnl.Parameters.AddWithValue("@param1", " ");
                add_svc_tmnl.Parameters.AddWithValue("@param2", PROGRAM_Servicing_Office);
                add_svc_tmnl.Parameters.AddWithValue("@param3", PROGRAM_window);
                add_svc_tmnl.ExecuteNonQuery();
                //MessageBox.Show("no rows returned, adding new servicing terminal!");
            }
           // else MessageBox.Show("already exist...");
            
            con.Close();
        }
        
        private void z(String a)
        {
            w_temp_run += a + Environment.NewLine;
        }
        private void incrementQueueNumber(SqlConnection con, int q_so)
        {
            int a = 0;
            // increment queue number 
            SqlCommand cmd4;
            w_temp_run += "@ incrementQueueNumber has been called.";
            w_temp_run += "@ tbl.Queue_Info will be updated.";
            String query2 = "update Queue_Info set Current_Queue = Current_Queue+1, Customer_Queue_Number = @q_cqn where Servicing_Office = @Servicing_Office";
            cmd4 = new SqlCommand(query2, con);
            cmd4.Parameters.AddWithValue("@Servicing_Office", q_so);
            cmd4.Parameters.AddWithValue("@q_cqn", _customer_queue_number);
            cmd4.ExecuteNonQuery();
            w_temp_run += "@ New values for tbl.Queue_Info at Servicing Office [" + q_so + "]:";
            w_temp_run += "@ Check Current_Queue incremented by 1, Customer_Queue_Number changed to " + _customer_queue_number;
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
                _cmd1.Parameters.AddWithValue("@sn", PROGRAM_Servicing_Office);
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
        #endregion

        #region FIREBASE METHODS (NOT USED)
        private async void Terminal_Delete_MainQueue(int q_id)
        {
            firebase_Connection fcon = new firebase_Connection();

            await fcon.App_Delete_MainQueue(q_id);
        }
        private async void Terminal_Delete_QueueTransaction(string ID_Pattern)
        {
            firebase_Connection fcon = new firebase_Connection();

            await fcon.App_Delete_QueueTransaction(ID_Pattern);
        }
        private void Terminal_Update_MainQueue(int gqn, int q_nso, int res_Pattern_Increment, int q_id)
        {
            _Main_Queue mq = new _Main_Queue {
                Queue_Number = gqn,
                Servicing_Office = q_nso,
                Pattern_Current = res_Pattern_Increment
            };
            firebase_Connection fcon = new firebase_Connection();
            //Task.Run(() => conn.Update(this.hero)).Wait();
            //await fcon.App_Update_MainQueue(q_id,mq);
            Task.Run(() => fcon.App_Update_MainQueue(q_id,mq));
        }
        private void Terminal_Update_QueueInfo(int q_cn, int modeCounter, int _servicing_office, int window)
        {
            _Queue_Info a = new _Queue_Info {
                Current_Number = q_cn,
                Counter = modeCounter,
                Window = window
            };
            firebase_Connection fcon = new firebase_Connection();
            fcon.App_Update_QueueInfo(_servicing_office,a);

        }
        #endregion

        #region GET METHODS
        private int getNextCustomerID()
        {
            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                int a = 0;
                String _query3 = "select id from Main_Queue where Servicing_Office = @sn";
                SqlCommand _cmd3 = new SqlCommand(_query3, con);
                _cmd3.Parameters.AddWithValue("@sn", PROGRAM_Servicing_Office);
                object r = _cmd3.ExecuteScalar();
                if (r != null) a = (int)r;
                else
                {
                    //code to handle the null case here...
                    a = 0;
                }
                con.Close();
                return a;
            }

        }
        private string getTransactionTypeName(int _tt_id)
        {
            int id_temp = 0;
            string transactionName_temp = "";
            foreach (DataRow row in table_Transactions.Rows)
            {
                id_temp = (int)row["Transaction_Type"];
                if (id_temp == _tt_id)
                {
                    transactionName_temp = (string)row["Transaction_Name"];
                    break;
                }
            }
            return transactionName_temp;
        }
        private DataTable getServicingOfficeList()
        {
            DataTable ServicingOfficeList = new DataTable();
            ServicingOfficeList.Columns.Add("Name", typeof(string));
            ServicingOfficeList.Columns.Add("ID", typeof(int));

            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                SqlCommand t_cmd = con.CreateCommand();
                SqlDataReader t_rdr;

                String t_q = "select * from Servicing_Office";
                t_cmd = new SqlCommand(t_q, con);

                t_rdr = t_cmd.ExecuteReader();
                while (t_rdr.Read())
                {
                    ServicingOfficeList.Rows.Add(
                       (string)t_rdr["Name"],
                       (int)t_rdr["ID"]);
                }
                con.Close();
            }
            //Console.Write(" \n returning transactionList... \n ");
            return ServicingOfficeList;
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
                    //Console.Write(" getTransactions -> Added a row! ");
                }
                con.Close();
            }
            //Console.Write(" \n returning transactionList... \n ");
            return transactionList;
        }
        private int nextServicingOffice(int _pattern_no, int _transaction_id)
        {
            //_pattern_no++;
            int a = 0;
            int temp_pattern_no = 0;
            int temp_transaction_id = 0;
            foreach (DataRow row in table_Transactions_List.Rows)
            {
                temp_pattern_no = (int)row["Pattern_No"];
                temp_transaction_id = (int)row["Transaction_ID"];
                //Console.Write(" \n retrieving the next servicing office ");
                if (_transaction_id == temp_transaction_id && temp_pattern_no == _pattern_no)
                {
                    a = (int)row["Servicing_Office"];
                    break;
                }
            }
            z("------>>>nextServicingOffice returns = "+a+"at Pattern#"+temp_pattern_no);
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
                    //Console.Write(" getTransactions -> Added a row! ");
                }
                con.Close();
            }
            //Console.Write(" \n returning transactionType... \n ");
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
                   // Console.Write(" getModes -> Added a row! ");
                }
                con.Close();
            }
           // Console.Write(" \n returning modeList... \n ");
            return modeList;
        }
        
        private int getCurrentQueueCounter(int What_Servicing_Office, SqlConnection con)
        {
            int x = 0;
            SqlCommand internal_Command1;
            string QUERY_retrieve_QueueInfo_Counter = "select TOP 1 Counter from Queue_Info where Servicing_Office = @param_so";
            internal_Command1 = new SqlCommand(QUERY_retrieve_QueueInfo_Counter, con);
            internal_Command1.Parameters.AddWithValue("@param_so", What_Servicing_Office);
            x = (int)internal_Command1.ExecuteScalar();
            return x;
        }
        private int geQueueCurrentNumber(int What_Servicing_Office, SqlConnection con)
        {
            int x = 0;
            SqlCommand internal_Command1;
            string QUERY_retrieve_QueueInfo_QueueNumber = "select TOP 1 Current_Number from Queue_Info where Servicing_Office = @param_so";
            internal_Command1 = new SqlCommand(QUERY_retrieve_QueueInfo_QueueNumber, con);
            internal_Command1.Parameters.AddWithValue("@param_so", What_Servicing_Office);
            x = (int)internal_Command1.ExecuteScalar();
            return x;

        }
        private int getQueueCurrentQueue(int What_Servicing_Office, SqlConnection con)
        {
            int x = 0;
            SqlCommand internal_Command1;
            string QUERY_retrieve_QueueInfo_CurrentQueue = "select TOP 1 Current_Queue from Queue_Info where Servicing_Office = @param_so";
            internal_Command1 = new SqlCommand(QUERY_retrieve_QueueInfo_CurrentQueue, con);
            internal_Command1.Parameters.AddWithValue("@param_so", What_Servicing_Office);
            x = (int)internal_Command1.ExecuteScalar();
            return x;

        }
        private int getCurrentQueueAmount(int What_Servicing_Office, SqlConnection con)
        {
            // Counts how many customers are on the queue of this Servicing_Office
            int x = 0;
            SqlCommand internal_Command1;
            string QUERY_retrieve_MainQueue_amount = "select COUNT(id) from Main_Queue where Servicing_Office = @param_so";
            internal_Command1 = new SqlCommand(QUERY_retrieve_MainQueue_amount, con);
            internal_Command1.Parameters.AddWithValue("@param_so", What_Servicing_Office);
            x = (int)internal_Command1.ExecuteScalar();
            return x;

        }
        
        private void Mode_Check(SqlConnection con,int retrieved_counter) {
            int min = 0, max = 0, current_queue_amount = 0, count_walkin = 0;
            string mode_name = "PROGRAM_MODE_NOT_FOUND_ON_DB";
            int mode_number = 0;

            current_queue_amount = getCurrentQueueAmount(PROGRAM_Servicing_Office,con);

            foreach (DataRow row in table_Modes.Rows)
            {
                // checks what mode is to be used when serving customers on queue
                // January 30, 2018 -- please recheck if working on different [Servicing Offices]
                mode_number = (int)row["Mode_Number"];
                mode_name = row["Mode_Name"].ToString();
                min = (int)row["Min"];
                max = (int)row["Max"];
                count_walkin = (int)row["Count_WalkIn"];
                //Console.Write(" [[" + current_queue_amount + "]] >= " + min + " && <= " + max);
                //Console.Write("wew"+mode_name);
                if (current_queue_amount >= min && current_queue_amount <= max) {
                    break;
                }
            }
            if (retrieved_counter < count_walkin)
            {
                // increment, continue accepting walk-ins
                //Console.Write(" \n ** Mode_Check() -> Incrementing modeCounter ** \n ");
                PROGRAM_modeCounter++;
            }
            else
            {
                //Console.Write(" \n * Mode_Check() -> Resetting modeCounter ** \n ");
                PROGRAM_modeCounter = 0;
                // reset the counter, add a code (not here) to pick a mobile user
            }
            //return x;

            
        }
        private void Fnf_FirstNameUpdated(object sender, updateForm e)
        {
            // to be used to show data on form #3
            if(e != null && e.CQN != null)
            {   fnf.setCQN = e.CQN;
                fnf.setID = e.s_id;
                fnf.setName = e.full_name;
                fnf.setTransaction = e.transaction_type;
                fnf.setType = e.type;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }
        #endregion

        #region OBJECT METHODS
        private void button1_Click_1(object sender, EventArgs e)
        {

            if (frmSettings.Visible)
                return;

            frmSettings = new settingsForm();
            frmSettings.Show();
        }
        private void onMouseClick(object sender, EventArgs e)
        {
            if (((PictureBox)sender) == pictureBox1)
            {
                // If next button is clicked
                Next();
            }
            else
            {
                // If delete button is clicked
            }
        }

        private void onHover(object sender, EventArgs e)
        {
            if(((PictureBox)sender) == pictureBox1)
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.nextBtn_pressed;
            }
            else
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.deleteBtn_pressed;
            }
        }

        private void onMouseLeave(object sender, EventArgs e)
        {
            if (((PictureBox)sender) == pictureBox1)
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.nextBtn;
            }
            else
            {
                ((PictureBox)sender).Image = ServicingTerminalApplication.Properties.Resources.deleteBtn;
            }
        }
        #endregion

        #region MAIN METHODS
        private void Next()
        {
            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                // Called when next button is clicked
                // declare variables to be used
                String QUERY_next_customer_mq = "select TOP 1 Pattern_Current, id, Queue_Number, Full_Name, Servicing_Office, Transaction_Type, Type, Customer_Queue_Number from Main_Queue where Servicing_Office = @param_so and Queue_Status = @param_qs order by Queue_Number asc";
                String QUERY_next_customer_tq = "select TOP 1 Pattern_Current, id, Queue_Number, Full_Name, Servicing_Office, Transaction_Type, Type, Customer_Queue_Number from Main_Queue where Servicing_Office = @param_so and Queue_Status = @param_qs order by Queue_Number asc";
                String QUERY_mq_next_customer_update_on_success = "update Main_Queue set Queue_Status = @param_qs where id = @param_uniqueid";
                String QUERY_tq_next_customer_update_on_success = "update Transfer_Queue set Queue_Status = @param_qs where Main_Queue_ID = @param_uniqueid";
                SqlCommand Command1,Command2,CommandQuickUpdate;
                SqlDataReader Reader1,Reader2;
                
                bool _READ_COUNTER = false;

                Command1 = new SqlCommand(QUERY_next_customer_mq, con);
                Command1.Parameters.AddWithValue("@param_so",PROGRAM_Servicing_Office);
                Command1.Parameters.AddWithValue("@param_qs", "Waiting");
                Reader1 = Command1.ExecuteReader();

                while (Reader1.Read())
                {
                    _READ_COUNTER = true;
                    z("Customer found on main_queue");
                    Next_Customer = new _Main_Queue
                    {
                        Queue_Number = (int)Reader1["Queue_Number"],
                        Full_Name = (string)Reader1["Full_Name"],
                        Servicing_Office = (int)Reader1["Servicing_Office"],
                        Transaction_Type = (int)Reader1["Transaction_Type"],
                        Type = ((Boolean)Reader1["Type"] == false) ? "Student" : "Guest",
                        Customer_Queue_Number = (string)Reader1["Customer_Queue_Number"],
                        ID = (int)Reader1["id"],
                        Student_No = ((Boolean)Reader1["Type"] == false) ? (string)Reader1["Student_No"] : "",
                        Pattern_Current = (int)Reader1["Pattern_Current"]
                    };
                    CommandQuickUpdate = new SqlCommand(QUERY_mq_next_customer_update_on_success, con);
                    CommandQuickUpdate.Parameters.AddWithValue("@param_qs", "Serving");
                    CommandQuickUpdate.Parameters.AddWithValue("@param_uniqueid", (int)Reader1["id"]);
                    CommandQuickUpdate.ExecuteNonQuery();
                    z("Setting the customer as \"Currently Serving\" finished.");

                }
                if (!_READ_COUNTER)
                {
                    Command2 = new SqlCommand(QUERY_next_customer_tq, con);
                    Command2.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                    Command2.Parameters.AddWithValue("@param_qs", "Waiting");
                    Reader2 = Command2.ExecuteReader();

                    while (Reader2.Read())
                    {
                        z("read found on transfer queue");
                        _READ_COUNTER = true;
                        Next_Customer = new _Main_Queue
                        {
                            Queue_Number = (int)Reader2["Queue_Number"],
                            Full_Name = (string)Reader2["Full_Name"],
                            Servicing_Office = (int)Reader2["Servicing_Office"],
                            Transaction_Type = (int)Reader2["Transaction_Type"],
                            Type = ((Boolean)Reader2["Type"] == false) ? "Student" : "Guest",
                            Customer_Queue_Number = (string)Reader2["Customer_Queue_Number"],
                            ID = (int)Reader2["Main_Queue_ID"],
                            Student_No = ((Boolean)Reader1["Type"] == false) ? (string)Reader1["Student_No"] : "",
                            Pattern_Current = (int)Reader2["Pattern_Current"]
                        };
                        CommandQuickUpdate = new SqlCommand(QUERY_tq_next_customer_update_on_success, con);
                        CommandQuickUpdate.Parameters.AddWithValue("@param_qs", "Serving");
                        CommandQuickUpdate.Parameters.AddWithValue("@param_uniqueid", (int)Reader2["id"]);
                        CommandQuickUpdate.ExecuteNonQuery();
                    }
                    if (_READ_COUNTER) MessageBox.Show("Customer found on Transfer Table.");
                }
                

                // Check if timer run a little bit last time = there was a customer
                if (_SERVING_TIME.Elapsed.Milliseconds > 0)
                {
                    // Save this to DB
                    int _serving_Seconds = _SERVING_TIME.Elapsed.Seconds;
                    int _serving_Minutes = _SERVING_TIME.Elapsed.Minutes;
                    int _serving_Hours = _SERVING_TIME.Elapsed.Hours;
                    int _serving_TotalSeconds = (int)_SERVING_TIME.Elapsed.TotalSeconds;

                    // Check first if 5 entries where written for average
                    string QUERY_Count_ServingTime = "select count(id) from Serving_Time where Servicing_Office = @param1";
                    SqlCommand CMD_Count_ServingTime = new SqlCommand(QUERY_Count_ServingTime, con);
                    CMD_Count_ServingTime.Parameters.AddWithValue("@param1", PROGRAM_Servicing_Office);

                    if ((int)CMD_Count_ServingTime.ExecuteScalar() > 4)
                    {
                        int _serving_time_average = 1;
                        // Delete oldest entry
                        string QUERY_Delete_Oldest_ServingTime = "DELETE FROM Serving_Time WHERE id IN " +
                            "(SELECT TOP 1 id FROM Serving_Time where Servicing_Office = @param1 order by id asc)";
                        SqlCommand CMD_Delete_Oldest_ServingTime = new SqlCommand(QUERY_Delete_Oldest_ServingTime, con);
                        CMD_Delete_Oldest_ServingTime.Parameters.AddWithValue("@param1", PROGRAM_Servicing_Office);
                        CMD_Delete_Oldest_ServingTime.ExecuteNonQuery();

                        z("-->Deleting oldest row on ast");

                        // Average all entries
                        string QUERY_Average_ServingTime = "select AVG(Duration_Seconds) from Serving_Time where " +
                            "Servicing_Office = @param1";
                        SqlCommand CMD_Average_ServingTime = new SqlCommand(QUERY_Average_ServingTime, con);
                        CMD_Average_ServingTime.Parameters.AddWithValue("@param1", PROGRAM_Servicing_Office);

                        _serving_time_average = (int)CMD_Average_ServingTime.ExecuteScalar();

                        z("-->Getting average serving time");

                        // Write to Table: Queue_Info
                        string QUERY_Update_Avg_Serving_Time = "update Queue_Info set Avg_Serving_Time = @param1 where " +
                            "Servicing_Office = @param2 ";
                        SqlCommand CMD_Update_QueueInfo_AST = new SqlCommand(QUERY_Update_Avg_Serving_Time, con);
                        CMD_Update_QueueInfo_AST.Parameters.AddWithValue("@param1", _serving_time_average);
                        CMD_Update_QueueInfo_AST.Parameters.AddWithValue("@param2", PROGRAM_Servicing_Office);

                        CMD_Update_QueueInfo_AST.ExecuteNonQuery();

                        z("-->Updating AST on Queue_Info");

                    }
                    string QUERY_Insert_ServingTime = "insert into Serving_Time (Servicing_Office, Duration_Seconds) values" +
                        " (@param1, @param2) ";
                    SqlCommand CMD_Insert_ServingTime = new SqlCommand(QUERY_Insert_ServingTime, con);

                    CMD_Insert_ServingTime.Parameters.AddWithValue("@param1", PROGRAM_Servicing_Office);
                    CMD_Insert_ServingTime.Parameters.AddWithValue("@param2", _serving_TotalSeconds);

                    CMD_Insert_ServingTime.ExecuteNonQuery();
                    z("A Serving time is just inserted.");
                    // Stop time before starting another
                    _SERVING_TIME.Stop();

                    // Reset
                    _SERVING_TIME.Reset();
                }
                else
                {

                }
                if (!_READ_COUNTER) {
                    //Check first if the Servicing App is holding a real customer to move him to the next one or not.
                    MoveCustomerToNextOrDelete(con);
                    Previous_Customer = No_Customer;
                    z("Empty Customers.");
                }
                else
                {
                    // Customer found.

                    // Show the information of the customer.
                    updateForm nuea = new updateForm();
                    nuea.CQN = Next_Customer.Customer_Queue_Number.ToString();
                    nuea.type = Next_Customer.Type;
                    nuea.s_id = Next_Customer.Student_No;
                    nuea.full_name = Next_Customer.Full_Name;
                    nuea.transaction_type = getTransactionTypeName(Next_Customer.Transaction_Type);
                    fnf.OnFirstNameUpdated(nuea);

                    // Start time serving this customer
                    _SERVING_TIME.Start();
                    z("--> Serving Time is now running.");

                    // Running calculations for counter
                    int internal_Current_Counter = getCurrentQueueCounter(PROGRAM_Servicing_Office, con);
                    Mode_Check(con, internal_Current_Counter);

                    MoveCustomerToNextOrDelete(con);

                    Previous_Customer = Next_Customer;


                    //}
                    //Update Queue_Info everytime next is clicked
                    SqlCommand Command4;
                    String QUERY_update_QueueInfo = "update Queue_Info set Current_Number = Current_Number+1, Counter = @param_counter, Window = @param_window, Customer_Queue_Number = @param_cqn where Servicing_Office = @param_so";
                    Command4 = new SqlCommand(QUERY_update_QueueInfo, con);

                    Command4.Parameters.AddWithValue("@param_counter", PROGRAM_modeCounter);
                    Command4.Parameters.AddWithValue("@param_window", PROGRAM_window);
                    Command4.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                    Command4.Parameters.AddWithValue("@param_cqn", Next_Customer.Customer_Queue_Number);

                    Command4.ExecuteNonQuery();

                    SqlCommand Command_Update_svc_tmnl;
                    String QUERY_update_svc_tmnl = "update Servicing_Terminal set Customer_Queue_Number = @param_cqn where Servicing_Office = @param_so and Window = @param_window";
                    Command_Update_svc_tmnl = new SqlCommand(QUERY_update_svc_tmnl, con);

                    Command_Update_svc_tmnl.Parameters.AddWithValue("@param_cqn", Next_Customer.Customer_Queue_Number);
                    Command_Update_svc_tmnl.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                    Command_Update_svc_tmnl.Parameters.AddWithValue("@param_window", PROGRAM_window);

                    Command_Update_svc_tmnl.ExecuteNonQuery();

                    z("Updating Queue_Info when next is clicked finished!");

                    Console.WriteLine(w_temp_run);
                    w_temp_run = string.Empty;
                }
                



            }
            con.Close();


        }
        private void MoveCustomerToNextOrDelete(SqlConnection con)
        {
            Previous_Customer.Pattern_Current++;
            //z("Previous Customer Pattern Current is " + Previous_Customer.Pattern_Current);
            int _NEXT_Servicing_Office = nextServicingOffice(Previous_Customer.Pattern_Current, Previous_Customer.Transaction_Type);
            //z("Checking Pattern Current again " + Previous_Customer.Pattern_Current);
            z("START of MoveCustomerToNextOrDelete function.");
            //If the Previous Customer has no more "next" Servicing Office, remove him from the Main Queue
            if (_NEXT_Servicing_Office == 0)
            {
                if (Previous_Customer.ID > 0)
                {
                    //Remove the customer from the Table:Main_Queue
                    SqlCommand deleteCommand;
                    String QUERY_delete_MainQueue_onNext = "delete from Main_Queue where id = @param_unique_id";
                    deleteCommand = new SqlCommand(QUERY_delete_MainQueue_onNext, con);
                    deleteCommand.Parameters.AddWithValue("@param_unique_id", Previous_Customer.ID);
                    z("Deleting from Table: Main_Queue with id of " + Previous_Customer.ID);
                    z("Customer id#" + Previous_Customer.ID + " deleted!");
                    deleteCommand.ExecuteNonQuery();
                    z("Real customer is deleted.");
                }
                else { z("Previous customer is a null."); z("NULL Customer is ignored. Nothing is deleted"); }
            }
            else
            {
                //Transfer the Customer to the next Servicing Office
                //Change the values of the served Customer at Main_Queue
                SqlCommand Command6;
                //Next line -> Update Table: Main Queue of Previous Customer
                //Sending #customer to the next Servicing Office
                String QUERY_update_MainQueue_onNext = "update Main_Queue set Queue_Number = @param_qn, Servicing_Office = @param_so, Pattern_Current = Pattern_Current + 1, Queue_Status = @param_qs where id = @param_unique_id";
                Command6 = new SqlCommand(QUERY_update_MainQueue_onNext, con);
                //Get the Queue_Number from the tip of Table:Queue_Info -> Current_Queue
                int TEMP_getCustomerQueueNumberOnNext = getQueueCurrentQueue(_NEXT_Servicing_Office, con);
                //Declare parameters
                Command6.Parameters.AddWithValue("@param_qn", TEMP_getCustomerQueueNumberOnNext);
                Command6.Parameters.AddWithValue("@param_so", _NEXT_Servicing_Office);
                Command6.Parameters.AddWithValue("@param_unique_id", Previous_Customer.ID);
                Command6.Parameters.AddWithValue("@param_qs", "Waiting");
                z("The update Main_Queue for moving a customer run with the following values:");
                z("Queue Number : " + TEMP_getCustomerQueueNumberOnNext);
                z("Servicing Office : " + _NEXT_Servicing_Office);
                z("Unique id of : " + Previous_Customer.ID);
                z("Pattern Number is :" + Previous_Customer.Pattern_Current);
                //Execute Commands
                Command6.ExecuteNonQuery();
                //Increment the Previous_Customer 

                z("Main_Queue and Queue_Info updated since next SO is found.");
                z("NEXT Servicing Office is ->" + _NEXT_Servicing_Office);
                z("This Query finished : update Main_Queue set Queue_Number = " + TEMP_getCustomerQueueNumberOnNext + ", Servicing_Office = " + _NEXT_Servicing_Office + " where id = " + Previous_Customer.ID);

                SqlCommand Command5;
                String QUERY_update_QueueInfo_addThisOnNext = "update Queue_Info set Current_Queue = Current_Queue + 1 where Servicing_Office = @param_so";

                Command5 = new SqlCommand(QUERY_update_QueueInfo_addThisOnNext, con);
                Command5.Parameters.AddWithValue("@param_so", _NEXT_Servicing_Office);

                Command5.ExecuteNonQuery();

                z("This Query finished : update Queue_Info increment Current_Queue where Servicing_Office = " + _NEXT_Servicing_Office + " where id = " + Previous_Customer.ID);


            }
            z("END of MoveCustomerToNextOrDelete function.");
        }
        #endregion 


    }
}
