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
    public partial class Spreadsheet : Form
    {
        private Timer fadeTimer;
        private Form nextFormToOpen;
        private SidebarPanel sidebarPanel;

        public Spreadsheet()
        {
            InitializeComponent();
            InitializeFadeTimer();
            InitializeSidebar();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.TopMost = true;

            // Make EventsDgv read-only and non-editable
            EventsDgv.ReadOnly = true;
            EventsDgv.AllowUserToAddRows = false;
            EventsDgv.AllowUserToDeleteRows = false;
            EventsDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            EventsDgv.MultiSelect = true;
            EventsDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Allow multi-select in both DataGridViews
            ArchivesDgv.MultiSelect = true;

            // Wire up search
            SearchTb.TextChanged += SearchTb_TextChanged;

            // Wire up archive button
            ArchiveBtn.Click += ArchiveBtn_Click;

            LoadEvents();
            LoadArchives();
        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 5;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void InitializeSidebar()
        {
            sidebarPanel = new SidebarPanel("Spreadsheets");
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
                formToOpen.Activate();
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
                        nextFormToOpen.Activate();
                    }
                }
            }
        }

        private void StartFadeOutAndNavigate(Form formToOpen)
        {
            this.nextFormToOpen = formToOpen;
            fadeTimer.Start();
        }

        private void Spreadsheet_Load(object sender, EventArgs e)
        {
            // Load spreadsheet data
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            LoadEvents();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Handle panel painting if needed
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }

        private void LoadEvents(string search = "")
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string query = "SELECT * FROM EventTable WHERE Hidden = 0";
            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " AND (EventTitle LIKE @search OR EventType LIKE @search OR Venue LIKE @search OR CustomerName LIKE @search OR ContactNumber LIKE @search OR EmailAddress LIKE @search OR MenuType LIKE @search OR MenuDetails LIKE @search)";
            }

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

                    // Hide the Hidden column if it exists
                    if (EventsDgv.Columns.Contains("Hidden"))
                    {
                        EventsDgv.Columns["Hidden"].Visible = false;
                    }

                    // Allow horizontal scrolling
                    EventsDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    EventsDgv.ScrollBars = ScrollBars.Both;

                    // Auto-size columns and rows to fit content (max size)
                    foreach (DataGridViewColumn col in EventsDgv.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    EventsDgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    EventsDgv.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading events: " + ex.Message);
                }
            }
        }

        private void SearchTb_TextChanged(object sender, EventArgs e)
        {
            string search = SearchTb.Text.Trim();
            LoadEvents(search);
            LoadArchives(search);
        }

        private void ArchiveBtn_Click(object sender, EventArgs e)
        {
            if (EventsDgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one event to archive.");
                return;
            }

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    foreach (DataGridViewRow row in EventsDgv.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            // Build insert command for ArchivesTable
                            SqlCommand insertCmd = new SqlCommand(
                                @"INSERT INTO ArchivesTable 
                                (EventTitle, EventType, EventDate, EventTime, Venue, CustomerName, ContactNumber, EmailAddress, NumberOfGuests, MenuType, MenuDetails, CustomerNotes, EstimatedBudget)
                                VALUES (@EventTitle, @EventType, @EventDate, @EventTime, @Venue, @CustomerName, @ContactNumber, @EmailAddress, @NumberOfGuests, @MenuType, @MenuDetails, @CustomerNotes, @EstimatedBudget)", con, transaction);

                            insertCmd.Parameters.AddWithValue("@EventTitle", row.Cells["EventTitle"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@EventType", row.Cells["EventType"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@EventDate", row.Cells["EventDate"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@EventTime", row.Cells["EventTime"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Venue", row.Cells["Venue"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CustomerName", row.Cells["CustomerName"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@ContactNumber", row.Cells["ContactNumber"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@EmailAddress", row.Cells["EmailAddress"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@NumberOfGuests", row.Cells["NumberOfGuests"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@MenuType", row.Cells["MenuType"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@MenuDetails", row.Cells["MenuDetails"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CustomerNotes", row.Cells["CustomerNotes"].Value ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@EstimatedBudget", row.Cells["EstimatedBudget"].Value ?? DBNull.Value);

                            insertCmd.ExecuteNonQuery();

                            // Update EventTable to mark the event as hidden
                            SqlCommand updateCmd = new SqlCommand(
                                "UPDATE EventTable SET Hidden = 1 WHERE EventID = @EventID", con, transaction);
                            updateCmd.Parameters.AddWithValue("@EventID", row.Cells["EventID"].Value);
                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Selected events archived successfully!");

                    // Reload both grids
                    LoadEvents();
                    LoadArchives();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error archiving events: " + ex.Message);
                }
            }
        }

        private void LoadArchives(string search = "")
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string query = "SELECT * FROM ArchivesTable";
            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " WHERE EventTitle LIKE @search OR EventType LIKE @search OR Venue LIKE @search OR CustomerName LIKE @search OR ContactNumber LIKE @search OR EmailAddress LIKE @search OR MenuType LIKE @search OR MenuDetails LIKE @search";
            }

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
                    ArchivesDgv.DataSource = dt;
                    ArchivesDgv.RowHeadersVisible = false;

                    // Auto-size columns and rows to fit content (max size)
                    ArchivesDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    ArchivesDgv.ScrollBars = ScrollBars.Both;
                    foreach (DataGridViewColumn col in ArchivesDgv.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    ArchivesDgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    ArchivesDgv.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading archives: " + ex.Message);
                }
            }
        }
    }
}
