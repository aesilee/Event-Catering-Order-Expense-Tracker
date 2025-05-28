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
        private Timer eventsTimer;
        private FlowLayoutPanel notificationPanel;

        public Home()
        {
            InitializeComponent();
            InitializeFadeTimer();
            InitializeSidebar();
            InitializeNotificationPanel();
            LoadOngoingEvents();
            LoadUpcomingEvents();
            LoadNotifications();

            // Set up live refresh every 10 seconds for events only
            eventsTimer = new Timer();
            eventsTimer.Interval = 10000; // 10 seconds
            eventsTimer.Tick += (s, e) => { 
                LoadOngoingEvents(); 
                LoadUpcomingEvents(); 
            };
            eventsTimer.Start();

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
            if (eventsTimer != null)
            {
                eventsTimer.Stop();
                eventsTimer.Dispose();
            }
            Application.Exit();
        }

        private void LoadOngoingEvents()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

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
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
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

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
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

        private void InitializeNotificationPanel()
        {
            NotificationFlowPanel.AutoScroll = true;
            NotificationFlowPanel.FlowDirection = FlowDirection.TopDown;
            NotificationFlowPanel.WrapContents = false;
            NotificationFlowPanel.Padding = new Padding(10);
            NotificationFlowPanel.BackColor = Color.FromArgb(206, 193, 168);
        }

        private void LoadNotifications()
        {
            NotificationFlowPanel.Controls.Clear();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // 1. Newly created events (created in the last 24 hours)
                    string newEventsQuery = @"
                    SELECT EventTitle, EventDate, EventTime, Venue, DateCreated
                    FROM EventTable 
                    WHERE Hidden = 0 
                    AND DateCreated >= DATEADD(HOUR, -24, GETDATE())";
    
                    // 2. Events happening today
                    string todayEventsQuery = @"
                    SELECT EventTitle, EventDate, EventTime, Venue 
                    FROM EventTable 
                    WHERE Hidden = 0 
                    AND CONVERT(date, EventDate) = @today";

                    // 3. Events over budget
                    string overBudgetQuery = @"
                    SELECT e.EventTitle, e.EventDate, e.EventTime, e.Venue, e.EstimatedBudget, 
                    exp.TotalExpenses, exp.BudgetStatus
                    FROM EventTable e
                    INNER JOIN ExpensesTable exp ON e.EventID = exp.EventID
                    WHERE e.Hidden = 0 
                    AND exp.BudgetStatus = 'Over Budget'";

                    // 4. Venue conflicts
                    string venueConflictsQuery = @"
                    SELECT e1.EventTitle as Event1, e2.EventTitle as Event2, e1.Venue, e1.EventDate
                    FROM EventTable e1
                    JOIN EventTable e2 ON e1.Venue = e2.Venue 
                    AND e1.EventDate = e2.EventDate 
                    AND e1.EventID < e2.EventID
                    WHERE e1.Hidden = 0 AND e2.Hidden = 0";

                    // Execute queries and create notification panels
                    AddNotificationSection("New Events Added", newEventsQuery, con, Color.FromArgb(87, 153, 123));
                    AddNotificationSection("Today's Events", todayEventsQuery, con, Color.FromArgb(110, 164, 200));
                    AddNotificationSection("Over Budget Events", overBudgetQuery, con, Color.FromArgb(159, 71, 62));
                    AddNotificationSection("Venue Conflicts", venueConflictsQuery, con, Color.FromArgb(230, 159, 124));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading notifications: " + ex.Message);
                }
            }
        }

        private void AddNotificationSection(string title, string query, SqlConnection con, Color color)
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (query.Contains("@today"))
                {
                    cmd.Parameters.AddWithValue("@today", DateTime.Today);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Create header panel
                        var headerPanel = new Panel
                        {
                            Width = NotificationFlowPanel.Width - 38,
                            Height = 30,
                            Margin = new Padding(0, 0, 0, 8),
                            BackColor = color
                        };

                        var headerLabel = new Label
                        {
                            Text = title,
                            ForeColor = Color.White,
                            Font = new Font("Cambria", 12, FontStyle.Bold),
                            Dock = DockStyle.Fill,
                            TextAlign = ContentAlignment.MiddleCenter
                        };

                        headerPanel.Controls.Add(headerLabel);
                        NotificationFlowPanel.Controls.Add(headerPanel);

                        // Add notification items
                        while (reader.Read())
                        {
                            var notificationItem = new Panel
                            {
                                Width = NotificationFlowPanel.Width - 38,
                                Height = 70,
                                Margin = new Padding(0, 0, 0, 8),
                                BackColor = Color.FromArgb(241, 234, 218)
                            };

                            var contentLabel = new Label
                            {
                                Text = FormatNotificationContent(reader, title),
                                ForeColor = Color.FromArgb(88, 71, 56),
                                Font = new Font("Calibri", 10),
                                Dock = DockStyle.Fill,
                                TextAlign = ContentAlignment.MiddleLeft,
                                Padding = new Padding(0)
                            };

                            notificationItem.Controls.Add(contentLabel);
                            NotificationFlowPanel.Controls.Add(notificationItem);
                        }
                    }
                }
            }
        }

        private string FormatNotificationContent(SqlDataReader reader, string section)
        {
            switch (section)
            {
                case "New Events Added":
                    return $"{reader["EventTitle"]}\nCreated: {reader["DateCreated"]:MM/dd/yyyy HH:mm}\n{reader["EventDate"]:MM/dd/yyyy} at {reader["EventTime"]} - {reader["Venue"]}";
                
                case "Today's Events":
                    return $"{reader["EventTitle"]}\n{reader["EventDate"]:MM/dd/yyyy} at {reader["EventTime"]} - {reader["Venue"]}";
                
                case "Over Budget Events":
                    return $"{reader["EventTitle"]}\nBudget: ${reader["EstimatedBudget"]:N2} | Expenses: ${reader["TotalExpenses"]:N2}\nStatus: {reader["BudgetStatus"]}";
                
                case "Venue Conflicts":
                    return $"Venue Conflict: {reader["Venue"]}\n{reader["Event1"]} and {reader["Event2"]}\nDate: {reader["EventDate"]:MM/dd/yyyy}";
                
                default:
                    return "";
            }
        }

        private void NotificationPnl_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
