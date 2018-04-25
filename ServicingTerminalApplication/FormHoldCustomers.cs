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
    public partial class FormHoldCustomers : Form
    {
        List<_Main_Queue> LIST_Customers_On_Hold = new List<_Main_Queue>();
        Form1 mainForm = (Form1)Application.OpenForms["Form1"];
        private String connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;

        public FormHoldCustomers(List<_Main_Queue> new_List)
        {
            InitializeComponent();
            LIST_Customers_On_Hold = new_List;
            listBox1.DataSource = LIST_Customers_On_Hold;
            listBox1.DisplayMember = "Full_Name";
            listBox1.ValueMember = "ID";
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            _Main_Queue selected = (_Main_Queue)listBox1.SelectedItem;
            if (selected != null)
            {
                mainForm.Next_Customer = selected;
                //  Change Hold to Waiting
                SqlConnection con = new SqlConnection(connection_string);
                string query = "update Main_Queue set Queue_Status = @param1 where Customer_Queue_Number = @paramcqn";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@param1", "Waiting");
                cmd.Parameters.AddWithValue("@paramcqn", selected.Customer_Queue_Number);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                mainForm.updateCustomerInfoShown(selected);
                mainForm.Next();
                Console.WriteLine("Now serving customer " + selected.Full_Name);
                Close();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
