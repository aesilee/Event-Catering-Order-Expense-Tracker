namespace Event_Catering_Order___Expense_Tracker
{
    partial class EventsInDate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventsInDate));
            this.EventsFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.BackBtn = new System.Windows.Forms.PictureBox();
            this.DateLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.BackBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // EventsFlowPanel
            // 
            this.EventsFlowPanel.AutoScroll = true;
            this.EventsFlowPanel.Location = new System.Drawing.Point(8, 56);
            this.EventsFlowPanel.Name = "EventsFlowPanel";
            this.EventsFlowPanel.Size = new System.Drawing.Size(368, 296);
            this.EventsFlowPanel.TabIndex = 88;
            // 
            // BackBtn
            // 
            this.BackBtn.Image = global::Event_Catering_Order___Expense_Tracker.Properties.Resources.Eventra__2__removebg_preview;
            this.BackBtn.Location = new System.Drawing.Point(8, 8);
            this.BackBtn.Name = "BackBtn";
            this.BackBtn.Size = new System.Drawing.Size(21, 22);
            this.BackBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BackBtn.TabIndex = 87;
            this.BackBtn.TabStop = false;
            this.BackBtn.Click += new System.EventHandler(this.BackBtn_Click);
            // 
            // DateLbl
            // 
            this.DateLbl.AutoSize = true;
            this.DateLbl.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DateLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(70)))), ((int)(((byte)(56)))));
            this.DateLbl.Location = new System.Drawing.Point(64, 16);
            this.DateLbl.Name = "DateLbl";
            this.DateLbl.Size = new System.Drawing.Size(198, 28);
            this.DateLbl.TabIndex = 86;
            this.DateLbl.Text = "Events for [Date]";
            // 
            // EventsInDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(234)))), ((int)(((byte)(218)))));
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.EventsFlowPanel);
            this.Controls.Add(this.BackBtn);
            this.Controls.Add(this.DateLbl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EventsInDate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Events";
            this.Load += new System.EventHandler(this.EventsInDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BackBtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel EventsFlowPanel;
        private System.Windows.Forms.PictureBox BackBtn;
        private System.Windows.Forms.Label DateLbl;
    }
}