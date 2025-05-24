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

            // Set up live refresh every 10 seconds
            refreshTimer = new Timer();
            refreshTimer.Interval = 10000; // 10 seconds
            refreshTimer.Tick += (s, e) => LoadOngoingEvents();
            refreshTimer.Start();
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
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
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
    }
}
