using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class Calendar: Form
    {
        public static int _year, _month;
        public Calendar()
        {
            InitializeComponent();
        }

        private void Calendar_Load(object sender, EventArgs e)
        {
            showDays(DateTime.Now.Month, DateTime.Now.Year);
        }

        private void showDays(int month, int year)
        {
            flowLayoutPanel1.Controls.Clear();
            _year = year;
            _month = month;

            string monthName = new DateTimeFormatInfo().GetMonthName(month);
            monthLbl.Text = monthName.ToUpper() + " " + year;
            DateTime startOfTheMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int dayOfWeek = Convert.ToInt32(startOfTheMonth.DayOfWeek.ToString("d"));

            DateTime today = DateTime.Today;

            for (int i = 0; i < dayOfWeek; i++)
            {
                ucDays uc = new ucDays("");
                flowLayoutPanel1.Controls.Add(uc);
            }

            for (int i = 1; i <= daysInMonth; i++)
            {
                ucDays uc = new ucDays(i.ToString());
                if (i == today.Day && month == today.Month && year == today.Year)
                {
                    uc.BackColor = Color.FromArgb(88, 71, 56);
                    uc.ForeColor = Color.White;
                }
                else
                {
                    uc.BackColor = Color.FromArgb(255, 255, 255);
                    uc.ForeColor = Color.FromArgb(88, 71, 56);
                }
                flowLayoutPanel1.Controls.Add(uc);
            }
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

        private void prevBtn_Click(object sender, EventArgs e)
        {
            _month -= 1;
            if (_month < 1)
            {
                _month = 12;
                _year -= 1;
            }
            showDays(_month, _year);
        }

        private void nextbtn_Click(object sender, EventArgs e)
        {
            _month += 1;
            if (_month > 12)
            {
                _month = 1;
                _year += 1;
            }
            showDays(_month, _year);
        }

        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }
    }
}
