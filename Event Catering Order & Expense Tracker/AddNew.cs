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
    public partial class AddNew: Form
    {
        private Color originalSidebarLabelForeColor = Color.White;
        private Color hoverSidebarLabelForeColor = Color.FromArgb(88, 71, 56);

        private Timer fadeTimer;
        private Form nextFormToOpen;

        public AddNew()
        {
            InitializeComponent();
            SetupSidebarLabels();

            InitializeFadeTimer();
            this.Opacity = 0.0; 


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

        //Color in combo boxes
        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (e.Index < 0) return;

            string text = comboBox.Items[e.Index].ToString();
            bool isHovered = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            e.DrawBackground();

            Color backColor = isHovered ? Color.FromArgb(88, 71, 56) : comboBox.BackColor;
            Color foreColor = isHovered ? Color.White : comboBox.ForeColor;

            using (SolidBrush bgBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(bgBrush, e.Bounds);

            using (SolidBrush fgBrush = new SolidBrush(foreColor))
                e.Graphics.DrawString(text, e.Font, fgBrush, e.Bounds.X, e.Bounds.Y);

            e.DrawFocusRectangle();
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

       

        private void AddNewEventBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void EventTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AddNew_Load(object sender, EventArgs e)
        {
            StartFadeIn(); 

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DashboardLbl_Click_1(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Home());

            //Home homeForm = new Home();
            //homeForm.Show();
            //this.Hide();
        }

        private void CalendarLbl_Click_1(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Calendar());

            //Calendar calendarForm = new Calendar();
            //calendarForm.Show();
            //this.Hide();
        }

        private void SpreadsheetsLbl_Click_1(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Spreadsheet());

            //Spreadsheet spreadsheetsForm = new Spreadsheet();
            //spreadsheetsForm.Show();
            //this.Hide();
        }

        private void AddnewLbl_Click_1(object sender, EventArgs e)
        {
            if (this.GetType() == typeof(AddNew))
            {
                return;
            }
            StartFadeOutAndNavigate(new AddNew());

        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void LogOutBtn_Click_1(object sender, EventArgs e)
        {
            StartFadeOutAndNavigate(new Login());

        }
    }
}
