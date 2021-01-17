using System;
using System.Collections.Generic;
using System.Text;
using SaveTheWorldApp.Views;
using System.Windows.Input;
using Xamarin.Forms;
using SaveTheWorldApp.Dependencies;
using Acr.UserDialogs;
using System.Threading.Tasks;
using SaveTheWorldApp.Helpers;
using SaveTheWorldApp.Models;
using Xamarin.Essentials;

namespace SaveTheWorldApp.ViewModels
{
    class StartupViewModel : BaseViewModel
    {
        private bool isLogin;
        public string Email { get { if (!string.IsNullOrEmpty(email)) return email.Trim(); else return email; } set => email = value; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsLogin
        {
            get { return isLogin; }
            set
            {
                isLogin = value;
                OnChanged("IsLogin");
                if (isLogin)
                    ButtonText = "Login";
                else
                    ButtonText = "Register";
            }
        }

        private string buttonText;

        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; OnChanged("ButtonText"); }
        }

        public ICommand GoToLoginCommand { get; set; }
        public ICommand GoToRegistrationCommand { get; set; }

        private INavigation Navigation;
        private string email;

        public StartupViewModel(INavigation Navigation)
        {
            this.Navigation = Navigation;
            IsLogin = true;
            DependencyService.Get<IStatusBarColor>().ChangeStatusBarColor(Color.Transparent);
            InitCommands();
        }

        private void InitCommands()
        {
            GoToLoginCommand = new Command(async () =>
            {
                if (IsLogin)
                {
                    if (ValidateLogin())
                    {
                        await PerformLoginOperation();
                    }
                }
                else
                {
                    if (ValidateRegisteration())
                    {
                        await PerformRegistrationOperation();
                    }
                }
            });
            GoToRegistrationCommand = new Command(() =>
            {
                IsLogin = !IsLogin;
            });
        }

        private async Task PerformRegistrationOperation()
        {
            WebApiHelper helper = new WebApiHelper();
            var request = new
            {
                name = Name,
                email = Email,
                password = Password
            };
            var response = await helper.PostAsync<LoginResponse[]>(Constants.BaseApiURL + "Registration",
                request);
            if (response != null && response.Length > 0)
            {
                Preferences.Set(Constants.ID, response[0].id);
                Preferences.Set(Constants.EMAIL, Email);
                Preferences.Set(Constants.NAME, response[0].name);
                Preferences.Set(Constants.PASSWORD, Password);
                await Navigation.PushAsync(new MainTabbedPage());
            }
        }

        private async Task PerformLoginOperation()
        {
            WebApiHelper helper = new WebApiHelper();
            var response = await helper.GetAsync<LoginResponse[]>(Constants.BaseApiURL + $"login/{Email}/{Password}");
            if (response != null && response.Length > 0)
            {
                Preferences.Set(Constants.ID, response[0].id);
                Preferences.Set(Constants.EMAIL, Email);
                Preferences.Set(Constants.NAME, response[0].name);
                Preferences.Set(Constants.PASSWORD, Password);
                await Navigation.PushAsync(new MainTabbedPage());
            }
        }

        private bool ValidateRegisteration()
        {
            if (!ValidateLogin())
            {
                return false;
            }
            if (string.IsNullOrEmpty(Name))
            {
                UserDialogs.Instance.Alert("Please provide full name");
                return false;
            }
            return true;
        }

        private bool ValidateLogin()
        {
            if (string.IsNullOrEmpty(Email) || !IsValidEmail(Email))
            {
                UserDialogs.Instance.Alert("Enter correct email address");
                return false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                UserDialogs.Instance.Alert("Password can not be empty");
                return false;
            }
            if (Password.Length < 6)
            {
                UserDialogs.Instance.Alert("Password length must be atleast 6");
                return false;
            }
            return true;
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
