using SaveTheWorldApp.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SaveTheWorldApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartupPage : ContentPage
    {
        public StartupPage()
        {
            InitializeComponent();
            BindingContext = new StartupViewModel(Navigation);
        }
    }
}