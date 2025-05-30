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
    public partial class PaymentDetails : Form
    {
        private int eventID;
        private decimal currentBalance;

        public PaymentDetails(int eventID)
        {
            InitializeComponent();
            this.eventID = eventID;
            LoadEventData();
            InitializePaymentHandling();
            InitializeExpenseBreakdownChart();
            LoadExpenseBreakdownChart();
        }

        private void InitializePaymentHandling()
        {
            PaymentTb.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
            };

            SaveBtn.Click += SaveBtn_Click;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PaymentTb.Text))
            {
                MessageBox.Show("Please enter a payment amount.");
                return;
            }

            if (!decimal.TryParse(PaymentTb.Text, out decimal paymentAmount))
            {
                MessageBox.Show("Please enter a valid payment amount.");
                return;
            }

            if (paymentAmount <= 0)
            {
                MessageBox.Show("Payment amount must be greater than 0.");
                return;
            }

            if (paymentAmount > currentBalance)
            {
                MessageBox.Show("Payment amount cannot exceed the remaining balance.");
                return;
            }

            ProcessPayment(paymentAmount);
        }

        private void ProcessPayment(decimal paymentAmount)
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            string selectQuery = @"SELECT PaymentStatus, InitialPaymentAmount, FinalPaymentAmount, RemainingBalance, PaymentTerm, PaymentDate, FinalPaymentDate FROM ExpensesTable WHERE EventID = @EventID";
                            SqlCommand selectCmd = new SqlCommand(selectQuery, con, transaction);
                            selectCmd.Parameters.AddWithValue("@EventID", eventID);
                            string prevStatus = "Unpaid";
                            decimal prevInitialPayment = 0;
                            decimal prevFinalPayment = 0;
                            decimal prevRemainingBalance = 0;
                            string prevTerm = "Full Payment"; 
                            DateTime? prevPaymentDate = null;
                            DateTime? prevFinalPaymentDate = null;

                            using (SqlDataReader reader = selectCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    prevStatus = reader["PaymentStatus"]?.ToString() ?? "Unpaid";
                                    prevInitialPayment = reader["InitialPaymentAmount"] != DBNull.Value ? Convert.ToDecimal(reader["InitialPaymentAmount"]) : 0;
                                    prevFinalPayment = reader["FinalPaymentAmount"] != DBNull.Value ? Convert.ToDecimal(reader["FinalPaymentAmount"]) : 0;
                                    prevRemainingBalance = reader["RemainingBalance"] != DBNull.Value ? Convert.ToDecimal(reader["RemainingBalance"]) : 0;
                                    prevTerm = reader["PaymentTerm"]?.ToString() ?? "Full Payment";
                                    prevPaymentDate = reader["PaymentDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["PaymentDate"]) : null;
                                    prevFinalPaymentDate = reader["FinalPaymentDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["FinalPaymentDate"]) : null;
                                }
                            }

                            if (prevStatus == "Fully Paid")
                            {
                                MessageBox.Show("This event is already fully paid.");
                                transaction.Rollback();
                                return;
                            }

                            decimal newBalance = currentBalance - paymentAmount;
                            string newStatus = prevStatus; 
                            decimal newInitialPayment = prevInitialPayment; 
                            decimal newFinalPayment = prevFinalPayment;
                            DateTime? newInitialPaymentDate = null; 
                            DateTime? newFinalPaymentDate = null; 

                            SqlCommand cmd;
                            string updateQuery;
                            string finalPaymentAndDateSet = ""; 

                            if (prevTerm == "Full Payment")
                            {
                                newStatus = (newBalance <= 0) ? "Fully Paid" : prevStatus; 

                                updateQuery = @"
                                    UPDATE ExpensesTable 
                                    SET RemainingBalance = @NewBalance,
                                        PaymentStatus = @NewStatus,
                                        DatePayed = @DatePayed
                                    WHERE EventID = @EventID";

                                cmd = new SqlCommand(updateQuery, con, transaction);
                                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                                cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                                cmd.Parameters.AddWithValue("@DatePayed", DateTime.Now);
                                cmd.Parameters.AddWithValue("@EventID", eventID);
                            }
                            else 
                            {
                                newInitialPayment = prevInitialPayment + paymentAmount; 
                                newInitialPaymentDate = DateTime.Now;
                                newStatus = (newBalance <= 0) ? "Fully Paid" : "Partially Paid";

                                if (newStatus == "Fully Paid")
                                {
                                     newFinalPayment = paymentAmount; 
                                     newFinalPaymentDate = DateTime.Now;

                                     finalPaymentAndDateSet = ", FinalPaymentAmount = @NewFinalPayment, FinalPaymentDate = @NewFinalPaymentDate";
                                }

                                updateQuery = @"
                                    UPDATE ExpensesTable 
                                    SET RemainingBalance = @NewBalance,
                                        InitialPaymentAmount = @NewInitialPayment,
                                        PaymentStatus = @NewStatus,
                                        DatePayed = @DatePayed,
                                        InitialPaymentDate = @InitialPaymentDate
                                        {0}
                                    WHERE EventID = @EventID";

                                cmd = new SqlCommand(string.Format(updateQuery, finalPaymentAndDateSet), con, transaction);
                                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                                cmd.Parameters.AddWithValue("@NewInitialPayment", newInitialPayment);
                                cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                                cmd.Parameters.AddWithValue("@DatePayed", DateTime.Now);
                                cmd.Parameters.AddWithValue("@InitialPaymentDate", newInitialPaymentDate);
                                if (newStatus == "Fully Paid")
                                {
                                     cmd.Parameters.AddWithValue("@NewFinalPayment", newFinalPayment);
                                     cmd.Parameters.AddWithValue("@NewFinalPaymentDate", newFinalPaymentDate);
                                }
                                cmd.Parameters.AddWithValue("@EventID", eventID);
                            }

                            cmd.ExecuteNonQuery();

                            selectQuery = @"SELECT RemainingBalance, PaymentStatus, FinalPaymentDate, PaymentDate, PaymentTerm FROM ExpensesTable WHERE EventID = @EventID";
                            selectCmd = new SqlCommand(selectQuery, con, transaction);
                            selectCmd.Parameters.AddWithValue("@EventID", eventID);
                            decimal currentRemainingBalance = 0;
                            string currentPaymentStatus = "Unpaid";
                            DateTime? finalPayDate = null;
                            DateTime? paymentDate = null;
                            string currentPaymentTerm = "Full Payment";

                            using (SqlDataReader overdueReader = selectCmd.ExecuteReader())
                            {
                                if (overdueReader.Read())
                                {
                                    currentRemainingBalance = overdueReader["RemainingBalance"] != DBNull.Value ? Convert.ToDecimal(overdueReader["RemainingBalance"]) : 0;
                                    currentPaymentStatus = overdueReader["PaymentStatus"]?.ToString() ?? "Unpaid";
                                    finalPayDate = overdueReader["FinalPaymentDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(overdueReader["FinalPaymentDate"]) : null;
                                    paymentDate = overdueReader["PaymentDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(overdueReader["PaymentDate"]) : null;
                                    currentPaymentTerm = overdueReader["PaymentTerm"]?.ToString() ?? "Full Payment";
                                }
                            }

                            DateTime now = DateTime.Now;
                             if (currentRemainingBalance > 0 && currentPaymentStatus != "Fully Paid" && currentPaymentStatus != "Overdue")
                            {
                                if (currentPaymentTerm == "Full Payment" && paymentDate.HasValue && now > paymentDate.Value)
                                {
                                     string setOverdueQuery = @"UPDATE ExpensesTable SET PaymentStatus = 'Overdue' WHERE EventID = @EventID";
                                     SqlCommand setOverdueCmd = new SqlCommand(setOverdueQuery, con, transaction);
                                     setOverdueCmd.Parameters.AddWithValue("@EventID", eventID);
                                     setOverdueCmd.ExecuteNonQuery();
                                     currentPaymentStatus = "Overdue";
                                }
                                else if (currentPaymentTerm != "Full Payment" && finalPayDate.HasValue && now > finalPayDate.Value)
                                {
                                     string setOverdueQuery = @"UPDATE ExpensesTable SET PaymentStatus = 'Overdue' WHERE EventID = @EventID";
                                     SqlCommand setOverdueCmd = new SqlCommand(setOverdueQuery, con, transaction);
                                     setOverdueCmd.Parameters.AddWithValue("@EventID", eventID);
                                     setOverdueCmd.ExecuteNonQuery();
                                     currentPaymentStatus = "Overdue"; 
                                }
                            }

                            transaction.Commit();

                            currentBalance = newBalance; 
                            RemainingBalanceLbl.Text = $"₱{currentBalance:N2}";
                            StatusLbl.Text = currentPaymentStatus; 
                            StatusLbl.ForeColor = currentPaymentStatus == "Fully Paid" ? Color.Green : (currentPaymentStatus == "Overdue" ? Color.Red : Color.Orange);
                            PaymentTb.Clear();

                            MessageBox.Show("Payment processed successfully!");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error processing payment: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting to database: " + ex.Message);
                }
            }
        }

        private void LoadEventData()
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            e.EventTitle, e.EventType,
                            ex.PaymentStatus, ex.NextPayment, ex.RemainingBalance,
                            ex.TotalExpenses, ex.BudgetStatus, ex.PaymentTerm, ex.PaymentDate
                        FROM EventTable e
                        LEFT JOIN ExpensesTable ex ON e.EventID = ex.EventID
                        WHERE e.EventID = @EventID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventID", eventID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EventTitleLbl.Text = reader["EventTitle"].ToString();
                            EventTypeLbl.Text = reader["EventType"].ToString();
                            StatusLbl.Text = reader["PaymentStatus"].ToString();
                            
                            string paymentTerm = reader["PaymentTerm"]?.ToString();
                            if (paymentTerm == "Full Payment")
                            {
                                if (reader["PaymentDate"] != DBNull.Value)
                                {
                                    NextPaymentLbl.Text = Convert.ToDateTime(reader["PaymentDate"]).ToShortDateString();
                                }
                                else
                                {
                                    NextPaymentLbl.Text = "N/A";
                                }
                            }
                            else 
                            {
                                if (reader["NextPayment"] != DBNull.Value)
                                {
                                    NextPaymentLbl.Text = Convert.ToDateTime(reader["NextPayment"]).ToShortDateString();
                                }
                                else
                                {
                                    NextPaymentLbl.Text = "N/A";
                                }
                            }
                            
                            currentBalance = Convert.ToDecimal(reader["RemainingBalance"]);
                            RemainingBalanceLbl.Text = $"₱{currentBalance:N2}";
                            TotalExpensesLbl.Text = $"₱{Convert.ToDecimal(reader["TotalExpenses"]):N2}";

                            string paymentStatus = reader["PaymentStatus"].ToString();
                            if (paymentStatus == "Fully Paid")
                            {
                                StatusLbl.ForeColor = Color.Green;
                            }
                            else if (paymentStatus == "Overdue")
                            {
                                StatusLbl.ForeColor = Color.Red;
                            }
                            else 
                            {
                                StatusLbl.ForeColor = Color.Orange;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading event data: " + ex.Message);
                }
            }
        }

        private void InitializeExpenseBreakdownChart()
        {
            chart2.Series.Clear();

            var series = new System.Windows.Forms.DataVisualization.Charting.Series("ExpenseBreakdown");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";
            series["DoughnutRadius"] = "60";
            series.Label = "#PERCENT{P0}";
            series.LegendText = "#VALX (#VALY)";

            chart2.Series.Add(series);

            Color[] colors = new Color[] 
            {
                Color.FromArgb(88, 71, 56),    // Brown
                Color.FromArgb(170, 163, 150), // Light Brown
                Color.FromArgb(206, 193, 168), // Tan
                Color.FromArgb(241, 234, 218), // Light Tan
                Color.FromArgb(74, 57, 49),    // Dark Brown
                Color.FromArgb(150, 143, 130)  // Medium Brown
            };

            // Apply colors to the series
            series.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            series.CustomProperties = "PieLabelStyle=Outside";
            series["DoughnutRadius"] = "60";
            series.Label = "#PERCENT{P0}";
            series.LegendText = "#VALX (#VALY)";
        }

        private void LoadExpenseBreakdownChart()
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            'Food & Beverages' as Category, FoodBeverages as Amount
                        FROM ExpensesTable
                        WHERE EventID = @EventID
                        UNION ALL
                        SELECT 'Labor', Labor
                        FROM ExpensesTable
                        WHERE EventID = @EventID
                        UNION ALL
                        SELECT 'Decorations', Decorations
                        FROM ExpensesTable
                        WHERE EventID = @EventID
                        UNION ALL
                        SELECT 'Rentals', Rentals
                        FROM ExpensesTable
                        WHERE EventID = @EventID
                        UNION ALL
                        SELECT 'Transportation', Transportation
                        FROM ExpensesTable
                        WHERE EventID = @EventID
                        UNION ALL
                        SELECT 'Miscellaneous', Miscellaneous
                        FROM ExpensesTable
                        WHERE EventID = @EventID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventID", eventID);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    chart2.Series["ExpenseBreakdown"].Points.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        string category = row["Category"].ToString();
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        chart2.Series["ExpenseBreakdown"].Points.AddXY(category, amount);
                    }

                    if (chart2.Series["ExpenseBreakdown"].Points.Count > 0)
                    {
                        Color[] colors = new Color[] 
                        {
                            Color.FromArgb(88, 71, 56),    // Brown
                            Color.FromArgb(170, 163, 150), // Light Brown
                            Color.FromArgb(206, 193, 168), // Tan
                            Color.FromArgb(241, 234, 218), // Light Tan
                            Color.FromArgb(74, 57, 49),    // Dark Brown
                            Color.FromArgb(150, 143, 130)  // Medium Brown
                        };

                        for (int i = 0; i < chart2.Series["ExpenseBreakdown"].Points.Count; i++)
                        {
                            chart2.Series["ExpenseBreakdown"].Points[i].Color = colors[i % colors.Length];
                        }
                    }

                    foreach (var point in chart2.Series["ExpenseBreakdown"].Points)
                    {
                        point.LegendText = $"{point.AxisLabel} (₱{point.YValues[0]:N2})";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading expense breakdown chart: " + ex.Message);
                }
            }
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is Expenses)
                {
                    form.Show();
                    form.Activate();
                    break;
                }
            }
            this.Close();
        }

        private void TransactionLogBtn_Click(object sender, EventArgs e)
        {
            var transactionLogForm = new TransactionLog(this.eventID);
            transactionLogForm.Show();
            this.Hide();
        }
    }
}
