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
    public partial class Login: Form
    {

        private Timer fadeTimer;
        private Form nextFormToOpen;
        public Login()
        {
            InitializeComponent();

            InitializeFadeTimer();
            this.Opacity = 0.0; 
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
            StartFadeOutAndNavigate(new Home());

            //Home homeForm = new Home();
            //homeForm.Show();
            //this.Hide();
        }

        private void SignupBtn_Click(object sender, EventArgs e)
        {
            SignUp signupForm = new SignUp();
            signupForm.Show();
            this.Hide();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            StartFadeIn(); 

        }
    }
}
