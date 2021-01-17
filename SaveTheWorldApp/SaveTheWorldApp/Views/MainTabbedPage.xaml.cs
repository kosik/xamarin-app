using MediaManager;
using SaveTheWorldApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace SaveTheWorldApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : Xamarin.Forms.TabbedPage
    {
        MainViewModel ViewModel;
        public MainTabbedPage()
        {
            InitializeComponent();
            ViewModel = new MainViewModel(Navigation);
            BindingContext = ViewModel;
            
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.LoadData();
        }
    }
}