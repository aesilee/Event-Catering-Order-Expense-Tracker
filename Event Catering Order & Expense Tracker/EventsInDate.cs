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
    public partial class EventsInDate: Form
    {
        private DateTime selectedDate;
        public EventsInDate(DateTime date)
        {
            InitializeComponent();
            selectedDate = date;
            DateLbl.Text = $"Events for {date.ToShortDateString()}";
            LoadEvents();
        }
        private void LoadEvents()
        {
            try
            {
                EventsFlowPanel.Controls.Clear();

                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))
                {
                    con.Open();
                    string query = "SELECT EventID, EventTitle FROM EventTable WHERE CONVERT(date, EventDate) = @EventDate";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventDate", selectedDate.Date);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Label noEvents = new Label
                            {
                                Text = "No events scheduled for this date",
                                AutoSize = true,
                                ForeColor = Color.FromArgb(88, 71, 56)
                            };
                            EventsFlowPanel.Controls.Add(noEvents);
                            return;
                        }

                        while (reader.Read())
                        {
                            int eventID = Convert.ToInt32(reader["EventID"]);
                            string title = reader["EventTitle"].ToString();

                            var eventItem = new EventItemControl(eventID, title);
                            eventItem.EventClicked += (sender, e) =>
                            {
                                Event eventForm = new Event(eventID);
                                eventForm.Show();
                                this.Hide();
                            };

                            EventsFlowPanel.Controls.Add(eventItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EventsInDate_Load(object sender, EventArgs e)
        {

        }

        private void EventTitleLbl_Click_1(object sender, EventArgs e)
        {

        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            Calendar calendarForm = new Calendar();
            calendarForm.Show();
            this.Close();
        }
    }
}
