using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicingTerminalApplication
{


    public partial class currentCustomer : Form
    {
        public event EventHandler<updateForm> FirstNameUpdated;
        public virtual void OnFirstNameUpdated(updateForm e)
        {
            if (FirstNameUpdated != null)
                FirstNameUpdated(this, e);
        }

        public currentCustomer()
        {
            InitializeComponent();

        }
        public string setID
        {
            get { return this.textID.Text; }
            set
            {
                this.textID.Text = value;
                this.Refresh();
            }
        }
        public string setName
        {
            get { return this.textName.Text; }
            set
            {
                this.textName.Text = value;
                this.Refresh();
            }
        }
        public string setTransaction
        {
            get { return this.textTransaction.Text; }
            set
            {
                this.textTransaction.Text = value;
                this.Refresh();
            }
        }
        public string setCQN
        {
            get { return this.textCQN.Text; }
            set
            {
                this.textCQN.Text = value;
                this.Refresh();
            }
        }
        public string setType
        {
            get { return this.textType.Text; }
            set
            {
                this.textType.Text = value;
                this.Refresh();
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
    public class updateForm : EventArgs
    {
        public string CQN { get; set; }
        public string type { get; set; }
        public string s_id { get; set; }
        public string full_name { get; set; }
        public string transaction_type { get; set; }
    }
}

