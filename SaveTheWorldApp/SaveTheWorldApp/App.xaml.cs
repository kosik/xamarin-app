using SaveTheWorldApp.Helpers;
using SaveTheWorldApp.ViewModels;
using SaveTheWorldApp.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SaveTheWorldApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

        }

        protected override void OnStart()
        {
            if (string.IsNullOrEmpty(Preferences.Get(Constants.ID, "")))
                GoToLogin();
            else
                GoToMainPage();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        internal void GoToLogin()
        {
            MainPage = new NavigationPage(new StartupPage());
        }
        internal void GoToMainPage()
        {
            MainPage = new NavigationPage(new MainTabbedPage());
        }
    }
}
