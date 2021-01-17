using SaveTheWorldApp.Helpers;
using SaveTheWorldApp.ViewModels;
using Xamarin.Essentials;

namespace SaveTheWorldApp.Models
{
    public class User : BaseViewModel
    {
        private int contribution;

        public string ID { get; set; } = Preferences.Get(Constants.ID, "");
        public string Name { get; set; } = Preferences.Get(Constants.NAME, "");

        public string Email { get; set; } = Preferences.Get(Constants.EMAIL, "");

        public int Contribution
        {
            get => contribution; set
            {
                contribution = value;
                OnChanged("Contribution");
            }
        }
    }
}
