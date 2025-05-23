namespace Event_Catering_Order___Expense_Tracker
{
    partial class Calendar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Calendar));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.monthLbl = new System.Windows.Forms.Label();
            this.prevBtn = new System.Windows.Forms.PictureBox();
            this.nextbtn = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.prevBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextbtn)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(191, 125);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(672, 436);
            this.flowLayoutPanel1.TabIndex = 66;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(71)))), ((int)(((byte)(56)))));
            this.label1.Location = new System.Drawing.Point(191, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 19);
            this.label1.TabIndex = 67;
            // 
            // monthLbl
            // 
            this.monthLbl.AutoSize = true;
            this.monthLbl.Font = new System.Drawing.Font("Calibri Light", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(71)))), ((int)(((byte)(56)))));
            this.monthLbl.Location = new System.Drawing.Point(200, 95);
            this.monthLbl.Name = "monthLbl";
            this.monthLbl.Size = new System.Drawing.Size(0, 27);
            this.monthLbl.TabIndex = 68;
            // 
            // prevBtn
            // 
            this.prevBtn.Image = global::Event_Catering_Order___Expense_Tracker.Properties.Resources.Untitled_design__6__removebg_preview;
            this.prevBtn.Location = new System.Drawing.Point(807, 95);
            this.prevBtn.Name = "prevBtn";
            this.prevBtn.Size = new System.Drawing.Size(24, 24);
            this.prevBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.prevBtn.TabIndex = 69;
            this.prevBtn.TabStop = false;
            this.prevBtn.Click += new System.EventHandler(this.prevBtn_Click);
            // 
            // nextbtn
            // 
            this.nextbtn.Image = global::Event_Catering_Order___Expense_Tracker.Properties.Resources.Untitled_design__5__removebg_preview;
            this.nextbtn.Location = new System.Drawing.Point(837, 95);
            this.nextbtn.Name = "nextbtn";
            this.nextbtn.Size = new System.Drawing.Size(24, 24);
            this.nextbtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.nextbtn.TabIndex = 70;
            this.nextbtn.TabStop = false;
            this.nextbtn.Click += new System.EventHandler(this.nextbtn_Click);
            // 
            // Calendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(234)))), ((int)(((byte)(218)))));
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.nextbtn);
            this.Controls.Add(this.prevBtn);
            this.Controls.Add(this.monthLbl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Calendar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calendar";
            this.Load += new System.EventHandler(this.Calendar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.prevBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextbtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label monthLbl;
        private System.Windows.Forms.PictureBox prevBtn;
        private System.Windows.Forms.PictureBox nextbtn;
    }
}