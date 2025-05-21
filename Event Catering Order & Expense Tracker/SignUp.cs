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
    public partial class SignUp: Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }
        private void SubmitBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Account Created Successfully");

            this.Hide();
            Login loginForm = new Login();
            loginForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
