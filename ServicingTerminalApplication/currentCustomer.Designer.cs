namespace ServicingTerminalApplication
{
    partial class currentCustomer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.textCQN = new System.Windows.Forms.Label();
            this.textID = new System.Windows.Forms.TextBox();
            this.textName = new System.Windows.Forms.TextBox();
            this.textTransaction = new System.Windows.Forms.TextBox();
            this.textType = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel1.Controls.Add(this.textCQN);
            this.panel1.Location = new System.Drawing.Point(16, 15);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 167);
            this.panel1.TabIndex = 0;
            // 
            // textCQN
            // 
            this.textCQN.Font = new System.Drawing.Font("Bernard MT Condensed", 35.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textCQN.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.textCQN.Location = new System.Drawing.Point(17, 20);
            this.textCQN.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.textCQN.Name = "textCQN";
            this.textCQN.Size = new System.Drawing.Size(223, 132);
            this.textCQN.TabIndex = 5;
            this.textCQN.Text = "R-COG8";
            this.textCQN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textID
            // 
            this.textID.Cursor = System.Windows.Forms.Cursors.Default;
            this.textID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textID.Location = new System.Drawing.Point(283, 16);
            this.textID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textID.Name = "textID";
            this.textID.ReadOnly = true;
            this.textID.ShortcutsEnabled = false;
            this.textID.Size = new System.Drawing.Size(377, 29);
            this.textID.TabIndex = 1;
            this.textID.TabStop = false;
            this.textID.Text = "ID # 2013-99665";
            // 
            // textName
            // 
            this.textName.Cursor = System.Windows.Forms.Cursors.Default;
            this.textName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textName.Location = new System.Drawing.Point(281, 53);
            this.textName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textName.Name = "textName";
            this.textName.ReadOnly = true;
            this.textName.Size = new System.Drawing.Size(377, 29);
            this.textName.TabIndex = 2;
            this.textName.TabStop = false;
            this.textName.Text = "Cipriano Kyle John A. Soriben III";
            // 
            // textTransaction
            // 
            this.textTransaction.Cursor = System.Windows.Forms.Cursors.Default;
            this.textTransaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textTransaction.Location = new System.Drawing.Point(283, 90);
            this.textTransaction.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textTransaction.Multiline = true;
            this.textTransaction.Name = "textTransaction";
            this.textTransaction.ReadOnly = true;
            this.textTransaction.Size = new System.Drawing.Size(377, 54);
            this.textTransaction.TabIndex = 3;
            this.textTransaction.TabStop = false;
            this.textTransaction.Text = "[Registrar] Transcript of Records";
            // 
            // textType
            // 
            this.textType.Cursor = System.Windows.Forms.Cursors.Default;
            this.textType.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textType.Location = new System.Drawing.Point(281, 153);
            this.textType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textType.Name = "textType";
            this.textType.ReadOnly = true;
            this.textType.Size = new System.Drawing.Size(377, 29);
            this.textType.TabIndex = 4;
            this.textType.TabStop = false;
            this.textType.Text = "Student";
            // 
            // currentCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(677, 191);
            this.Controls.Add(this.textType);
            this.Controls.Add(this.textTransaction);
            this.Controls.Add(this.textName);
            this.Controls.Add(this.textID);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "currentCustomer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "currentCustomer";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label textCQN;
        private System.Windows.Forms.TextBox textID;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.TextBox textTransaction;
        private System.Windows.Forms.TextBox textType;
    }
}