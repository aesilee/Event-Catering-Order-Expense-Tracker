using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection.Emit;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class Receipt : Form
    {
        public Receipt(
            string eventName,
            DateTime eventDate,
            string eventTime,
            string eventType,
            string venue,
            string customerName,
            string contactNum,
            string email,
            int numOfGuests,
            string menuType,
            string menuDetails,
            decimal estBudget,
            decimal totalExpenses,
            string eventId
        )
        {
            InitializeComponent();

            // Set the labels/textboxes with the received data
            EventNameLbl.Text = eventName;
            EventDateDtp.Text = eventDate.ToShortDateString();
            EventTimeTb.Text = eventTime;
            EventTypeCb.Text = eventType;
            VenueTb.Text = venue;
            CustomerNameTb.Text = customerName;
            ContactNumTb.Text = contactNum;
            EmailAddressTb.Text = email;
            NumOfGuestsTb.Text = numOfGuests.ToString();
            MenuTypeCb.Text = menuType;
            MenuDetailsTb.Text = menuDetails;
            EstBudgetTb.Text = estBudget.ToString("0.00");
            TotalExpensesLbl.Text = totalExpenses.ToString("0.00");
            EventIdLbl.Text = $"#{eventId}";
        }


        private void printBtn_Click(object sender, EventArgs e)
        {
            // Get the user's Downloads folder
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string fileName = $"EventReceipt_{EventIdLbl.Text.Replace("#", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(downloadsPath, fileName);

            // Build the receipt content
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("         Event Catering Order Receipt   ");
            sb.AppendLine("========================================");
            sb.AppendLine($"Event Name:         {EventNameLbl.Text}");
            sb.AppendLine($"Event Date:         {EventDateDtp.Text}");
            sb.AppendLine($"Event Time:         {EventTimeTb.Text}");
            sb.AppendLine($"Event Type:         {EventTypeCb.Text}");
            sb.AppendLine($"Venue:              {VenueTb.Text}");
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Customer Name:      {CustomerNameTb.Text}");
            sb.AppendLine($"Contact #:          {ContactNumTb.Text}");
            sb.AppendLine($"Email:              {EmailAddressTb.Text}");
            sb.AppendLine($"Number of Guests:   {NumOfGuestsTb.Text}");
            sb.AppendLine($"Menu/Meal Type:     {MenuTypeCb.Text}");
            sb.AppendLine($"Menu Details:       {MenuDetailsTb.Text}");
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Estimated Budget:   {EstBudgetTb.Text}");
            sb.AppendLine($"Total Expenses:     {TotalExpensesLbl.Text}");
            sb.AppendLine();
            sb.AppendLine("Thank you!");
            sb.AppendLine($"Event ID:           {EventIdLbl.Text}");

            // Write to file
            try
            {
                this.Close();
                File.WriteAllText(filePath, sb.ToString());
                MessageBox.Show($"Receipt saved to {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Only after the user closes the MessageBox, show AddNew and close this form
                // var addNewForm = new AddNew();
                // addNewForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
