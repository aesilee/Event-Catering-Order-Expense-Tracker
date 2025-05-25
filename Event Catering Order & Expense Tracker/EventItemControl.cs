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
    public partial class EventItemControl: UserControl
    {
        public int EventID { get; set; }
        public string EventTitle { get; set; }

        public event EventHandler<EventArgs> EventClicked;

        public EventItemControl(int eventId, string title)
        {
            InitializeComponent();
            EventID = eventId;
            EventTitle = title;
            SetupControl();
        }

        private void SetupControl()
        {
            this.BackColor = Color.FromArgb(241, 234, 218);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Width = 355;
            this.Height = 35;
            this.Margin = new Padding(5);
            this.Cursor = Cursors.Hand;

            Label titleLabel = new Label
            {
                Text = EventTitle,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(88, 71, 56),
                Padding = new Padding(10, 0, 0, 0)
            };

            titleLabel.Click += (s, e) => EventClicked?.Invoke(this, EventArgs.Empty);
            this.Click += (s, e) => EventClicked?.Invoke(this, EventArgs.Empty);

            this.Controls.Add(titleLabel);
        }
    }
}
