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
using System.IO;

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

            // Make ArchivesDgv read-only and non-editable
            ArchivesDgv.ReadOnly = true;
            ArchivesDgv.AllowUserToAddRows = false;
            ArchivesDgv.AllowUserToDeleteRows = false;
            ArchivesDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ArchivesDgv.MultiSelect = true;

            // Wire up search
            SearchTb.TextChanged += SearchTb_TextChanged;

            // Wire up archive and unarchive buttons
            ArchiveBtn.Click += ArchiveBtn_Click;
            UnarchiveBtn.Click += UnarchiveBtn_Click;

            // Wire up cell double-click events
            EventsDgv.CellDoubleClick += EventsDgv_CellDoubleClick;
            ArchivesDgv.CellDoubleClick += ArchivesDgv_CellDoubleClick;

            // Wire up download buttons
            EventDownloadBtn.Click += EventDownloadBtn_Click;
            ArchiveDownloadBtn.Click += ArchiveDownloadBtn_Click;

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
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
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

            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
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

        private void UnarchiveBtn_Click(object sender, EventArgs e)
        {
            if (ArchivesDgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one event to unarchive.");
                return;
            }

            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    foreach (DataGridViewRow row in ArchivesDgv.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            // Update EventTable to mark the event as not hidden
                            SqlCommand updateCmd = new SqlCommand(
                                "UPDATE EventTable SET Hidden = 0 WHERE EventTitle = @EventTitle AND EventDate = @EventDate AND EventTime = @EventTime", con, transaction);
                            
                            updateCmd.Parameters.AddWithValue("@EventTitle", row.Cells["EventTitle"].Value ?? DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@EventDate", row.Cells["EventDate"].Value ?? DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@EventTime", row.Cells["EventTime"].Value ?? DBNull.Value);
                            updateCmd.ExecuteNonQuery();

                            // Update ArchivesTable to mark the event as hidden
                            SqlCommand updateArchiveCmd = new SqlCommand(
                                "UPDATE ArchivesTable SET Hidden = 1 WHERE EventTitle = @EventTitle AND EventDate = @EventDate AND EventTime = @EventTime", con, transaction);
                            
                            updateArchiveCmd.Parameters.AddWithValue("@EventTitle", row.Cells["EventTitle"].Value ?? DBNull.Value);
                            updateArchiveCmd.Parameters.AddWithValue("@EventDate", row.Cells["EventDate"].Value ?? DBNull.Value);
                            updateArchiveCmd.Parameters.AddWithValue("@EventTime", row.Cells["EventTime"].Value ?? DBNull.Value);
                            updateArchiveCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Selected events unarchived successfully!");

                    // Reload both grids
                    LoadEvents();
                    LoadArchives();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error unarchiving events: " + ex.Message);
                }
            }
        }

        private void LoadArchives(string search = "")
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

            string query = "SELECT * FROM ArchivesTable WHERE Hidden = 0";
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

        private void EventsDgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure we're clicking on a row, not the header
            {
                DataGridView dgv = (DataGridView)sender;
                int eventID = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["EventID"].Value);
                Event eventForm = new Event(eventID);
                eventForm.TopMost = true;
                eventForm.Show();
            }
        }

        private void ArchivesDgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure we're clicking on a row, not the header
            {
                DataGridView dgv = (DataGridView)sender;
                string eventTitle = dgv.Rows[e.RowIndex].Cells["EventTitle"].Value.ToString();
                DateTime eventDate = Convert.ToDateTime(dgv.Rows[e.RowIndex].Cells["EventDate"].Value);
                TimeSpan eventTime = TimeSpan.Parse(dgv.Rows[e.RowIndex].Cells["EventTime"].Value.ToString());

                //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        string query = "SELECT EventID FROM EventTable WHERE EventTitle = @EventTitle AND EventDate = @EventDate AND EventTime = @EventTime";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@EventTitle", eventTitle);
                        cmd.Parameters.AddWithValue("@EventDate", eventDate);
                        cmd.Parameters.AddWithValue("@EventTime", eventTime);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int eventID = Convert.ToInt32(result);
                            Event eventForm = new Event(eventID);
                            eventForm.TopMost = true;
                            eventForm.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error opening event details: " + ex.Message);
                    }
                }
            }
        }

        private void EventDownloadBtn_Click(object sender, EventArgs e)
        {
            if (EventsDgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one event to download.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DownloadSelectedEvents(EventsDgv);
        }

        private void ArchiveDownloadBtn_Click(object sender, EventArgs e)
        {
            if (ArchivesDgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one event to download.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DownloadSelectedEvents(ArchivesDgv);
        }

        private void DownloadSelectedEvents(DataGridView dgv)
        {
            try
            {
                // Get the user's Downloads folder
                string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                int successCount = 0;
                int failCount = 0;

                foreach (DataGridViewRow row in dgv.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        try
                        {
                            string eventTitle = row.Cells["EventTitle"].Value?.ToString() ?? "Unknown";
                            string eventId = row.Cells["EventID"].Value?.ToString() ?? DateTime.Now.ToString("yyyyMMddHHmmss");
                            string fileName = $"EventReceipt_{eventId}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                            string filePath = Path.Combine(downloadsPath, fileName);

                            // Build the receipt content
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("========================================");
                            sb.AppendLine("         Event Catering Order Receipt   ");
                            sb.AppendLine("========================================");
                            sb.AppendLine($"Event Name:         {eventTitle}");
                            sb.AppendLine($"Event Date:         {row.Cells["EventDate"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Event Time:         {row.Cells["EventTime"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Event Type:         {row.Cells["EventType"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Venue:              {row.Cells["Venue"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine("----------------------------------------");
                            sb.AppendLine($"Customer Name:      {row.Cells["CustomerName"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Contact #:          {row.Cells["ContactNumber"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Email:              {row.Cells["EmailAddress"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Number of Guests:   {row.Cells["NumberOfGuests"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Menu/Meal Type:     {row.Cells["MenuType"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine($"Menu Details:       {row.Cells["MenuDetails"].Value?.ToString() ?? "N/A"}");
                            sb.AppendLine("----------------------------------------");
                            sb.AppendLine($"Estimated Budget:   ₱{Convert.ToDecimal(row.Cells["EstimatedBudget"].Value ?? 0):N2}");

                            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
                            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30";
                            
                            using (SqlConnection con = new SqlConnection(connectionString))
                            {
                                con.Open();
                                string query = @"
                                    SELECT FoodBeverages, Labor, Decorations, Rentals, Transportation, 
                                           Miscellaneous, TotalExpenses, PaymentStatus, RemainingBalance
                                    FROM ExpensesTable 
                                    WHERE EventID = @EventID";

                                SqlCommand cmd = new SqlCommand(query, con);
                                cmd.Parameters.AddWithValue("@EventID", row.Cells["EventID"].Value);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        sb.AppendLine($"Food & Beverages:   ₱{Convert.ToDecimal(reader["FoodBeverages"]):N2}");
                                        sb.AppendLine($"Labor:              ₱{Convert.ToDecimal(reader["Labor"]):N2}");
                                        sb.AppendLine($"Decorations:        ₱{Convert.ToDecimal(reader["Decorations"]):N2}");
                                        sb.AppendLine($"Rentals:            ₱{Convert.ToDecimal(reader["Rentals"]):N2}");
                                        sb.AppendLine($"Transportation:     ₱{Convert.ToDecimal(reader["Transportation"]):N2}");
                                        sb.AppendLine($"Miscellaneous:      ₱{Convert.ToDecimal(reader["Miscellaneous"]):N2}");
                                        sb.AppendLine($"Total Expenses:     ₱{Convert.ToDecimal(reader["TotalExpenses"]):N2}");
                                        sb.AppendLine($"Payment Status:     {reader["PaymentStatus"]}");
                                        sb.AppendLine($"Remaining Balance:  ₱{Convert.ToDecimal(reader["RemainingBalance"]):N2}");
                                    }
                                }
                            }

                            sb.AppendLine();
                            sb.AppendLine("Thank you!");
                            sb.AppendLine($"Event ID:           #{eventId}");

                            // Write to file
                            File.WriteAllText(filePath, sb.ToString());
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            // Log the error but continue with other files
                            Console.WriteLine($"Error saving receipt for event {row.Cells["EventTitle"].Value}: {ex.Message}");
                        }
                    }
                }

                // Show summary message
                string message = $"Download completed:\n{successCount} receipt(s) saved successfully";
                if (failCount > 0)
                {
                    message += $"\n{failCount} receipt(s) failed to save";
                }
                MessageBox.Show(message, "Download Complete", MessageBoxButtons.OK, 
                    failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during download process: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
