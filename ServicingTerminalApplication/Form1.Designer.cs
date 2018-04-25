namespace ServicingTerminalApplication
{
    partial class Form1
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
            this.pictureBoxHOLD = new System.Windows.Forms.PictureBox();
            this.pictureBoxDELETE = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.info_background = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textCQN = new System.Windows.Forms.Label();
            this.textType = new System.Windows.Forms.TextBox();
            this.textTransaction = new System.Windows.Forms.TextBox();
            this.textName = new System.Windows.Forms.TextBox();
            this.textID = new System.Windows.Forms.TextBox();
            this.pictureBoxNEXT = new System.Windows.Forms.PictureBox();
            this.pictureBoxViewHold = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHOLD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDELETE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.info_background)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNEXT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxViewHold)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxHOLD
            // 
            this.pictureBoxHOLD.BackColor = System.Drawing.Color.DodgerBlue;
            this.pictureBoxHOLD.Image = global::ServicingTerminalApplication.Properties.Resources.nextBtn;
            this.pictureBoxHOLD.Location = new System.Drawing.Point(279, 224);
            this.pictureBoxHOLD.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxHOLD.Name = "pictureBoxHOLD";
            this.pictureBoxHOLD.Size = new System.Drawing.Size(109, 75);
            this.pictureBoxHOLD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxHOLD.TabIndex = 0;
            this.pictureBoxHOLD.TabStop = false;
            this.pictureBoxHOLD.Click += new System.EventHandler(this.onMouseClick);
            this.pictureBoxHOLD.MouseLeave += new System.EventHandler(this.onMouseLeave);
            this.pictureBoxHOLD.MouseHover += new System.EventHandler(this.onHover);
            // 
            // pictureBoxDELETE
            // 
            this.pictureBoxDELETE.BackColor = System.Drawing.Color.DodgerBlue;
            this.pictureBoxDELETE.Image = global::ServicingTerminalApplication.Properties.Resources.deleteBtn;
            this.pictureBoxDELETE.Location = new System.Drawing.Point(145, 224);
            this.pictureBoxDELETE.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxDELETE.Name = "pictureBoxDELETE";
            this.pictureBoxDELETE.Size = new System.Drawing.Size(114, 75);
            this.pictureBoxDELETE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxDELETE.TabIndex = 1;
            this.pictureBoxDELETE.TabStop = false;
            this.pictureBoxDELETE.Click += new System.EventHandler(this.onMouseClick);
            this.pictureBoxDELETE.MouseLeave += new System.EventHandler(this.onMouseLeave);
            this.pictureBoxDELETE.MouseHover += new System.EventHandler(this.onHover);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.BackColor = System.Drawing.Color.Black;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(560, 224);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(177, 75);
            this.button1.TabIndex = 2;
            this.button1.Text = "Settings";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // info_background
            // 
            this.info_background.BackColor = System.Drawing.Color.DarkGray;
            this.info_background.Location = new System.Drawing.Point(12, 12);
            this.info_background.Name = "info_background";
            this.info_background.Size = new System.Drawing.Size(726, 205);
            this.info_background.TabIndex = 4;
            this.info_background.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel1.Controls.Add(this.textCQN);
            this.panel1.Location = new System.Drawing.Point(41, 24);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 167);
            this.panel1.TabIndex = 5;
            // 
            // textCQN
            // 
            this.textCQN.Font = new System.Drawing.Font("Bernard MT Condensed", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textCQN.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.textCQN.Location = new System.Drawing.Point(17, 20);
            this.textCQN.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.textCQN.Name = "textCQN";
            this.textCQN.Size = new System.Drawing.Size(223, 132);
            this.textCQN.TabIndex = 5;
            this.textCQN.Text = "-";
            this.textCQN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textType
            // 
            this.textType.Cursor = System.Windows.Forms.Cursors.Default;
            this.textType.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textType.Location = new System.Drawing.Point(306, 162);
            this.textType.Margin = new System.Windows.Forms.Padding(4);
            this.textType.Name = "textType";
            this.textType.ReadOnly = true;
            this.textType.Size = new System.Drawing.Size(377, 29);
            this.textType.TabIndex = 9;
            this.textType.TabStop = false;
            this.textType.Text = "-----";
            // 
            // textTransaction
            // 
            this.textTransaction.Cursor = System.Windows.Forms.Cursors.Default;
            this.textTransaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textTransaction.Location = new System.Drawing.Point(306, 99);
            this.textTransaction.Margin = new System.Windows.Forms.Padding(4);
            this.textTransaction.Multiline = true;
            this.textTransaction.Name = "textTransaction";
            this.textTransaction.ReadOnly = true;
            this.textTransaction.Size = new System.Drawing.Size(377, 54);
            this.textTransaction.TabIndex = 8;
            this.textTransaction.TabStop = false;
            this.textTransaction.Text = "-----";
            // 
            // textName
            // 
            this.textName.Cursor = System.Windows.Forms.Cursors.Default;
            this.textName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textName.Location = new System.Drawing.Point(306, 62);
            this.textName.Margin = new System.Windows.Forms.Padding(4);
            this.textName.Name = "textName";
            this.textName.ReadOnly = true;
            this.textName.Size = new System.Drawing.Size(377, 29);
            this.textName.TabIndex = 7;
            this.textName.TabStop = false;
            this.textName.Text = "Name";
            // 
            // textID
            // 
            this.textID.Cursor = System.Windows.Forms.Cursors.Default;
            this.textID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textID.Location = new System.Drawing.Point(306, 25);
            this.textID.Margin = new System.Windows.Forms.Padding(4);
            this.textID.Name = "textID";
            this.textID.ReadOnly = true;
            this.textID.ShortcutsEnabled = false;
            this.textID.Size = new System.Drawing.Size(377, 29);
            this.textID.TabIndex = 6;
            this.textID.TabStop = false;
            this.textID.Text = "ID # ";
            // 
            // pictureBoxNEXT
            // 
            this.pictureBoxNEXT.BackColor = System.Drawing.Color.Lime;
            this.pictureBoxNEXT.Image = global::ServicingTerminalApplication.Properties.Resources.nextBtn;
            this.pictureBoxNEXT.Location = new System.Drawing.Point(13, 224);
            this.pictureBoxNEXT.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxNEXT.Name = "pictureBoxNEXT";
            this.pictureBoxNEXT.Size = new System.Drawing.Size(111, 75);
            this.pictureBoxNEXT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxNEXT.TabIndex = 10;
            this.pictureBoxNEXT.TabStop = false;
            this.pictureBoxNEXT.Click += new System.EventHandler(this.onMouseClick);
            // 
            // pictureBoxViewHold
            // 
            this.pictureBoxViewHold.BackColor = System.Drawing.Color.Red;
            this.pictureBoxViewHold.Image = global::ServicingTerminalApplication.Properties.Resources.nextBtn;
            this.pictureBoxViewHold.Location = new System.Drawing.Point(412, 224);
            this.pictureBoxViewHold.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxViewHold.Name = "pictureBoxViewHold";
            this.pictureBoxViewHold.Size = new System.Drawing.Size(104, 75);
            this.pictureBoxViewHold.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxViewHold.TabIndex = 11;
            this.pictureBoxViewHold.TabStop = false;
            this.pictureBoxViewHold.Click += new System.EventHandler(this.onMouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.ClientSize = new System.Drawing.Size(750, 315);
            this.Controls.Add(this.pictureBoxViewHold);
            this.Controls.Add(this.pictureBoxNEXT);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textType);
            this.Controls.Add(this.textTransaction);
            this.Controls.Add(this.textName);
            this.Controls.Add(this.textID);
            this.Controls.Add(this.info_background);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBoxDELETE);
            this.Controls.Add(this.pictureBoxHOLD);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.DarkRed;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHOLD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDELETE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.info_background)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNEXT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxViewHold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxHOLD;
        private System.Windows.Forms.PictureBox pictureBoxDELETE;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox info_background;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label textCQN;
        private System.Windows.Forms.TextBox textType;
        private System.Windows.Forms.TextBox textTransaction;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.TextBox textID;
        private System.Windows.Forms.PictureBox pictureBoxNEXT;
        private System.Windows.Forms.PictureBox pictureBoxViewHold;
    }
}

