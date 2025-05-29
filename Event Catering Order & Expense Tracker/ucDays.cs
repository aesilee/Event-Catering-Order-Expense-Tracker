using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class ucDays: UserControl
    {
        string _day, date;
        DateTime currentDate;
        int _year, _month;
        private List<string> eventTitles = new List<string>();

        public ucDays(string day, int year, int month)
        {
            InitializeComponent();
            _day = day;
            _year = year;
            _month = month;
            label1.Text = _day;

            if (!string.IsNullOrEmpty(_day)) 
            {
                currentDate = new DateTime(_year, _month, int.Parse(_day));
                date = _month + "/" + _day + "/" + _year;
                LoadEventsForDate();
            }
            label1.ForeColor = Color.FromArgb(88, 71, 56);
        }
        private void LoadEventsForDate()
        {
            eventTitles.Clear();

            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))
            //using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))

            {
                con.Open();
                string query = "SELECT EventTitle FROM EventTable WHERE CONVERT(date, EventDate) = @EventDate AND Hidden = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventDate", currentDate.Date);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        eventTitles.Add(reader["EventTitle"].ToString());
                    }
                }
            }

            if (eventTitles.Count > 0)
            {
                StringBuilder eventsText = new StringBuilder();
                foreach (string title in eventTitles)
                {
                    eventsText.AppendLine(title);
                }
                EventsLabel.Text = eventsText.ToString();
                EventsLabel.Visible = true;
                EventsLabel.ForeColor = Color.FromArgb(88, 71, 56);
                EventsLabel.BackColor = Color.Transparent;
            }
            else
            {
                EventsLabel.Visible = false;
            }
        }

        private void panel1_click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_day))
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))
                //using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))

                {
                    con.Open();
                    string query = @"SELECT COUNT(*) FROM EventTable 
                                   WHERE CONVERT(date, EventDate) = @EventDate
                                   AND Hidden = 0";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventDate", currentDate.Date);

                    int eventCount = (int)cmd.ExecuteScalar();

                    if (eventCount > 0)
                    {
                        EventsInDate eventsInDateForm = new EventsInDate(currentDate);
                        eventsInDateForm.Show();
                        ((Form)this.TopLevelControl).Hide();
                    }
                    else
                    {
                        MessageBox.Show("No events scheduled for this date", "Information",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void HighlightToday()
        {
            this.BackColor = Color.FromArgb(170, 163, 150);
            label1.Font = new Font(label1.Font, FontStyle.Bold);

            if (eventTitles.Count > 0)
            {
                EventsLabel.ForeColor = Color.FromArgb(88, 71, 56);
                EventsLabel.BackColor = Color.Transparent;
            }
        }

        private void ucDays_Load(object sender, EventArgs e)
        {

        }
    }
}
