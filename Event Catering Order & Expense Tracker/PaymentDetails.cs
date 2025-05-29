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
            // Make PaymentTb accept only numbers
            PaymentTb.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }
                // Only allow one decimal point
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
            };

            // Handle save button click
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
                            // Calculate new balance
                            decimal newBalance = currentBalance - paymentAmount;
                            string newStatus = newBalance <= 0 ? "Fully Paid" : "Unpaid";

                            // Update ExpensesTable
                            string updateQuery = @"
                                UPDATE ExpensesTable 
                                SET RemainingBalance = @NewBalance,
                                    PaymentStatus = @NewStatus,
                                    DatePayed = @DatePayed
                                WHERE EventID = @EventID";

                            SqlCommand cmd = new SqlCommand(updateQuery, con, transaction);
                            cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                            cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                            cmd.Parameters.AddWithValue("@DatePayed", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EventID", eventID);

                            cmd.ExecuteNonQuery();
                            transaction.Commit();

                            // Update UI
                            currentBalance = newBalance;
                            RemainingBalanceLbl.Text = $"₱{currentBalance:N2}";
                            StatusLbl.Text = newStatus;
                            StatusLbl.ForeColor = newStatus == "Fully Paid" ? Color.Green : Color.Red;
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
                            ex.TotalExpenses, ex.BudgetStatus
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
                            NextPaymentLbl.Text = Convert.ToDateTime(reader["NextPayment"]).ToShortDateString();
                            currentBalance = Convert.ToDecimal(reader["RemainingBalance"]);
                            RemainingBalanceLbl.Text = $"₱{currentBalance:N2}";
                            TotalExpensesLbl.Text = $"₱{Convert.ToDecimal(reader["TotalExpenses"]):N2}";

                            // Set status label color based on payment status
                            string paymentStatus = reader["PaymentStatus"].ToString();
                            if (paymentStatus == "Fully Paid")
                            {
                                StatusLbl.ForeColor = Color.Green;
                            }
                            else if (paymentStatus == "Unpaid")
                            {
                                StatusLbl.ForeColor = Color.Red;
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
            // Clear any existing series
            chart2.Series.Clear();

            // Create new series
            var series = new System.Windows.Forms.DataVisualization.Charting.Series("ExpenseBreakdown");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";
            series["DoughnutRadius"] = "60";
            series.Label = "#PERCENT{P0}";
            series.LegendText = "#VALX (#VALY)";

            // Add series to chart
            chart2.Series.Add(series);

            // Set colors for the pie chart
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

                    // Clear existing data
                    chart2.Series["ExpenseBreakdown"].Points.Clear();

                    // Add data points
                    foreach (DataRow row in dt.Rows)
                    {
                        string category = row["Category"].ToString();
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        chart2.Series["ExpenseBreakdown"].Points.AddXY(category, amount);
                    }

                    // Set colors for the pie chart
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

                    // Update the legend text to include peso currency
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
    }
}
