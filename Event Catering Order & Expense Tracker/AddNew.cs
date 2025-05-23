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
    public partial class AddNew : Form
    {
        private Timer fadeTimer;
        private Form nextFormToOpen;
        private SidebarPanel sidebarPanel;

        public AddNew()
        {
            InitializeComponent();
            InitializeFadeTimer();
            InitializeSidebar();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.TopMost = true;

            EventTypeCb.Items.AddRange(new string[]
            {
                "Birthday Party",
                "Wedding Reception",
                "Graduation Party",
                "Corporate Event",
                "Awarding/Recognition Night",
                "Engagement Party",
                "Christening/Baptism",
                "Baby Shower",
                "Family Reunion",
                "Product Launch",
                "Holiday Party"
            });

            MenuTypeCb.Items.AddRange(new string[]
            {
                "Buffet Style",
                "Plated/ Sit-down meal",
                "Packed Meals/ Bento Box",
                "Finger Food/ Cocktail Style",
                "Grazing Table",
                "Themed Menu (Customizable)",
                "Dessert Bar/ Sweet Table",
                "Breakfast/ Brunch setup"
            });

            EventTypeCb.DrawMode = DrawMode.OwnerDrawFixed;
            MenuTypeCb.DrawMode = DrawMode.OwnerDrawFixed;

            EventTypeCb.DrawItem += ComboBox_DrawItem;
            MenuTypeCb.DrawItem += ComboBox_DrawItem;
        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 5;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void InitializeSidebar()
        {
            sidebarPanel = new SidebarPanel("Add New");
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

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (e.Index < 0) return;

            string text = comboBox.Items[e.Index].ToString();
            bool isHovered = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            e.DrawBackground();

            using (SolidBrush brush = new SolidBrush(isHovered ? Color.FromArgb(88, 71, 56) : Color.FromArgb(241, 234, 218)))
            {
                e.Graphics.DrawString(text, comboBox.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void AddNew_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Handle panel painting if needed
        }

        private void EventTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle event type selection change
            // This can be used to update UI or perform validation based on selected event type
        }

        private void DashboardLbl_Click_1(object sender, EventArgs e)
        {
            var homeForm = new Home();
            homeForm.Show();
            this.Hide();
        }

        private void CalendarLbl_Click_1(object sender, EventArgs e)
        {
            var calendarForm = new Calendar();
            calendarForm.Show();
            this.Hide();
        }

        private void SpreadsheetsLbl_Click_1(object sender, EventArgs e)
        {
            var spreadsheetForm = new Spreadsheet();
            spreadsheetForm.Show();
            this.Hide();
        }

        private void AddnewLbl_Click_1(object sender, EventArgs e)
        {
            // Already on AddNew form, no action needed
        }

        private void LogOutBtn_Click_1(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Login());
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {
            // Handle panel painting if needed
        }

        private void AddNewEventBtn_Click(object sender, EventArgs e)
        {
            // Handle adding new event
            // This will be implemented later
        }
    }
}
