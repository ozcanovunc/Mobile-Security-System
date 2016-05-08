using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Mobile_Security_System
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If logged in, navigate to main page
            if (!(App.RoamingSettings.Values["Loggedin"] == null || 
                App.RoamingSettings.Values["Loggedin"].Equals(false)))
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private async void LoginPage_SigninButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            try
            {
                bool userExists = await Controllers.UserController.GetUser
                    (LoginPage_MailText.Text, LoginPage_PasswordText.Password);

                if (!userExists)
                {
                    MessageBox.Show("User could not be found!");
                }
                else
                {
                    // Navigate to main page
                    App.RoamingSettings.Values["Loggedin"] = true;
                    App.RoamingSettings.Values["Emailactivate"] = true;
                    App.RoamingSettings.Values["Email"] = LoginPage_MailText.Text;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
            catch
            {
                MessageBox.Show("Check your internet connection!");
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private async void LoginPage_SignupButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            try
            {
                bool added = await Controllers.UserController.AddUser
                    (LoginPage_MailText.Text, LoginPage_PasswordText.Password);

                if (!added)
                {
                    MessageBox.Show("User already exists!");
                }
                else
                {
                    // Navigate to main page
                    App.RoamingSettings.Values["Loggedin"] = true;
                    App.RoamingSettings.Values["Emailactivate"] = true;
                    App.RoamingSettings.Values["Email"] = LoginPage_MailText.Text;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
            catch
            {
                MessageBox.Show("Check your internet connection!");
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }
}