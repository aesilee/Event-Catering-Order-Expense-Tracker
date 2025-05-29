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

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class TransactionLog : Form
    {
        private SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");
        private int eventID;

        public TransactionLog(int eventID)
        {
            InitializeComponent();
            this.eventID = eventID;
            LoadTransactionLog();
        }

        private void LoadTransactionLog()
        {
            try
            {
                con.Open();
                string query = @"
                    SELECT 
                        e.EventTitle,
                        ex.InitialPaymentAmount,
                        ex.DatePayed,
                        ex.PaymentMethod,
                        ex.PaymentTerm,
                        ex.PaymentStatus
                    FROM EventTable e
                    JOIN ExpensesTable ex ON e.EventID = ex.EventID
                    WHERE ex.DatePayed IS NOT NULL AND e.EventID = @EventID
                    ORDER BY ex.DatePayed DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventID", eventID);
                SqlDataReader reader = cmd.ExecuteReader();

                StringBuilder transactionLog = new StringBuilder();

                while (reader.Read())
                {
                    string eventTitle = reader["EventTitle"].ToString();
                    decimal initialPaymentAmount = reader["InitialPaymentAmount"] != DBNull.Value ? Convert.ToDecimal(reader["InitialPaymentAmount"]) : 0;
                    DateTime datePayed = Convert.ToDateTime(reader["DatePayed"]);
                    string paymentMethod = reader["PaymentMethod"].ToString();
                    string paymentTerm = reader["PaymentTerm"].ToString();
                    string paymentStatus = reader["PaymentStatus"].ToString();

                    transactionLog.AppendLine($"Payment of ₱{initialPaymentAmount:N2} for {eventTitle} recorded {datePayed:MM/dd/yyyy HH:mm}");
                    transactionLog.AppendLine($"PAID BY {paymentMethod}");
                    transactionLog.AppendLine($"PAYMENT TERM: {paymentTerm}");
                    transactionLog.AppendLine($"PAYMENT STATUS: {paymentStatus}");
                    transactionLog.AppendLine($"--------------------------------");
                    transactionLog.AppendLine(); // Add extra line for spacing
                }

                EventDescriptionLbl.Text = transactionLog.ToString();
                EventDescriptionLbl.AutoSize = false;
                EventDescriptionLbl.MaximumSize = new Size(this.ClientSize.Width - 40, 0); // 40px margin
                EventDescriptionLbl.Width = this.ClientSize.Width - 40;
                EventDescriptionLbl.Height = TextRenderer.MeasureText(EventDescriptionLbl.Text, EventDescriptionLbl.Font, new Size(EventDescriptionLbl.Width, int.MaxValue), TextFormatFlags.WordBreak).Height;
                EventDescriptionLbl.TextAlign = ContentAlignment.TopLeft;

                // Enable word wrap if not already set
                EventDescriptionLbl.UseMnemonic = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transaction log: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is PaymentDetails)
                {
                    form.Show();
                    form.Activate();
                    break;
                }
            }
            this.Close();
        }
    }
}
