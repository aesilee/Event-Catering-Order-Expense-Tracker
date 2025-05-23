using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_Catering_Order___Expense_Tracker
{
    public class EventData
    {
        public string EventTitle { get; set; }
        public string EventType { get; set; }
        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public string Location { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public string MenuType { get; set; }
    }
}
