using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class ucDays: UserControl
    {
        string _day, date;
        DateTime currentDate;
        int _year, _month;

        private void panel1_click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                checkBox1.Checked = true;
                this.BackColor = Color.FromArgb(170, 163, 150);
            }
            else
            {
                checkBox1.Checked = false;
                this.BackColor = Color.FromArgb(255, 255, 255);
            }
        }

        public ucDays(string day, int year, int month)
        {
            InitializeComponent();
            _day = day;
            _year = year;
            _month = month;
            label1.Text = _day;
            checkBox1.Hide();

            if (!string.IsNullOrEmpty(_day)) 
            {
                currentDate = new DateTime(_year, _month, int.Parse(_day));
                date = _month + "/" + _day + "/" + _year;
            }
            label1.ForeColor = Color.FromArgb(88, 71, 56);
        }

        public void HighlightToday()
        {
            this.BackColor = Color.FromArgb(170, 163, 150);
            label1.Font = new Font(label1.Font, FontStyle.Bold);
        }

        private void ucDays_Load(object sender, EventArgs e)
        {

        }
    }
}
