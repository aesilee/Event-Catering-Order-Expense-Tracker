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
    public partial class Calendar : Form
    {
        private Timer fadeTimer;
        private Form nextFormToOpen;
        private SidebarPanel sidebarPanel;
        public int _month = DateTime.Now.Month;
        public int _year = DateTime.Now.Year;

        public Calendar()
        {
            InitializeComponent();
            InitializeFadeTimer();
            InitializeSidebar();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.TopMost = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.TopMost = false;
        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 5;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void InitializeSidebar()
        {
            sidebarPanel = new SidebarPanel("Calendar");
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

        private void Calendar_Load(object sender, EventArgs e)
        {
            showDays(DateTime.Now.Month, DateTime.Now.Year);
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void showDays(int month, int year)
        {
            flowLayoutPanel1.Controls.Clear();
            _year = year;
            _month = month;

            string monthName = new DateTimeFormatInfo().GetMonthName(month);
            lblmonthYear.Text = monthName.ToUpper() + " " + year;
            DateTime startOfTheMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int dayOfWeek = Convert.ToInt32(startOfTheMonth.DayOfWeek.ToString("d"));

            DateTime today = DateTime.Today;

            for (int i = 0; i < dayOfWeek; i++)
            {
                ucDays uc = new ucDays("", year, month);
                flowLayoutPanel1.Controls.Add(uc);
            }

            for (int i = 1; i <= daysInMonth; i++)
            {
                ucDays uc = new ucDays(i.ToString(), year, month);
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
            // Handle panel painting if needed
        }
    }
}
