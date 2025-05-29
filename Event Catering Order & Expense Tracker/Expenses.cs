using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class Expenses : Form
    {
        private Timer fadeTimer;
        private Timer refreshTimer;
        private Form nextFormToOpen;
        private SidebarPanel sidebarPanel;

        public Expenses()
        {
            InitializeComponent();
            InitializeFadeTimer();
            InitializeRefreshTimer();
            InitializeSidebar();
            this.StartPosition = FormStartPosition.CenterScreen;

            // Make EventsDgv read-only
            EventsDgv.ReadOnly = true;
            EventsDgv.AllowUserToAddRows = false;
            EventsDgv.AllowUserToDeleteRows = false;
            EventsDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            EventsDgv.MultiSelect = false;

            // Wire up cell double-click event
            EventsDgv.CellDoubleClick += EventsDgv_CellDoubleClick;

            // Wire up search
            SearchTb.TextChanged += SearchTb_TextChanged;

            // Initialize chart
            InitializeChart();
            InitializeProfitChart();

            LoadExpenses();
            LoadPaymentStatusChart();
            LoadExpenseBreakdownChart();
        }

        private void InitializeRefreshTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 5000; // Refresh every 5 seconds
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // Store current selections
            var selectedEvents = EventsDgv.SelectedRows.Cast<DataGridViewRow>()
                .Select(r => r.Cells["EventTitle"].Value.ToString())
                .ToList();

            LoadExpenses();
            LoadPaymentStatusChart();
            LoadExpenseBreakdownChart();

            // Restore selections
            foreach (DataGridViewRow row in EventsDgv.Rows)
            {
                if (selectedEvents.Contains(row.Cells["EventTitle"].Value.ToString()))
                {
                    row.Selected = true;
                }
            }
        }

        private void InitializeChart()
        {
            // Clear any existing series
            EventCharts.Series.Clear();

            // Create new series
            var series = new System.Windows.Forms.DataVisualization.Charting.Series("PaymentStatus");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";
            series["DoughnutRadius"] = "60";
            series.Label = "#PERCENT{P0}";
            series.LegendText = "#VALX (#VALY)";

            // Add series to chart
            EventCharts.Series.Add(series);

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

        private void LoadExpenses(string search = "")
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            string query = @"SELECT e.EventTitle, 
                                  exp.PaymentStatus, 
                                  exp.NextPayment, 
                                  exp.RemainingBalance
                           FROM EventTable e
                           LEFT JOIN ExpensesTable exp ON e.EventID = exp.EventID
                           WHERE e.Hidden = 0";

            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " AND (e.EventTitle LIKE @search OR e.EventType LIKE @search OR e.Venue LIKE @search OR e.CustomerName LIKE @search OR e.ContactNumber LIKE @search OR e.EmailAddress LIKE @search OR e.MenuType LIKE @search OR e.MenuDetails LIKE @search)";
            }

            query += " ORDER BY e.EventDate DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                    }
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    EventsDgv.DataSource = dt;
                    EventsDgv.RowHeadersVisible = false;

                    // Auto-size columns and rows
                    EventsDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    EventsDgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    EventsDgv.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);

                    // Format columns
                    if (EventsDgv.Columns.Contains("NextPayment"))
                    {
                        EventsDgv.Columns["NextPayment"].DefaultCellStyle.Format = "MM/dd/yyyy";
                    }
                    if (EventsDgv.Columns.Contains("RemainingBalance"))
                    {
                        EventsDgv.Columns["RemainingBalance"].DefaultCellStyle.Format = "N2";
                        EventsDgv.Columns["RemainingBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading expenses: " + ex.Message);
                }
            }
        }

        private void LoadPaymentStatusChart()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            CASE 
                                WHEN PaymentStatus = 'Fully Paid' THEN 'Fully Paid'
                                ELSE 'Unpaid'
                            END as PaymentStatus,
                            COUNT(*) as Count
                        FROM ExpensesTable ex
                        INNER JOIN EventTable e ON ex.EventID = e.EventID
                        WHERE e.Hidden = 0
                        GROUP BY 
                            CASE 
                                WHEN PaymentStatus = 'Fully Paid' THEN 'Fully Paid'
                                ELSE 'Unpaid'
                            END";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Clear existing data
                    EventCharts.Series["PaymentStatus"].Points.Clear();

                    // Add data points
                    foreach (DataRow row in dt.Rows)
                    {
                        string status = row["PaymentStatus"].ToString();
                        int count = Convert.ToInt32(row["Count"]);
                        EventCharts.Series["PaymentStatus"].Points.AddXY(status, count);
                    }

                    // Set colors for the pie chart
                    if (EventCharts.Series["PaymentStatus"].Points.Count > 0)
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

                        for (int i = 0; i < EventCharts.Series["PaymentStatus"].Points.Count; i++)
                        {
                            EventCharts.Series["PaymentStatus"].Points[i].Color = colors[i % colors.Length];
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading payment status chart: " + ex.Message);
                }
            }
        }

        private void InitializeProfitChart()
        {
            // Clear any existing series
            ProfitChart.Series.Clear();

            // Create new series
            var series = new System.Windows.Forms.DataVisualization.Charting.Series("ExpenseBreakdown");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";
            series["DoughnutRadius"] = "60";
            series.Label = "#PERCENT{P0}";
            series.LegendText = "#VALX (#VALY)";

            // Add series to chart
            ProfitChart.Series.Add(series);

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
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            'Food & Beverages' as Category, SUM(FoodBeverages) as Total
                        FROM ExpensesTable ex
                        INNER JOIN EventTable e ON ex.EventID = e.EventID
                        WHERE e.Hidden = 0
                        UNION ALL
                        SELECT 'Labor', SUM(Labor)
                        FROM ExpensesTable ex
                        INNER JOIN EventTable e ON ex.EventID = e.EventID
                        WHERE e.Hidden = 0
                        UNION ALL
                        SELECT 'Decorations', SUM(Decorations)
                        FROM ExpensesTable ex
                        INNER JOIN EventTable e ON ex.EventID = e.EventID
                        WHERE e.Hidden = 0
                        UNION ALL
                        SELECT 'Rentals', SUM(Rentals)
                        FROM ExpensesTable ex
                        INNER JOIN EventTable e ON ex.EventID = e.EventID
                        WHERE e.Hidden = 0
                        UNION ALL
                        SELECT 'Transportation', SUM(Transportation)
                        FROM ExpensesTable ex
                        INNER JOIN EventTable e ON ex.EventID = e.EventID
                        WHERE e.Hidden = 0
                        UNION ALL
                        SELECT 'Miscellaneous', SUM(Miscellaneous)
                        FROM ExpensesTable ex
                        INNER JOIN EventTable e ON ex.EventID = e.EventID
                        WHERE e.Hidden = 0";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Clear existing data
                    ProfitChart.Series["ExpenseBreakdown"].Points.Clear();

                    // Add data points
                    foreach (DataRow row in dt.Rows)
                    {
                        string category = row["Category"].ToString();
                        decimal total = Convert.ToDecimal(row["Total"]);
                        ProfitChart.Series["ExpenseBreakdown"].Points.AddXY(category, total);
                    }

                    // Set colors for the pie chart
                    if (ProfitChart.Series["ExpenseBreakdown"].Points.Count > 0)
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

                        for (int i = 0; i < ProfitChart.Series["ExpenseBreakdown"].Points.Count; i++)
                        {
                            ProfitChart.Series["ExpenseBreakdown"].Points[i].Color = colors[i % colors.Length];
                        }
                    }

                    // Update the legend text to include peso currency
                    foreach (var point in ProfitChart.Series["ExpenseBreakdown"].Points)
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

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 5;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void InitializeSidebar()
        {
            sidebarPanel = new SidebarPanel("Expenses");
            sidebarPanel.NavigationRequested += SidebarPanel_NavigationRequested;
            this.Controls.Add(sidebarPanel);
            sidebarPanel.Dock = DockStyle.Left;
        }

        private void SidebarPanel_NavigationRequested(object sender, Form formToOpen)
        {
            if (formToOpen is Login)
            {
                // Use fade animation only for logout
                StartFadeOutAndNavigate(formToOpen);
            }
            else
            {
                // Direct navigation for all other forms
                if (refreshTimer != null)
                {
                    refreshTimer.Stop();
                    refreshTimer.Dispose();
                }
                formToOpen.Show();
                this.Dispose();
            }
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0.0 && nextFormToOpen != null)
            {
                this.Opacity -= 0.20;
                if (this.Opacity <= 0.0)
                {
                    this.Opacity = 0.0;
                    fadeTimer.Stop();
                    this.Hide();

                    if (nextFormToOpen != null)
                    {
                        nextFormToOpen.Show();
                    }
                }
            }
        }

        private void StartFadeOutAndNavigate(Form formToOpen)
        {
            this.nextFormToOpen = formToOpen;
            fadeTimer.Start();
        }

        private void EventsDgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure we're clicking on a row, not the header
            {
                DataGridView dgv = (DataGridView)sender;
                string eventTitle = dgv.Rows[e.RowIndex].Cells["EventTitle"].Value.ToString();

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
                //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        string query = "SELECT EventID FROM EventTable WHERE EventTitle = @EventTitle AND Hidden = 0";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@EventTitle", eventTitle);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int eventID = Convert.ToInt32(result);
                            PaymentDetails paymentForm = new PaymentDetails(eventID);
                            paymentForm.TopMost = true;
                            paymentForm.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error opening payment details: " + ex.Message);
                    }
                }
            }
        }

        private void SearchTb_TextChanged(object sender, EventArgs e)
        {
            string search = SearchTb.Text.Trim();
            LoadExpenses(search);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }

        private void EventsDgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
