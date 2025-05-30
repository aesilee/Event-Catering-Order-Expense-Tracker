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
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
        private readonly int eventID;

        public TransactionLog(int eventID)
        {
            InitializeComponent();
            this.eventID = eventID;
            LoadTransactionLog();
        }

        private void LoadTransactionLog()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            CONVERT(varchar, DatePayed, 101) AS [Payment Date],
                            InitialPaymentAmount AS [Amount Paid],
                            PaymentStatus AS [Status],
                            RemainingBalance AS [Remaining Balance]
                        FROM ExpensesTable
                        WHERE EventID = @EventID 
                        AND IsPaymentRecord = 1
                        ORDER BY DatePayed DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@EventID", eventID);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Clear existing data and columns
                    TransactionsDgv.DataSource = null;
                    TransactionsDgv.Columns.Clear();

                    // Create and configure columns manually
                    DataGridViewTextBoxColumn dateColumn = new DataGridViewTextBoxColumn();
                    dateColumn.Name = "PaymentDate";
                    dateColumn.HeaderText = "Payment Date";
                    TransactionsDgv.Columns.Add(dateColumn);

                    DataGridViewTextBoxColumn amountColumn = new DataGridViewTextBoxColumn();
                    amountColumn.Name = "AmountPaid";
                    amountColumn.HeaderText = "Amount Paid";
                    amountColumn.DefaultCellStyle.Format = "C2";
                    amountColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    TransactionsDgv.Columns.Add(amountColumn);

                    DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn();
                    statusColumn.Name = "Status";
                    statusColumn.HeaderText = "Status";
                    TransactionsDgv.Columns.Add(statusColumn);

                    DataGridViewTextBoxColumn balanceColumn = new DataGridViewTextBoxColumn();
                    balanceColumn.Name = "RemainingBalance";
                    balanceColumn.HeaderText = "Remaining Balance";
                    balanceColumn.DefaultCellStyle.Format = "C2";
                    balanceColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    TransactionsDgv.Columns.Add(balanceColumn);

                    // Bind data manually
                    foreach (DataRow row in dt.Rows)
                    {
                        int rowIndex = TransactionsDgv.Rows.Add();
                        TransactionsDgv.Rows[rowIndex].Cells["PaymentDate"].Value = row["Payment Date"];
                        TransactionsDgv.Rows[rowIndex].Cells["AmountPaid"].Value = row["Amount Paid"];
                        TransactionsDgv.Rows[rowIndex].Cells["Status"].Value = row["Status"];
                        TransactionsDgv.Rows[rowIndex].Cells["RemainingBalance"].Value = row["Remaining Balance"];
                    }

                    // Auto-size columns
                    TransactionsDgv.AutoResizeColumns();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading transactions:\n{ex.Message}",
                                  "Database Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
