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
    public partial class Event: Form
    {
        private int eventID;

        public Event(int eventID)
        {
            InitializeComponent();
            this.eventID = eventID;
            LoadEventDetails();
        }
        private void LoadEventDetails()
        {
            //using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))

            {
                con.Open();
                string query = @"
                    SELECT 
                        e.EventTitle, e.EventType, e.EventDate, e.EventTime, e.Venue,
                        e.CustomerName, e.ContactNumber, e.NumberOfGuests, e.MenuType,
                        ex.FoodBeverages, ex.Labor, ex.Decorations, ex.Rentals, 
                        ex.Transportation, ex.Miscellaneous, ex.TotalExpenses, ex.BudgetStatus,
                        ex.PaymentStatus
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
                        EventDateLbl.Text = Convert.ToDateTime(reader["EventDate"]).ToShortDateString();
                        EventTimeLbl.Text = reader["EventTime"].ToString();
                        LocationLbl.Text = reader["Venue"].ToString();
                        CustomerNameLbl.Text = reader["CustomerName"].ToString();
                        CustomerNumLbl.Text = reader["ContactNumber"].ToString();
                        NumOfGuestsLbl.Text = reader["NumberOfGuests"].ToString();
                        MenuTypeLbl.Text = reader["MenuType"].ToString();

                        // Display payment status
                        string paymentStatus = reader["PaymentStatus"]?.ToString() ?? "Unpaid";
                        StatusLbl.Text = paymentStatus;
                        StatusLbl.ForeColor = paymentStatus == "Fully Paid" ? Color.Green : Color.Red;

                        // Display expenses if available
                        if (reader["TotalExpenses"] != DBNull.Value)
                        {
                            FoodDrinksLbl.Text = $"₱{Convert.ToDecimal(reader["FoodBeverages"]):N2}";
                            LaborLbl.Text = $"₱{Convert.ToDecimal(reader["Labor"]):N2}";
                            DecoLbl.Text = $"₱{Convert.ToDecimal(reader["Decorations"]):N2}";
                            RentalsLbl.Text = $"₱{Convert.ToDecimal(reader["Rentals"]):N2}";
                            TranspoLbl.Text = $"₱{Convert.ToDecimal(reader["Transportation"]):N2}";
                            MiscLbl.Text = $"₱{Convert.ToDecimal(reader["Miscellaneous"]):N2}";
                            TotalExpensesLbl.Text = $"₱{Convert.ToDecimal(reader["TotalExpenses"]):N2}";
                        }
                    }
                }
            }
        }

        private void PaymentBtn_Click(object sender, EventArgs e)
        {
            PaymentDetails paymentForm = new PaymentDetails(eventID);
            paymentForm.TopMost = true;
            paymentForm.Show();
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is EventsInDate)
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
