using Firebase.Auth;
using Firebase.Auth.Payloads;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        //Form f3 = (currentCustomer)Application.OpenForms["currentCustomer"];
        //currentCustomer fnf = new currentCustomer();
        settingsForm frmSettings = new settingsForm();
        FormHoldCustomers frmHoldCustomers;
        public int user_type { get; set; } = 0;
        public int user_id { get; set; } = 0;
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        static int PROGRAM_Servicing_Office = 1;
        int PROGRAM_window = 0;
        private static int PROGRAM_modeCounter = 0;
        private bool PROGRAM_ServeMobileCustomer = false;
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
        private string w_temp_run = string.Empty;
        DataTable table_Modes;
        DataTable table_Transactions;
        DataTable table_Transactions_List;
        DataTable table_Servicing_Office;

        Stopwatch _SERVING_TIME = new Stopwatch();

        public _Main_Queue Next_Customer = new _Main_Queue();
        public _Main_Queue Previous_Customer = new _Main_Queue();
        public _Main_Queue No_Customer = new _Main_Queue
        {
            Queue_Number = 0,
            Full_Name = "NULL",
            Servicing_Office = 0,
            Transaction_Type = 0,
            Type = "NULL",
            Customer_Queue_Number = "NULL",
            Customer_From = "NULL"
        };
        public _Main_Queue Clear_Customer = new _Main_Queue
        {
            Queue_Number = 0,
            Full_Name = " ",
            Servicing_Office = 0,
            Transaction_Type = 0,
            Type = " ",
            Customer_Queue_Number = " ",
            Customer_From = " "
        };
        public _Main_Queue Hold_Customer = new _Main_Queue();

        public List<_Main_Queue> LIST_Customers_On_Hold = new List<_Main_Queue>();
        #endregion
        public Form1( int _a_user_id, int _a_user_window, int _a_servicing_office_id, string _a_servicing_office_name)
        {
            #region CONSTRUCTOR
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
            /**
             * Disabled when removed currentCustomer (April 24, 2018)
            fnf.FirstNameUpdated += Fnf_FirstNameUpdated;
            fnf.Show();**/
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
            Hold_Customer = No_Customer;
            //setThisServicingOfficeName();
            user_id = _a_user_id;
            PROGRAM_window = _a_user_window;
            PROGRAM_Servicing_Office = _a_servicing_office_id;
            PROGRAM_Servicing_Office_Name = _a_servicing_office_name;
            AddThisServicingTerminal();
            RefreshHoldList();
            MessageBox.Show("You are currently serving "+PROGRAM_Servicing_Office_Name+" at Window " + PROGRAM_window.ToString()+"!"
                ,"Notice");
            #endregion
        }
        #region METHODS
        public void updateCustomerInfoShown(_Main_Queue thisCustomer)
        {
            textCQN.Text = thisCustomer.Customer_Queue_Number.ToString();
            textType.Text = thisCustomer.Type;
            textID.Text = thisCustomer.Student_No;
            textName.Text = thisCustomer.Full_Name;
            textTransaction.Text = getTransactionTypeName(thisCustomer.Transaction_Type);
        }
        private void RefreshHoldList()
        {
            


            LIST_Customers_On_Hold.Clear();
            SqlConnection con = new SqlConnection(connection_string);
            con.Open();
            string query = "select * from Main_Queue where Queue_Status = @param1";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@param1", "Hold");
            SqlDataReader rdr;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                _Main_Queue a = new _Main_Queue
                {
                    Customer_Queue_Number = (string)rdr["Customer_Queue_Number"],
                    Full_Name = (string)rdr["Full_Name"],
                    ID = (int)rdr["ID"],
                    Pattern_Current = (int)rdr["Pattern_Current"],
                    Pattern_Max = (int)rdr["Pattern_Max"],
                    Queue_Number = (int)rdr["Queue_Number"],
                    Queue_Status = (string)rdr["Queue_Status"],
                    Servicing_Office = (int)rdr["Servicing_Office"],
                    Student_No = (string)rdr["Student_No"],
                    Time = (DateTime)rdr["Time"],
                    Transaction_Type = (int)rdr["Transaction_Type"],
                    Type = ((Boolean)rdr["Type"] == false) ? "Student" : "Guest",
                    Customer_From = ((Boolean)rdr["Customer_From"] == false) ? "Local" : "Mobile",
                };
                LIST_Customers_On_Hold.Add(a);
            }
            con.Close();
        }
        //private void setThisServicingOfficeName()
        //{
        //    foreach (DataRow row in table_Servicing_Office.Rows)
        //    {
        //        int temp_id = (int)row["ID"];
        //        if (temp_id == PROGRAM_Servicing_Office)
        //        {
        //            PROGRAM_Servicing_Office_Name = (string)row["Name"];
        //            break;
        //        }
        //    }
        //}
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
        private void Mode_Set()
        {

        }
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
        private int getQueueCurrentNumber(int What_Servicing_Office, SqlConnection con)
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
            string QUERY_retrieve_MainQueue_amount = "select COUNT(id) from Main_Queue where Servicing_Office = @param_so and Queue_Status = @param_qs";
            internal_Command1 = new SqlCommand(QUERY_retrieve_MainQueue_amount, con);
            internal_Command1.Parameters.AddWithValue("@param_qs", "Waiting");
            internal_Command1.Parameters.AddWithValue("@param_so", What_Servicing_Office);
            x = (int)internal_Command1.ExecuteScalar();
            return x;

        }
        
        private void Mode_Check(SqlConnection con,int retrieved_counter) {
            int min = 0, max = 0, current_queue_amount = 0, count_walkin = 0;
            string mode_name = "PROGRAM_MODE_NOT_FOUND_ON_DB";
            int mode_number = 0;

            current_queue_amount = getCurrentQueueAmount(PROGRAM_Servicing_Office,con);
            string query = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
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
            retrieved_counter++;
            Console.WriteLine("if {0} < {1}", retrieved_counter, count_walkin);
            if (retrieved_counter < count_walkin)
            {
                
                // increment, continue accepting walk-ins
                //Console.Write(" \n ** Mode_Check() -> Incrementing modeCounter ** \n ");
                query = "update Queue_Info set Counter = Counter + 1, Mode = @param_m where Servicing_Office = @param_so";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                cmd.Parameters.AddWithValue("@param_m", mode_number);
                cmd.ExecuteNonQuery();
                z("->?? Next one is walk-ins");
                //PROGRAM_modeCounter++;
            }
            else
            {
                //Console.Write(" \n * Mode_Check() -> Resetting modeCounter ** \n ");
                //PROGRAM_modeCounter = 0;
                query = "update Queue_Info set Counter = 0, Mode = @param_m where Servicing_Office = @param_so";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                cmd.Parameters.AddWithValue("@param_m", mode_number);
                cmd.ExecuteNonQuery();
                // reset the counter, add a code (on Next()) to pick a mobile user
                PROGRAM_ServeMobileCustomer = true;
                z("->?? Next one is online");
            }
            //return x;

            
        }
        /**
             * Disabled when removed currentCustomer (April 24, 2018)
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
        **/
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
            frmSettings.StartPosition = FormStartPosition.CenterScreen;
            frmSettings.ShowDialog();
        }
        private void onMouseClick(object sender, EventArgs e)
        {
            if (((Button)sender) == pictureBoxNEXT)
            {
                // If next button is clicked
                Console.WriteLine(" START ------------------- NEXT --------------------");
                Next();
                Console.WriteLine(" END ------------------- NEXT --------------------");
            }
            else if (((Button)sender) == pictureBoxDELETE)
            {
                // If delete button is clicked
                // Delete the customer
                Console.WriteLine(" START ------------------- DELETE --------------------");
                var confirmResult = MessageBox.Show("Are you sure to delete this queue?",
                                     "Delete?",
                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    // If 'Yes', do something here.
                    SqlConnection con = new SqlConnection(connection_string);
                    con.Open();
                    if (Next_Customer.ID > 0)
                    {
                        //Remove the customer from the Table:Main_Queue
                        SqlCommand deleteCommand;
                        String QUERY_delete_MainQueue_onNext = "delete from Main_Queue where id = @param_unique_id";
                        deleteCommand = new SqlCommand(QUERY_delete_MainQueue_onNext, con);
                        deleteCommand.Parameters.AddWithValue("@param_unique_id", Next_Customer.ID);
                        Console.WriteLine("Deleting from Table: Main_Queue with id of " + Next_Customer.ID);
                        Console.WriteLine("Customer id#" + Next_Customer.ID + " deleted!");
                        deleteCommand.ExecuteNonQuery();
                        Console.WriteLine("Real customer is deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Previous customer is a null.");
                        Console.WriteLine("NULL Customer is ignored. Nothing is deleted");
                    }
                    Next_Customer = No_Customer;
                    Previous_Customer = No_Customer;
                    con.Close();
                }
                else
                {
                    // If 'No', do something here.
                }
                Console.WriteLine(" END ------------------- DELETE --------------------");
            }
            else if (((Button)sender) == pictureBoxHOLD)
            {

                Console.WriteLine(" START ------------------- HOLD --------------------");
                HoldThisCustomer();
                Console.WriteLine(" END ------------------- HOLD --------------------");
            }
            else if (((Button)sender) == pictureBoxViewHold)
            {

                Console.WriteLine(" START ------------------- HOLD-FORM --------------------");
                RefreshHoldList();
                frmHoldCustomers = new FormHoldCustomers(LIST_Customers_On_Hold);
                frmHoldCustomers.StartPosition = FormStartPosition.CenterScreen;
                frmHoldCustomers.ShowDialog();
                Console.WriteLine(" END ------------------- HOLD-FORM --------------------");
            }
            else
            {
                Console.WriteLine("onMouseClick does not know what picturebox called it.");
                // do nothing
            }
        }

        private void onHover(object sender, EventArgs e)
        {
            if(((Button)sender) == pictureBoxNEXT)
            {
                ((Button)sender).Image = ServicingTerminalApplication.Properties.Resources.nextBtn_pressed;
            }
            else
            {
                ((Button)sender).Image = ServicingTerminalApplication.Properties.Resources.deleteBtn_pressed;
            }
        }

        private void onMouseLeave(object sender, EventArgs e)
        {
            if (((Button)sender) == pictureBoxNEXT)
            {
                ((Button)sender).Image = ServicingTerminalApplication.Properties.Resources.nextBtn;
            }
            else
            {
                ((Button)sender).Image = ServicingTerminalApplication.Properties.Resources.deleteBtn;
            }
        }
        #endregion

        #region MAIN METHODS
        private void HoldThisCustomer()
        {
            if (Next_Customer.ID > 0)
            {
                string q = "Hold_Customer is " + Hold_Customer.Customer_Queue_Number + Environment.NewLine
                    + "Next_Customer is " + Next_Customer.Customer_Queue_Number + Environment.NewLine
                    + "Previous_Customer is " + Previous_Customer.Customer_Queue_Number + Environment.NewLine;
                Console.WriteLine(q);
                Hold_Customer = Next_Customer;
                Next_Customer = No_Customer;
                Previous_Customer = No_Customer;
                updateCustomerInfoShown(Clear_Customer);
                SqlConnection con = new SqlConnection(connection_string);
                string query = "update Main_Queue set Queue_Status = @param1 OUTPUT Inserted.id where Customer_Queue_Number = @paramcqn ";
               
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@param1", "Hold");
                cmd.Parameters.AddWithValue("@paramcqn", Hold_Customer.Customer_Queue_Number);
                Console.WriteLine(""+cmd.ExecuteScalar().ToString());

                
                RefreshHoldList();
                

                con.Close();

                Console.WriteLine("update Main_Queue set Queue_Status = Hold where id = {0}", Hold_Customer.Customer_Queue_Number);
                MessageBox.Show(Hold_Customer.Full_Name+" is now on hold.");
            }
            else
            {
                Console.WriteLine("Customer currently serving is NULL.");
            }
            Console.WriteLine(w_temp_run);
            w_temp_run = string.Empty;
        }
        public void Next()
        {
            SqlConnection con = new SqlConnection(connection_string);
            using (con)
            {
                con.Open();
                // Called when next button is clicked
                // declare variables to be used
                String QUERY_next_customer_mq = "select TOP 1 Pattern_Current, id, Queue_Number, Full_Name, Servicing_Office, Transaction_Type, Type, Customer_Queue_Number, Customer_From, Student_No from Main_Queue " +
                    "where Servicing_Office = @param_so and Queue_Status = @param_qs and Customer_From = @param_cf order by Queue_Number asc";
                String QUERY_next_customer_online = "select TOP 1 Pattern_Current, id, Queue_Number, Full_Name, Servicing_Office, Transaction_Type, Type, Customer_Queue_Number, Customer_From, Student_No from Main_Queue " +
                    "where Servicing_Office = @param_so and Queue_Status = @param_qs and Customer_From = @param_cf order by Queue_Number asc";
                String QUERY_mq_next_customer_update_on_success = "update Main_Queue set Queue_Status = @param_qs where id = @param_uniqueid";
                //String QUERY_tq_next_customer_update_on_success = "update Transfer_Queue set Queue_Status = @param_qs where Main_Queue_ID = @param_uniqueid";
                SqlCommand Command1,Command2,Command3,CommandQuickUpdate;
                SqlDataReader Reader1,Reader2,Reader3;
                Command1 = new SqlCommand();
                Command1.Connection = con;
                Command1.CommandType = CommandType.Text;

                bool _READ_COUNTER = false;



                if (PROGRAM_ServeMobileCustomer)
                {
                    Command1.CommandText = QUERY_next_customer_online;
                    Command1.Parameters.AddWithValue("@param_cf", 1);
                }
                else
                {
                    Command1.CommandText = QUERY_next_customer_mq;
                    Command1.Parameters.AddWithValue("@param_cf", 0);
                }

                Command1.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                Command1.Parameters.AddWithValue("@param_qs", "Waiting");
                PROGRAM_ServeMobileCustomer = false;
                
                
                Reader1 = Command1.ExecuteReader();

                while (Reader1.Read())
                {
                    _READ_COUNTER = true;
                    z("Customer found on Local (Normal)");
                    Console.WriteLine("?->" + Reader1["Customer_From"]);
                    Next_Customer = new _Main_Queue
                    {
                        Queue_Number = (int)Reader1["Queue_Number"],
                        Full_Name = (string)Reader1["Full_Name"],
                        Servicing_Office = (int)Reader1["Servicing_Office"],
                        Transaction_Type = (int)Reader1["Transaction_Type"],
                        Type = ((Boolean)Reader1["Type"] == false) ? "Student" : "Guest",
                        Customer_From = ((Boolean)Reader1["Customer_From"] == false) ? "Local" : "Mobile",
                        Customer_Queue_Number = (string)Reader1["Customer_Queue_Number"],
                        ID = (int)Reader1["id"],
                        Student_No = ((Boolean)Reader1["Type"] == false) ? (string)Reader1["Student_No"] : "N/A",
                        Pattern_Current = (int)Reader1["Pattern_Current"],
                        Transfer_Customer = false
                    };
                    CommandQuickUpdate = new SqlCommand(QUERY_mq_next_customer_update_on_success, con);
                    CommandQuickUpdate.Parameters.AddWithValue("@param_qs", "Serving");
                    CommandQuickUpdate.Parameters.AddWithValue("@param_uniqueid", (int)Reader1["id"]);
                    CommandQuickUpdate.ExecuteNonQuery();
                    Console.WriteLine("Customer is now " + Next_Customer.Customer_Queue_Number);

                }
                Console.WriteLine("////////////////////{0} {1}", _READ_COUNTER, PROGRAM_ServeMobileCustomer);
                if (!_READ_COUNTER && PROGRAM_ServeMobileCustomer)
                {
                    // If it did not received mobile customers because there were none on the Main_Queue,
                    // pick one from Local
                    MessageBox.Show("No more mobile: getting locals instead.");
                    Command2 = new SqlCommand(QUERY_next_customer_mq, con);
                    Command2.Parameters.AddWithValue("@param_cf", 0); // 0 = Local
                    Command2.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                    Command2.Parameters.AddWithValue("@param_qs", "Waiting");
                    Reader2 = Command2.ExecuteReader();
                    while (Reader2.Read())
                    {
                        _READ_COUNTER = true;
                        z("Customer found on Local (no more mobile)");
                        Console.WriteLine("?->" + Reader2["Customer_From"]);
                        Next_Customer = new _Main_Queue
                        {
                            Queue_Number = (int)Reader2["Queue_Number"],
                            Full_Name = (string)Reader2["Full_Name"],
                            Servicing_Office = (int)Reader2["Servicing_Office"],
                            Transaction_Type = (int)Reader2["Transaction_Type"],
                            Type = ((Boolean)Reader2["Type"] == false) ? "Student" : "Guest",
                            Customer_From = ((Boolean)Reader2["Customer_From"] == false) ? "Local" : "Mobile",
                            Customer_Queue_Number = (string)Reader2["Customer_Queue_Number"],
                            ID = (int)Reader2["id"],
                            Student_No = ((Boolean)Reader2["Type"] == false) ? (string)Reader2["Student_No"] : "N/A",
                            Pattern_Current = (int)Reader2["Pattern_Current"],
                            Transfer_Customer = false
                        };
                        CommandQuickUpdate = new SqlCommand(QUERY_mq_next_customer_update_on_success, con);
                        CommandQuickUpdate.Parameters.AddWithValue("@param_qs", "Serving");
                        CommandQuickUpdate.Parameters.AddWithValue("@param_uniqueid", (int)Reader2["id"]);
                        CommandQuickUpdate.ExecuteNonQuery();
                        Console.WriteLine("Customer is now " + Next_Customer.Customer_Queue_Number);

                    }
                    Command2.Dispose();
                }
                else if (!_READ_COUNTER && !PROGRAM_ServeMobileCustomer)
                {
                    // If it did not received local customers because there were none on the Main_Queue,
                    // pick one from Mobile
                    Command3 = new SqlCommand(QUERY_next_customer_online, con);
                    Command3.Parameters.AddWithValue("@param_cf", 1);
                    Command3.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                    Command3.Parameters.AddWithValue("@param_qs", "Waiting");
                    Reader3 = Command3.ExecuteReader();
                    while (Reader3.Read())
                    {
                        _READ_COUNTER = true;
                        z("Customer found on Mobile (no more local)");
                        Console.WriteLine("?->" + Reader3["Customer_From"]);
                        Next_Customer = new _Main_Queue
                        {
                            Queue_Number = (int)Reader3["Queue_Number"],
                            Full_Name = (string)Reader3["Full_Name"],
                            Servicing_Office = (int)Reader3["Servicing_Office"],
                            Transaction_Type = (int)Reader3["Transaction_Type"],
                            Type = ((Boolean)Reader3["Type"] == false) ? "Student" : "Guest",
                            Customer_From = ((Boolean)Reader3["Customer_From"] == false) ? "Local" : "Mobile",
                            Customer_Queue_Number = (string)Reader3["Customer_Queue_Number"],
                            ID = (int)Reader3["id"],
                            Student_No = ((Boolean)Reader3["Type"] == false) ? (string)Reader3["Student_No"] : "N/A",
                            Pattern_Current = (int)Reader3["Pattern_Current"],
                            Transfer_Customer = false
                        };
                        CommandQuickUpdate = new SqlCommand(QUERY_mq_next_customer_update_on_success, con);
                        CommandQuickUpdate.Parameters.AddWithValue("@param_qs", "Serving");
                        CommandQuickUpdate.Parameters.AddWithValue("@param_uniqueid", (int)Reader3["id"]);
                        CommandQuickUpdate.ExecuteNonQuery();
                        Console.WriteLine("Customer is now " + Next_Customer.Customer_Queue_Number);

                    }
                    Command3.Dispose();
                }
                //if (!_READ_COUNTER)
                //{
                //    Command2 = new SqlCommand(QUERY_next_customer_mobile, con);
                //    Command2.Parameters.AddWithValue("@param_so", PROGRAM_Servicing_Office);
                //    Command2.Parameters.AddWithValue("@param_qs", "Waiting");
                //    Reader2 = Command2.ExecuteReader();

                //    while (Reader2.Read())
                //    {
                //        z("read found on transfer queue");
                //        _READ_COUNTER = true;
                //        Next_Customer = new _Main_Queue
                //        {
                //            Queue_Number = (int)Reader2["Queue_Number"],
                //            Full_Name = (string)Reader2["Full_Name"],
                //            Servicing_Office = (int)Reader2["Servicing_Office"],
                //            Transaction_Type = (int)Reader2["Transaction_Type"],
                //            Type = ((Boolean)Reader2["Type"] == false) ? "Student" : "Guest",
                //            Customer_From = ((Boolean)Reader2["Customer_From"] == false) ? "Local" : "Mobile",
                //            Customer_Queue_Number = (string)Reader2["Customer_Queue_Number"],
                //            ID = (int)Reader2["Main_Queue_ID"],
                //            Student_No = ((Boolean)Reader1["Type"] == false) ? (string)Reader1["Student_No"] : "",
                //            Pattern_Current = (int)Reader2["Pattern_Current"],
                //            Transfer_Customer = true
                //        };
                //        CommandQuickUpdate = new SqlCommand(QUERY_tq_next_customer_update_on_success, con);
                //        CommandQuickUpdate.Parameters.AddWithValue("@param_qs", "Serving");
                //        CommandQuickUpdate.Parameters.AddWithValue("@param_uniqueid", (int)Reader2["id"]);
                //        CommandQuickUpdate.ExecuteNonQuery();
                //    }
                //    if (_READ_COUNTER) MessageBox.Show("Customer found on Transfer Table.");
                //}
                if (!_READ_COUNTER) Console.WriteLine("No customers found on queue.");

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
                if (!_READ_COUNTER) {
                    //Check first if the Servicing App is holding a real customer to move him to the next one or not.
                    MoveCustomerToNextOrDelete(con);
                    Previous_Customer = No_Customer;
                    Next_Customer = No_Customer;
                    updateCustomerInfoShown(Clear_Customer);
                    z("Empty Customers.");
                }
                else
                {
                    // Customer found.

                    // Show the information of the customer.
                    updateCustomerInfoShown(Next_Customer);
                    /**
                    * Disabled when removed currentCustomer (April 24, 2018)
                    fnf.OnFirstNameUpdated(nuea);
                    **/
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
                    String QUERY_update_QueueInfo = "update Queue_Info set Current_Number = Current_Number+1, Window = @param_window, Customer_Queue_Number = @param_cqn where Servicing_Office = @param_so";
                    Command4 = new SqlCommand(QUERY_update_QueueInfo, con);
                    
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

                    
                }
                



            }
            Console.WriteLine(w_temp_run);
            w_temp_run = string.Empty;
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
                    if (Previous_Customer.Type == "Student")
                        try
                        {
                            firebase_Connection fcon = new firebase_Connection();
                            fcon.User_SetToInactive(Previous_Customer.Student_No);
                        }
                        catch (FirebaseException) { }
                    z("Real customer is deleted.");
                }
                else { z("Previous customer is a null."); z("NULL Customer is ignored. Nothing is deleted"); }
            }
            else
            {
                //Transfer the Customer to the next Servicing Office
                //Change the values of the served Customer at Main_Queue
                SqlCommand Command6,Command7;
                //Next line -> Update Table: Main Queue of Previous Customer
                //Sending #customer to the next Servicing Office
                String QUERY_update_MainQueue_onNext = "update Main_Queue set Queue_Number = @param_qn, Servicing_Office = @param_so, Pattern_Current = Pattern_Current + 1, Queue_Status = @param_qs where id = @param_unique_id";
                Command6 = new SqlCommand(QUERY_update_MainQueue_onNext, con);
                String QUERY_create_RatingOffice_onNext = "insert into Rating_Office (Customer_Queue_Number,isStudent,Score,isGiven,Transaction_ID,Servicing_Office) " +
                    " values " +
                    " (@param_CQN,@param_isStudent,0,0,@param_tt_ID,@param_so)";
                Command7 = new SqlCommand(QUERY_create_RatingOffice_onNext);
                //Get the Queue_Number from the tip of Table:Queue_Info -> Current_Queue
                int TEMP_getCustomerQueueNumberOnNext = getQueueCurrentQueue(_NEXT_Servicing_Office, con);
                //Declare parameters
                Command6.Parameters.AddWithValue("@param_qn", TEMP_getCustomerQueueNumberOnNext);
                Command6.Parameters.AddWithValue("@param_so", _NEXT_Servicing_Office);
                Command6.Parameters.AddWithValue("@param_unique_id", Previous_Customer.ID);
                Command6.Parameters.AddWithValue("@param_qs", "Waiting");
                z("The update Main_Queue for moving a customer run with the following values:");
                Command7.Parameters.AddWithValue("@param_CQN", Previous_Customer.Customer_Queue_Number);
                Command7.Parameters.AddWithValue("@param_isStudent", (Previous_Customer.Type == "Guest") ? false : true);
                Command7.Parameters.AddWithValue("@param_tt_ID", Previous_Customer.Transaction_Type);
                Command7.Parameters.AddWithValue("@param_so", _NEXT_Servicing_Office);
                z("Queue Number : " + TEMP_getCustomerQueueNumberOnNext);
                z("Servicing Office : " + _NEXT_Servicing_Office);
                z("Unique id of : " + Previous_Customer.ID);
                z("Pattern Number is :" + Previous_Customer.Pattern_Current);
                //Execute Commands
                Command6.ExecuteNonQuery();
                Command7.ExecuteNonQuery();
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
