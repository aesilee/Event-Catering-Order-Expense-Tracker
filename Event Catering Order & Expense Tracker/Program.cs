using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;   
using System.Windows.Forms;

namespace Event_Catering_Order___Expense_Tracker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
