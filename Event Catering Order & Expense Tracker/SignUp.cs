using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class SignUp: Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");

        private void SignUpBtn_Click(object sender, EventArgs e)
        {
            if (FirstNameTb.Text == "" || LastNameTb.Text == "" || UsernameTb.Text == "" || EmailTb.Text == "" || PasswordTb.Text == "" || ConfirmPassTb.Text == "")
            {
                MessageBox.Show("Please fill in all fields");
            }
            else if (PasswordTb.Text != ConfirmPassTb.Text)
            {
                MessageBox.Show("Passwords do not match");
            }
            else
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO AccountsTable (FirstName, LastName, Username, Email, Password) VALUES (@FN, @LN, @UN, @EM, @PW)", con);
                    cmd.Parameters.AddWithValue("@FN", FirstNameTb.Text);
                    cmd.Parameters.AddWithValue("@LN", LastNameTb.Text);
                    cmd.Parameters.AddWithValue("@UN", UsernameTb.Text);
                    cmd.Parameters.AddWithValue("@EM", EmailTb.Text);
                    cmd.Parameters.AddWithValue("@PW", PasswordTb.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Account Created Successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void LogInLlbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }
    }
}
