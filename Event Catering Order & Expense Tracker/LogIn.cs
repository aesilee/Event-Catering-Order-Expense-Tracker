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
    public partial class Login: Form
    {

        private Timer fadeTimer;
        private Form nextFormToOpen;
        private const string RememberMeFileName = "rememberme.dat";

//        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ashbs\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kyle\Documents\EventraDB.mdf;Integrated Security=True;Connect Timeout=30");

        public Login()
        {
            InitializeComponent();

            InitializeFadeTimer();
            this.Opacity = 0.0;

            LoadRememberedCredentials();
        }

        private void LoadRememberedCredentials()
        {
            try
            {
                if (System.IO.File.Exists(RememberMeFileName))
                {
                    string[] lines = System.IO.File.ReadAllLines(RememberMeFileName);
                    if (lines.Length >= 2)
                    {
                        UsernameTb.Text = lines[0];
                        PasswordTb.Text = lines[1];
                        RemMeChkB.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent fail - if we can't load remembered credentials, just continue normally
                Console.WriteLine("Error loading remembered credentials: " + ex.Message);
            }
        }

        private void SaveCredentials()
        {
            try
            {
                System.IO.File.WriteAllText(RememberMeFileName, $"{UsernameTb.Text}\n{PasswordTb.Text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving credentials: " + ex.Message);
            }
        }

        private void ClearSavedCredentials()
        {
            try
            {
                if (System.IO.File.Exists(RememberMeFileName))
                {
                    System.IO.File.Delete(RememberMeFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing credentials: " + ex.Message);
            }
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

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (UsernameTb.Text == "" || PasswordTb.Text == "")
            {
                MessageBox.Show("Please enter both username and password");
            }
            else
            {
                try
                {
                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM AccountsTable WHERE Username = '" + UsernameTb.Text + "' AND Password = '" + PasswordTb.Text + "'", con);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        if (RemMeChkB.Checked)
                        {
                            SaveCredentials();
                        }
                        else
                        {
                            ClearSavedCredentials();
                        }

                        StartFadeOutAndNavigate(new Home());
                    }
                
                    else
                    {
                        MessageBox.Show("Invalid username or password");
                    }
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
        

        private void SignUpLlbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SignUp signupForm = new SignUp();
            signupForm.Show();
            this.Hide();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            StartFadeIn(); 

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }
    }
}
