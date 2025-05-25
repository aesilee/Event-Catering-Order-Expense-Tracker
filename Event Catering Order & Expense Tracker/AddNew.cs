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
    public partial class AddNew : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");
        //SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");


        private Timer fadeTimer;
        private Form nextFormToOpen;
        private SidebarPanel sidebarPanel;

        public AddNew()
        {
            InitializeComponent();
            InitializeFadeTimer();
            InitializeSidebar();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.TopMost = true;

            // Create a panel to hold all content including sidebar
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Move all controls except sidebar to contentPanel
            List<Control> controlsToMove = new List<Control>();
            foreach (Control control in this.Controls)
            {
                if (control != sidebarPanel)
                {
                    controlsToMove.Add(control);
                }
            }

            foreach (Control control in controlsToMove)
            {
                this.Controls.Remove(control);
                contentPanel.Controls.Add(control);
            }

            // Add contentPanel to form
            this.Controls.Add(contentPanel);

            EventTypeCb.Items.AddRange(new string[]
            {
                "Birthday Party",
                "Wedding Reception",
                "Graduation Party",
                "Corporate Event",
                "Awarding/Recognition Night",
                "Engagement Party",
                "Christening/Baptism",
                "Baby Shower",
                "Family Reunion",
                "Product Launch",
                "Holiday Party"
            });

            MenuTypeCb.Items.AddRange(new string[]
            {
                "Buffet Style",
                "Plated/ Sit-down meal",
                "Packed Meals/ Bento Box",
                "Finger Food/ Cocktail Style",
                "Grazing Table",
                "Themed Menu (Customizable)",
                "Dessert Bar/ Sweet Table",
                "Breakfast/ Brunch setup"
            });

            EventTypeCb.DrawMode = DrawMode.OwnerDrawFixed;
            MenuTypeCb.DrawMode = DrawMode.OwnerDrawFixed;

            EventTypeCb.DrawItem += ComboBox_DrawItem;
            MenuTypeCb.DrawItem += ComboBox_DrawItem;
        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 5;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void InitializeSidebar()
        {
            sidebarPanel = new SidebarPanel("Add New");
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

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (e.Index < 0) return;

            string text = comboBox.Items[e.Index].ToString();
            bool isHovered = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            e.DrawBackground();

            using (SolidBrush brush = new SolidBrush(isHovered ? Color.FromArgb(88, 71, 56) : Color.FromArgb(241, 234, 218)))
            {
                e.Graphics.DrawString(text, comboBox.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void AddNew_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Handle panel painting if needed
        }

        private void EventTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle event type selection change
            // This can be used to update UI or perform validation based on selected event type
        }

        private void DashboardLbl_Click_1(object sender, EventArgs e)
        {
            var homeForm = new Home();
            homeForm.Show();
            this.Hide();
        }

        private void CalendarLbl_Click_1(object sender, EventArgs e)
        {
            var calendarForm = new Calendar();
            calendarForm.Show();
            this.Hide();
        }

        private void SpreadsheetsLbl_Click_1(object sender, EventArgs e)
        {
            var spreadsheetForm = new Spreadsheet();
            spreadsheetForm.Show();
            this.Hide();
        }

        private void AddnewLbl_Click_1(object sender, EventArgs e)
        {
            // Already on AddNew form, no action needed
        }

        private void LogOutBtn_Click_1(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Login());
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {
            // Handle panel painting if needed
        }

        private void CalculateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                decimal foodDrinks = decimal.Parse(FoodDrinksTb.Text);
                decimal labor = decimal.Parse(LaborTb.Text);
                decimal deco = decimal.Parse(DecoTb.Text);
                decimal rentals = decimal.Parse(RentalsTb.Text);
                decimal transpo = decimal.Parse(TranspoTb.Text);
                decimal misc = decimal.Parse(MiscTb.Text);

                decimal totalExpenses = foodDrinks + labor + deco + rentals + transpo + misc;
                TotalExpensesLbl.Text = totalExpenses.ToString("0.00");

                decimal estimatedBudget = decimal.Parse(EstBudgetTb.Text);
                if (totalExpenses < estimatedBudget)
                {
                    StatusLbl.Text = "Under Budget";
                    StatusLbl.ForeColor = Color.Green;
                }
                else if (totalExpenses > estimatedBudget)
                {
                    StatusLbl.Text = "Over Budget";
                    StatusLbl.ForeColor = Color.Red;
                }
                else
                {
                    StatusLbl.Text = "Exact Budget";
                    StatusLbl.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please enter valid numbers for all expense fields: " + ex.Message);
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            SqlTransaction transaction = null;
            try
            {
                con.Open();
                transaction = con.BeginTransaction();

                // Insert into EventTable (with corrected column names)
                string eventQuery = @"
                INSERT INTO EventTable (
                    EventTitle, EventType, EventDate, EventTime, Venue,
                    CustomerName, ContactNumber, EmailAddress, NumberOfGuests,
                    MenuType, MenuDetails, CustomerNotes, EstimatedBudget
                ) VALUES (
                    @EventTitle, @EventType, @EventDate, @EventTime, @Venue,
                    @CustomerName, @ContactNumber, @EmailAddress, @NumberOfGuests,
                    @MenuType, @MenuDetails, @CustomerNotes, @EstimatedBudget
                ); SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(eventQuery, con, transaction);

                // Add parameters with proper validation
                cmd.Parameters.AddWithValue("@EventTitle", EventTitleTb.Text.Trim());
                cmd.Parameters.AddWithValue("@EventType", EventTypeCb.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@EventDate", EventDateDtp.Value.Date);
                cmd.Parameters.AddWithValue("@EventTime", TimeSpan.Parse(EventTimeTb.Text));
                cmd.Parameters.AddWithValue("@Venue", VenueTb.Text.Trim());
                cmd.Parameters.AddWithValue("@CustomerName", CustomerNameTb.Text.Trim());
                cmd.Parameters.AddWithValue("@ContactNumber", ContactNumTb.Text.Trim());
                cmd.Parameters.AddWithValue("@EmailAddress", EmailAddressTb.Text.Trim());
                cmd.Parameters.AddWithValue("@NumberOfGuests", int.Parse(NumOfGuestsTb.Text));
                cmd.Parameters.AddWithValue("@MenuType", MenuTypeCb.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@MenuDetails", MenuDetailsTb.Text.Trim());
                cmd.Parameters.AddWithValue("@CustomerNotes", NotesTb.Text.Trim());
                cmd.Parameters.AddWithValue("@EstimatedBudget", decimal.Parse(EstBudgetTb.Text));

                // Execute and get EventID
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    transaction.Rollback();
                    MessageBox.Show("Failed to create event record.");
                    return;
                }
                int eventId = Convert.ToInt32(result);

                // Insert into ExpensesTable
                string expenseQuery = @"
                INSERT INTO ExpensesTable (
                    EventID, FoodBeverages, Labor, Decorations,
                    Rentals, Transportation, Miscellaneous,
                    TotalExpenses, BudgetStatus
                ) VALUES (
                    @EventID, @FoodBeverages, @Labor, @Decorations,
                    @Rentals, @Transportation, @Miscellaneous,
                    @TotalExpenses, @BudgetStatus
                )";

                cmd = new SqlCommand(expenseQuery, con, transaction);
                cmd.Parameters.AddWithValue("@EventID", eventId);
                cmd.Parameters.AddWithValue("@FoodBeverages", decimal.Parse(FoodDrinksTb.Text));
                cmd.Parameters.AddWithValue("@Labor", decimal.Parse(LaborTb.Text));
                cmd.Parameters.AddWithValue("@Decorations", decimal.Parse(DecoTb.Text));
                cmd.Parameters.AddWithValue("@Rentals", decimal.Parse(RentalsTb.Text));
                cmd.Parameters.AddWithValue("@Transportation", decimal.Parse(TranspoTb.Text));
                cmd.Parameters.AddWithValue("@Miscellaneous", decimal.Parse(MiscTb.Text));
                cmd.Parameters.AddWithValue("@TotalExpenses", decimal.Parse(TotalExpensesLbl.Text));
                cmd.Parameters.AddWithValue("@BudgetStatus", StatusLbl.Text);

                cmd.ExecuteNonQuery();
                transaction.Commit();

                MessageBox.Show("Event saved successfully!");

                var receipt = new Receipt(
                    EventTitleTb.Text,
                    EventDateDtp.Value,
                    EventTimeTb.Text,
                    EventTypeCb.SelectedItem?.ToString() ?? "",
                    VenueTb.Text,
                    CustomerNameTb.Text,
                    ContactNumTb.Text,
                    EmailAddressTb.Text,
                    int.Parse(NumOfGuestsTb.Text),
                    MenuTypeCb.SelectedItem?.ToString() ?? "",
                    MenuDetailsTb.Text,
                    decimal.Parse(EstBudgetTb.Text),
                    decimal.Parse(TotalExpensesLbl.Text),
                    eventId.ToString()
                );
                receipt.Show();

                ClearForm();
            }
            catch (FormatException)
            {
                transaction?.Rollback();
                MessageBox.Show("Please check your input formats.\nTime should be HH:mm, numbers should be valid.");
            }
            catch (SqlException ex)
            {
                transaction?.Rollback();
                MessageBox.Show($"Database error: {ex.Message}\nError code: {ex.Number}");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private bool ValidateInputs()
        {
            // Check required fields
            if (string.IsNullOrWhiteSpace(EventTitleTb.Text) ||
                EventTypeCb.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(EventTimeTb.Text) ||
                string.IsNullOrWhiteSpace(VenueTb.Text) ||
                string.IsNullOrWhiteSpace(CustomerNameTb.Text) ||
                string.IsNullOrWhiteSpace(ContactNumTb.Text) ||
                string.IsNullOrWhiteSpace(EmailAddressTb.Text) ||
                string.IsNullOrWhiteSpace(NumOfGuestsTb.Text) ||
                MenuTypeCb.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(MenuDetailsTb.Text) ||
                string.IsNullOrWhiteSpace(EstBudgetTb.Text) ||
                string.IsNullOrWhiteSpace(FoodDrinksTb.Text) ||
                string.IsNullOrWhiteSpace(LaborTb.Text) ||
                string.IsNullOrWhiteSpace(DecoTb.Text) ||
                string.IsNullOrWhiteSpace(RentalsTb.Text) ||
                string.IsNullOrWhiteSpace(TranspoTb.Text) ||
                string.IsNullOrWhiteSpace(MiscTb.Text))
            {
                MessageBox.Show("All fields are required!");
                return false;
            }

            // Validate Contact Number: must be numeric and 7-15 digits
            string contactNum = ContactNumTb.Text.Trim();
            if (!contactNum.All(char.IsDigit) || contactNum.Length < 7 || contactNum.Length > 15)
            {
                MessageBox.Show("Contact number must be numeric and between 7 and 15 digits.");
                return false;
            }

            // Validate specific formats
            if (!TimeSpan.TryParse(EventTimeTb.Text, out _))
            {
                MessageBox.Show("Please enter time in HH:mm format (e.g. 14:30)");
                return false;
            }

            if (!int.TryParse(NumOfGuestsTb.Text, out int guests) || guests <= 0)
            {
                MessageBox.Show("Number of guests must be a positive whole number");
                return false;
            }

            return true;
        }
        

        private void ClearForm()
        {
            EventTitleTb.Clear();
            EventTypeCb.SelectedIndex = -1;
            EventDateDtp.Value = DateTime.Now;
            EventTimeTb.Clear();
            VenueTb.Clear();
            CustomerNameTb.Clear();
            ContactNumTb.Clear();
            EmailAddressTb.Clear();
            NumOfGuestsTb.Clear();
            MenuTypeCb.SelectedIndex = -1;
            MenuDetailsTb.Clear();
            EstBudgetTb.Clear();
            FoodDrinksTb.Clear();
            LaborTb.Clear();
            DecoTb.Clear();
            RentalsTb.Clear();
            TranspoTb.Clear();
            MiscTb.Clear();
            NotesTb.Clear();
            TotalExpensesLbl.Text = "";
            StatusLbl.Text = "";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }
    }
}
