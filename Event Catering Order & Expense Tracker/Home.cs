using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class Home: Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }
        private void DashboardLbl_Click(object sender, EventArgs e)
        {
            Home homeForm = new Home();
            homeForm.Show();
            this.Hide();
        }

        private void CalendarLbl_Click(object sender, EventArgs e)
        {
            Calendar calendarForm = new Calendar();
            calendarForm.Show();
            this.Hide();
        }

        private void SpreadsheetsLbl_Click(object sender, EventArgs e)
        {
            Spreadsheet spreadsheetsForm = new Spreadsheet();
            spreadsheetsForm.Show();
            this.Hide();
        }

        private void AddnewLbl_Click(object sender, EventArgs e)
        {
            AddNew addNewForm = new AddNew();
            addNewForm.Show();
            this.Hide();
        }
    }
}
