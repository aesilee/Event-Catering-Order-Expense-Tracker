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
            this.TopMost = true; // Set form to always be on top

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
            EstBudgetTb.Text = $"₱{estBudget:N2}";
            TotalExpensesLbl.Text = $"₱{totalExpenses:N2}";
            EventIdLbl.Text = $"#{eventId}";
        }


        private void printBtn_Click_1(object sender, EventArgs e)
        {
            try
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
                File.WriteAllText(filePath, sb.ToString());

                // Ensure form stays on top for the message
                this.TopMost = true;

                // Show success message with file path
                MessageBox.Show(
                    this, // Set the owner form
                    $"Receipt has been successfully saved to:\n{filePath}",
                    "Download Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Close the form after successful download
                this.Close();
            }
            catch (Exception ex)
            {
                // Ensure form stays on top for the error message
                this.TopMost = true;

                MessageBox.Show(
                    this, // Set the owner form
                    $"Failed to save receipt: {ex.Message}",
                    "Download Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
