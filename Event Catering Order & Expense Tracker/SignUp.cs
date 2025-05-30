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
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");
        //SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");

        public SignUp()
        {
            InitializeComponent();

            SignUpBtn.MouseEnter += SignUpBtn_MouseEnter;
            SignUpBtn.MouseLeave += SignUpBtn_MouseLeave;

            PasswordTb.PasswordChar = '•';
            ConfirmPassTb.PasswordChar = '•';

            
            ShowPassBtn1.MouseDown += ShowPassBtn1_MouseDown;
            ShowPassBtn1.MouseUp += ShowPassBtn1_MouseUp;
            ShowPassBtn2.MouseDown += ShowPassBtn2_MouseDown;
            ShowPassBtn2.MouseUp += ShowPassBtn2_MouseUp;
        }
        private void ShowPassBtn1_MouseDown(object sender, MouseEventArgs e)
        {
            PasswordTb.PasswordChar = '\0'; 
        }

        private void ShowPassBtn1_MouseUp(object sender, MouseEventArgs e)
        {
            PasswordTb.PasswordChar = '•'; 
        }

        private void ShowPassBtn2_MouseDown(object sender, MouseEventArgs e)
        {
            ConfirmPassTb.PasswordChar = '\0'; 
        }

        private void ShowPassBtn2_MouseUp(object sender, MouseEventArgs e)
        {
            ConfirmPassTb.PasswordChar = '•'; 
        }

        private async Task AnimateButtonColors(Button button, Color targetBackColor, Color targetForeColor)
        {
            Color originalBackColor = button.BackColor;
            Color originalForeColor = button.ForeColor;

            for (int i = 0; i <= 10; i++)
            {
                if (button.IsDisposed) return;

                int backR = originalBackColor.R + (int)((targetBackColor.R - originalBackColor.R) * (i / 10f));
                int backG = originalBackColor.G + (int)((targetBackColor.G - originalBackColor.G) * (i / 10f));
                int backB = originalBackColor.B + (int)((targetBackColor.B - originalBackColor.B) * (i / 10f));

                int foreR = originalForeColor.R + (int)((targetForeColor.R - originalForeColor.R) * (i / 10f));
                int foreG = originalForeColor.G + (int)((targetForeColor.G - originalForeColor.G) * (i / 10f));
                int foreB = originalForeColor.B + (int)((targetForeColor.B - originalForeColor.B) * (i / 10f));

                button.BackColor = Color.FromArgb(backR, backG, backB);
                button.ForeColor = Color.FromArgb(foreR, foreG, foreB);

                await Task.Delay(15);
            }
        }

        private async void SignUpBtn_MouseEnter(object sender, EventArgs e)
        {
            await AnimateButtonColors(SignUpBtn,
                Color.FromArgb(170, 163, 150),
                Color.Black);
        }

        private async void SignUpBtn_MouseLeave(object sender, EventArgs e)
        {
            await AnimateButtonColors(SignUpBtn,
                Color.FromArgb(88, 71, 56),
                Color.White);
        }

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
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM AccountsTable WHERE Username COLLATE SQL_Latin1_General_CP1_CS_AS = @UN", con);
                    checkCmd.Parameters.AddWithValue("@UN", UsernameTb.Text);
                    int userCount = (int)checkCmd.ExecuteScalar();

                    if (userCount > 0)
                    {
                        MessageBox.Show("Username already exists");
                        return;
                    }

                    SqlCommand cmd = new SqlCommand("INSERT INTO AccountsTable (FirstName, LastName, Username, Email, Password) VALUES (@FN, @LN, @UN, @EM, @PW)", con);
                    cmd.Parameters.AddWithValue("@FN", FirstNameTb.Text);
                    cmd.Parameters.AddWithValue("@LN", LastNameTb.Text);
                    cmd.Parameters.AddWithValue("@UN", UsernameTb.Text);
                    cmd.Parameters.AddWithValue("@EM", EmailTb.Text);
                    cmd.Parameters.AddWithValue("@PW", PasswordTb.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Account Created Successfully");

                    FirstNameTb.Clear();
                    LastNameTb.Clear();
                    UsernameTb.Clear();
                    EmailTb.Clear();
                    PasswordTb.Clear();
                    ConfirmPassTb.Clear();
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }
    }
}
