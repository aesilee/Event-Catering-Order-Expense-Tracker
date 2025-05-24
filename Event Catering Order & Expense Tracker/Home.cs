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
using System.Windows.Forms.DataVisualization.Charting;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class Home: Form
    {
        private Timer fadeTimer;
        private Form nextFormToOpen;
        private SidebarPanel sidebarPanel;
        private Timer refreshTimer;

        public Home()
        {
            InitializeComponent();
            InitializeFadeTimer();
            InitializeSidebar();
            LoadOngoingEvents();
            LoadUpcomingEvents();

            // Set up live refresh every 10 seconds
            refreshTimer = new Timer();
            refreshTimer.Interval = 10000; // 10 seconds
            refreshTimer.Tick += (s, e) => { LoadOngoingEvents(); LoadUpcomingEvents(); };
            refreshTimer.Start();

            LoadAnalyticsChart("Month"); // or "Year", "Quarter", "Day"
        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 5;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void InitializeSidebar()
        {
            sidebarPanel = new SidebarPanel("Dashboard");
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

        public void StartFadeIn()
        {
            // This method is kept for compatibility but is no longer used
            // since we removed fade animations except for logout
        }

        private void UsernameTb_TextChanged(object sender, EventArgs e)
        {
            // WIP Search Input
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }

        private void LoadOngoingEvents()
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
            string query = "SELECT EventTitle, EventDate, EventTime, Venue FROM EventTable WHERE Hidden = 0 AND CONVERT(date, EventDate) = @today";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@today", DateTime.Today);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    OngoingEventsDgv.DataSource = dt;
                    OngoingEventsDgv.RowHeadersVisible = false;

                    // Hide the first column if it has no header or is blank
                    if (OngoingEventsDgv.Columns.Count > 0 && string.IsNullOrWhiteSpace(OngoingEventsDgv.Columns[0].HeaderText))
                    {
                        OngoingEventsDgv.Columns[0].Visible = false;
                    }

                    // Only show the specified columns (already selected in query), but ensure all others are hidden if present
                    foreach (DataGridViewColumn col in OngoingEventsDgv.Columns)
                    {
                        if (col.Name != "EventTitle" && col.Name != "EventDate" && col.Name != "EventTime" && col.Name != "Venue")
                        {
                            col.Visible = false;
                        }
                        else
                        {
                            col.Visible = true;
                        }
                    }

                    // Auto-size columns and rows
                    OngoingEventsDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    OngoingEventsDgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    OngoingEventsDgv.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading ongoing events: " + ex.Message);
                }
            }
        }

        private void LoadUpcomingEvents()
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
            string query = "SELECT EventTitle, EventDate, EventTime, Venue FROM EventTable WHERE Hidden = 0 AND CONVERT(date, EventDate) > @tomorrow AND CONVERT(date, EventDate) <= @future";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@tomorrow", DateTime.Today);
                    adapter.SelectCommand.Parameters.AddWithValue("@future", DateTime.Today.AddDays(5));
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    UpcomingEventsDgv.DataSource = dt;
                    UpcomingEventsDgv.RowHeadersVisible = false;

                    // Hide the first column if it has no header or is blank
                    if (UpcomingEventsDgv.Columns.Count > 0 && string.IsNullOrWhiteSpace(UpcomingEventsDgv.Columns[0].HeaderText))
                    {
                        UpcomingEventsDgv.Columns[0].Visible = false;
                    }

                    // Only show the specified columns (already selected in query), but ensure all others are hidden if present
                    foreach (DataGridViewColumn col in UpcomingEventsDgv.Columns)
                    {
                        if (col.Name != "EventTitle" && col.Name != "EventDate" && col.Name != "EventTime" && col.Name != "Venue")
                        {
                            col.Visible = false;
                        }
                        else
                        {
                            col.Visible = true;
                        }
                    }

                    // Auto-size columns and rows
                    UpcomingEventsDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    UpcomingEventsDgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    UpcomingEventsDgv.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading upcoming events: " + ex.Message);
                }
            }
        }

        private void LoadAnalyticsChart(string groupBy = "Month", DateTime? start = null, DateTime? end = null)
        {
            AnalyticsChart.Series.Clear();
            AnalyticsChart.ChartAreas.Clear();
            AnalyticsChart.ChartAreas.Add("MainArea");

            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
            string dateFormat = "yyyy-MM"; // Default: group by month

            string selectDate = "FORMAT(EventDate, 'yyyy-MM')";
            if (groupBy == "Year")
            {
                selectDate = "FORMAT(EventDate, 'yyyy')";
                dateFormat = "yyyy";
            }
            else if (groupBy == "Quarter")
            {
                selectDate = "FORMAT(EventDate, 'yyyy') + '-Q' + DATENAME(QUARTER, EventDate)";
                dateFormat = "yyyy-'Q'q";
            }
            else if (groupBy == "Day")
            {
                selectDate = "FORMAT(EventDate, 'yyyy-MM-dd')";
                dateFormat = "yyyy-MM-dd";
            }

            string query = $@"
                SELECT {selectDate} AS Period,
                       SUM(EventTable.EstimatedBudget) AS TotalEstimatedBudget,
                       SUM(ISNULL(ExpensesTable.TotalExpenses, 0)) AS TotalExpenses
                FROM EventTable
                LEFT JOIN ExpensesTable ON ExpensesTable.EventID = EventTable.EventID
                WHERE EventTable.Hidden = 0
            ";

            if (start.HasValue && end.HasValue)
            {
                query += " AND EventDate >= @start AND EventDate <= @end";
            }

            query += $" GROUP BY {selectDate} ORDER BY {selectDate}";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    if (start.HasValue && end.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@start", start.Value.Date);
                        cmd.Parameters.AddWithValue("@end", end.Value.Date);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Add series
                    var budgetSeries = new Series("Total Estimated Budget")
                    {
                        ChartType = SeriesChartType.Column,
                        XValueType = ChartValueType.String
                    };
                    var expensesSeries = new Series("Total Expenses")
                    {
                        ChartType = SeriesChartType.Column,
                        XValueType = ChartValueType.String
                    };

                    foreach (DataRow row in dt.Rows)
                    {
                        string period = row["Period"].ToString();
                        decimal budget = row["TotalEstimatedBudget"] != DBNull.Value ? Convert.ToDecimal(row["TotalEstimatedBudget"]) : 0;
                        decimal expenses = row["TotalExpenses"] != DBNull.Value ? Convert.ToDecimal(row["TotalExpenses"]) : 0;

                        budgetSeries.Points.AddXY(period, budget);
                        expensesSeries.Points.AddXY(period, expenses);
                    }

                    AnalyticsChart.Series.Add(budgetSeries);
                    AnalyticsChart.Series.Add(expensesSeries);

                    AnalyticsChart.ChartAreas[0].AxisX.Title = groupBy;
                    AnalyticsChart.ChartAreas[0].AxisY.Title = "Amount";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading analytics chart: " + ex.Message);
                }
            }
        }
    }
}
