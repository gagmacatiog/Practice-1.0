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
    public partial class addTransactionType : Form
    {
        public addTransactionType()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 9;
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

                var dataSource = new List<servicingOffice>();
                dataSource.Add(new servicingOffice() { Office = "Cashier", Value = "Cashier" });
                dataSource.Add(new servicingOffice() { Office = "Registrar", Value = "Registrar" });
                dataSource.Add(new servicingOffice() { Office = "Student Accounts", Value = "Student Accounts" });
                dataSource.Add(new servicingOffice() { Office = "Book keeper", Value = "Book keeper" });

                cBox.DataSource = dataSource;
                cBox.DisplayMember = "Office";
                cBox.ValueMember = "Value";
                cBox.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);

                cBox.DropDownStyle = ComboBoxStyle.DropDownList;

                p.Controls.Add(label);
                p.Controls.Add(cBox);
                p.Invalidate();

                flowLayoutPanel1.Controls.Add(p);
                flowLayoutPanel1.Controls.SetChildIndex(p, 0);  // this moves the new one to the top!
                                                                // this is just for fun:
                                                                

                flowLayoutPanel1.Invalidate();
            }
            

        }

        public class servicingOffice
        {
            public String Office { get; set; }
            public String Value { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Reset Button
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Height = 0;
            addTransactionType.ActiveForm.Height = 171;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Finish Button
        }
    }
}
