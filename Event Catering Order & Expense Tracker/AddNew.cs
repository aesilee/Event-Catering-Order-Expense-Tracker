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
        public AddNew()
        {
            InitializeComponent();

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

        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }

        private void AddNewEventBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void EventTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
