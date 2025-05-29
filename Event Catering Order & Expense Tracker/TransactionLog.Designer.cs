namespace Event_Catering_Order___Expense_Tracker
{
    partial class TransactionLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransactionLog));
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BackBtn = new System.Windows.Forms.PictureBox();
            this.TransactionLogLbl = new System.Windows.Forms.Label();
            this.EventDescriptionLbl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BackBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(71)))), ((int)(((byte)(56)))));
            this.label3.Location = new System.Drawing.Point(96, 24);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 33);
            this.label3.TabIndex = 12;
            this.label3.Text = "Expenses";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(163)))), ((int)(((byte)(150)))));
            this.panel1.Controls.Add(this.TransactionLogLbl);
            this.panel1.Controls.Add(this.BackBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(329, 56);
            this.panel1.TabIndex = 13;
            // 
            // BackBtn
            // 
            this.BackBtn.Image = ((System.Drawing.Image)(resources.GetObject("BackBtn.Image")));
            this.BackBtn.Location = new System.Drawing.Point(8, 8);
            this.BackBtn.Name = "BackBtn";
            this.BackBtn.Size = new System.Drawing.Size(24, 24);
            this.BackBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BackBtn.TabIndex = 70;
            this.BackBtn.TabStop = false;
            // 
            // TransactionLogLbl
            // 
            this.TransactionLogLbl.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TransactionLogLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(57)))), ((int)(((byte)(49)))));
            this.TransactionLogLbl.Location = new System.Drawing.Point(58, 24);
            this.TransactionLogLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TransactionLogLbl.Name = "TransactionLogLbl";
            this.TransactionLogLbl.Size = new System.Drawing.Size(203, 28);
            this.TransactionLogLbl.TabIndex = 71;
            this.TransactionLogLbl.Text = "Transaction Log";
            this.TransactionLogLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EventDescriptionLbl
            // 
            this.EventDescriptionLbl.AutoSize = true;
            this.EventDescriptionLbl.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EventDescriptionLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(71)))), ((int)(((byte)(56)))));
            this.EventDescriptionLbl.Location = new System.Drawing.Point(12, 88);
            this.EventDescriptionLbl.Name = "EventDescriptionLbl";
            this.EventDescriptionLbl.Size = new System.Drawing.Size(138, 19);
            this.EventDescriptionLbl.TabIndex = 49;
            this.EventDescriptionLbl.Text = "EventDescriptionLbl";
            // 
            // TransactionLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(234)))), ((int)(((byte)(218)))));
            this.ClientSize = new System.Drawing.Size(329, 581);
            this.Controls.Add(this.EventDescriptionLbl);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Name = "TransactionLog";
            this.Text = "TransactionLog";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BackBtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox BackBtn;
        private System.Windows.Forms.Label TransactionLogLbl;
        private System.Windows.Forms.Label EventDescriptionLbl;
    }
}