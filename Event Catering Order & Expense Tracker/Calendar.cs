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
        // Sidebar hover colors
        private Color originalSidebarLabelForeColor = Color.White;
        private Color hoverSidebarLabelForeColor = Color.FromArgb(88, 71, 56);

        // Fade in/out timer
        private Timer fadeTimer;
        private Form nextFormToOpen;

        public static int _year, _month;
        public Calendar()
        {
            InitializeComponent();
            SetupSidebarLabels();

            InitializeFadeTimer();
            this.Opacity = 0.0; 

        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 5;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1.0 && nextFormToOpen == null)
            {
                this.Opacity += 0.10;
                if (this.Opacity >= 1.0)
                {
                    this.Opacity = 1.0;
                    fadeTimer.Stop();
                }
            }
            else if (this.Opacity > 0.0 && nextFormToOpen != null)
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
                        if (nextFormToOpen is Home homeForm) homeForm.StartFadeIn();
                        else if (nextFormToOpen is Calendar calendarForm) calendarForm.StartFadeIn();
                        else if (nextFormToOpen is Spreadsheet spreadsheetForm) spreadsheetForm.StartFadeIn();
                        else if (nextFormToOpen is AddNew addNewForm) addNewForm.StartFadeIn();
                        else if (nextFormToOpen is Login loginForm) loginForm.StartFadeIn();
                    }
                }
            }
        }
        public void StartFadeIn()
        {
            this.Opacity = 0.0;
            this.nextFormToOpen = null;
            fadeTimer.Start();
        }

        private void StartFadeOutAndNavigate(Form formToOpen)
        {
            this.nextFormToOpen = formToOpen;
            fadeTimer.Start();
        }

        private void SetupSidebarLabels()
        {
            Label[] sidebarLabels = { DashboardLbl, CalendarLbl, SpreadsheetsLbl, AddnewLbl };

            foreach (Label lbl in sidebarLabels)
            {
                lbl.ForeColor = originalSidebarLabelForeColor;
                lbl.MouseEnter += SidebarLabel_MouseEnter;
                lbl.MouseLeave += SidebarLabel_MouseLeave;
            }
        }
        private void SidebarLabel_MouseEnter(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl != null)
            {
                lbl.ForeColor = hoverSidebarLabelForeColor;
                lbl.Cursor = Cursors.Hand;
            }
        }

        private void SidebarLabel_MouseLeave(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl != null)
            {
                lbl.ForeColor = originalSidebarLabelForeColor;
                lbl.Cursor = Cursors.Default;
            }
        }

        private void Calendar_Load(object sender, EventArgs e)
        {
            showDays(DateTime.Now.Month, DateTime.Now.Year);
            StartFadeIn(); 

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

            StartFadeOutAndNavigate(new Home());

            //Home homeForm = new Home();
            //homeForm.Show();
            //this.Hide();
        }

        private void CalendarLbl_Click(object sender, EventArgs e)
        {
            if (this.GetType() == typeof(Calendar))
            {
                return;
            }
            StartFadeOutAndNavigate(new Calendar());

            //Calendar calendarForm = new Calendar();
            //calendarForm.Show();
            //this.Hide();
        }

        private void SpreadsheetsLbl_Click(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Spreadsheet());

            //Spreadsheet spreadsheetsForm = new Spreadsheet();
            //spreadsheetsForm.Show();
            //this.Hide();
        }

        private void AddnewLbl_Click(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new AddNew());

            //AddNew addNewForm = new AddNew();
            //addNewForm.Show();
            //this.Hide();
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

   

        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Login());

            //Login loginForm = new Login();
            //loginForm.Show();
            //this.Hide();
        }
    }
}
