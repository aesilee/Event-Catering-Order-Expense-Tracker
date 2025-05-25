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
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30"))
            {
                con.Open();
                string query = @"
                    SELECT 
                        e.EventTitle, e.EventType, e.EventDate, e.EventTime, e.Venue,
                        e.CustomerName, e.ContactNumber, e.NumberOfGuests, e.MenuType,
                        ex.FoodBeverages, ex.Labor, ex.Decorations, ex.Rentals, 
                        ex.Transportation, ex.Miscellaneous, ex.TotalExpenses, ex.BudgetStatus
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

                        // Display expenses if available
                        if (reader["TotalExpenses"] != DBNull.Value)
                        {
                            FoodDrinksLbl.Text = Convert.ToDecimal(reader["FoodBeverages"]).ToString("C");
                            LaborLbl.Text = Convert.ToDecimal(reader["Labor"]).ToString("C");
                            DecoLbl.Text = Convert.ToDecimal(reader["Decorations"]).ToString("C");
                            RentalsLbl.Text = Convert.ToDecimal(reader["Rentals"]).ToString("C");
                            TranspoLbl.Text = Convert.ToDecimal(reader["Transportation"]).ToString("C");
                            MiscLbl.Text = Convert.ToDecimal(reader["Miscellaneous"]).ToString("C");
                            TotalExpensesLbl.Text = Convert.ToDecimal(reader["TotalExpenses"]).ToString("C");
                        }
                    }
                }
            }
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
