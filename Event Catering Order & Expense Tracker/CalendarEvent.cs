using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class CalendarEvent: Form
    {
        private string _eventDate;
        private EventData _eventData;

        public CalendarEvent(string date)
        {
            InitializeComponent();
            _eventDate = date;
        }
        public CalendarEvent(EventData eventData)
        {
            InitializeComponent();
            _eventData = eventData;
            DisplayEventData();
        }
        private void DisplayEventData()
        {
            if (_eventData != null)
            {
                EventTitleLbl.Text = _eventData.EventTitle;
                EventTypeLbl.Text = _eventData.EventType;
                EventDateLbl.Text = _eventData.EventDate;
                EventTimeLbl.Text = _eventData.EventTime;
                EventLocationLbl.Text = _eventData.Location;
                CustomerNameLbl.Text = _eventData.CustomerName;
                ContactNumberLbl.Text = _eventData.ContactNumber;
                NumOfGuestsLbl.Text = _eventData.NumberOfGuests.ToString();
                MenuTypeLbl.Text = _eventData.MenuType;
            }
        }

        private void CalendarEvent_Load(object sender, EventArgs e)
        {

        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            Calendar calendarForm = new Calendar();
            calendarForm.Show();
            this.Hide();
        }
    }
}
