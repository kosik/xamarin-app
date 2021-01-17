
using Acr.UserDialogs;
using MediaManager;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using SaveTheWorldApp.Dependencies;
using SaveTheWorldApp.Helpers;
using SaveTheWorldApp.Models;
using SaveTheWorldApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SaveTheWorldApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public User CurrentUser { get; set; }
        public bool IsLoading = false;
        public ICommand GooglePayCommand { get; set; }
        public ICommand GoToFAQPageCommand { get; set; }
        public ICommand ActionsCommand { get; set; }
        public int TotalContribution { get => totalContribution; set { totalContribution = value; OnChanged("TotalContribution"); } }
        public ObservableCollection<Contributor> TopContributors { get; set; }
        private INavigation Navigation;
        public MainViewModel(INavigation Navigation)
        {
            this.Navigation = Navigation;
            DependencyService.Get<IStatusBarColor>().ChangeStatusBarColor(Color.White);
            TopContributors = new ObservableCollection<Contributor>();
            CurrentUser = new User();
            InitCommands();
        }

        public async Task LoadData()
        {
            if (!Constants.IsLoaded && !IsLoading)
            {
                IsLoading = true;
                UserDialogs.Instance.ShowLoading();
                var userLoaded = await LoadUserData();
                var contributorsLoaded = await LoadContributors();
                if (userLoaded && contributorsLoaded)
                    Constants.IsLoaded = true;
                UserDialogs.Instance.HideLoading();
                IsLoading = false;
            }
        }

        private async Task<bool> LoadContributors()
        {
            WebApiHelper helper = new WebApiHelper();
            var response = await helper.GetAsync<UserHistory[]>(Constants.BaseApiURL + "Payment/0");
            if (response != null && response.Length > 0)
            {
                TopContributors.Clear();
                TotalContribution = 0;
                var ordered = response.OrderByDescending(u => u.total_records);
                for (int i = 0; i < ordered.Count(); i++)
                {
                    TopContributors.Add(new Contributor
                    {
                        Rank = i + 1,
                        Name = ordered.ElementAt(i).name,
                        Contribution = ordered.ElementAt(i).total_records,
                    });
                    TotalContribution += ordered.ElementAt(i).total_records;
                }

                return true;
            }
            return false;
        }
        private async Task<bool> LoadUserData()
        {
            WebApiHelper helper = new WebApiHelper();
            var response = await helper.GetAsync<UserHistory[]>(Constants.BaseApiURL + $"Payment/{Preferences.Get(Constants.ID, "")}");
            if (response != null && response.Length > 0)
            {
                CurrentUser.Contribution = response[0].total_records;
                return true;
            }
            return false;
        }
        bool rotated = false;
        private int totalContribution;

        private void InitCommands()
        {
            GooglePayCommand = new Command(async (control) =>
              {
                  if (await InitPayment(PurchaseType.OneTime))
                      if (control is ImageButton btn)
                      {
                          if (!rotated)
                          {
                              await btn.RotateTo(360, 2000);
                              rotated = true;
                          }
                          else
                          {
                              await btn.RotateTo(0, 2000);
                              rotated = false;
                          }
                      }

              });
            GoToFAQPageCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new FAQsPage());
            });


            ActionsCommand = new Command(async () =>
            {
                var result = await UserDialogs.Instance.ActionSheetAsync("Select Subscription", "", "Logout", null, new string[] { "Weekly Subscription", "Monthly Subscription" });
                if (result.Equals("Weekly Subscription"))
                {
                    await InitPayment(PurchaseType.Weekly);
                }
                else if (result.Equals("Monthly Subscription"))
                {
                    await InitPayment(PurchaseType.Monthly);
                }
                else if (result.Equals("Logout"))
                {
                    Preferences.Clear();
                    ((App)Application.Current).GoToLogin();
                }
            });
        }

        private async Task<bool> InitPayment(PurchaseType type)
        {

            string CheckUrl = "http://google.com";

            try
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);

                iNetRequest.Timeout = 5000;

                WebResponse iNetResponse = iNetRequest.GetResponse();

                Console.WriteLine("...connection established..." + iNetRequest.ToString());
                iNetResponse.Close();


            }
            catch (WebException ex)
            {
                //await DisplayAlert("Error", "no connection internet", "cancel");
                Console.WriteLine(".....no connection..." + ex.ToString());

                return false;


            }
            return await PurchaseItem("android.test.purchased", "payloada");

        }

        public async Task<bool> PurchaseItem(string productId, string payload)
        {
            var billing = CrossInAppBilling.Current;
            try
            {
                var connected = await billing.ConnectAsync(ItemType.InAppPurchase);
                if (!connected)
                {
                    //await DisplayAlert("Error", "Couldn't connect to billing", "cancel");

                    return false;
                }

                var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchase, payload);

                if (purchase == null)
                {
                    //await DisplayAlert("Error", "Not purchased", "cancel");
                }
                else if (purchase.State == PurchaseState.Purchased)
                {
                    if (Device.RuntimePlatform == Device.iOS)
                        return true;

                    var consumedItem = await CrossInAppBilling.Current.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken);

                    if (consumedItem != null)
                    {
                        //Consumed!!
                    }
                    Constants.IsLoaded = false;
                    await SaveRecord();
                    PlayAnimationVideo();
                    return true;
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                switch (purchaseEx.PurchaseError)
                {
                    case PurchaseError.BillingUnavailable:
                        UserDialogs.Instance.Alert("Billing Unavailable, please try again later");
                        break;
                    case PurchaseError.DeveloperError:
                        break;
                    case PurchaseError.ItemUnavailable:
                        UserDialogs.Instance.Alert("Item Unavailable, please try again later");
                        break;
                    case PurchaseError.GeneralError:
                        UserDialogs.Instance.Alert("Something went wrong, please try again later");
                        break;
                    case PurchaseError.UserCancelled:
                        UserDialogs.Instance.Alert("Payment was cancelled");
                        break;
                    case PurchaseError.AppStoreUnavailable:
                        UserDialogs.Instance.Alert("Store Unavailable, please try again later");
                        break;
                    case PurchaseError.PaymentNotAllowed:
                        UserDialogs.Instance.Alert("Payment not allowed, please try again later");
                        break;
                    case PurchaseError.PaymentInvalid:
                        UserDialogs.Instance.Alert("Payment Invalid, please try again later");
                        break;
                    case PurchaseError.InvalidProduct:
                        UserDialogs.Instance.Alert("Invalid Product, please try again later");
                        break;
                    case PurchaseError.ProductRequestFailed:
                        UserDialogs.Instance.Alert("Request failed, please try again later");
                        break;
                    case PurchaseError.RestoreFailed:
                        UserDialogs.Instance.Alert("Billing Unavailable, please try again later");
                        break;
                    case PurchaseError.ServiceUnavailable:
                        UserDialogs.Instance.Alert("Service Unavailable, please try again later");
                        break;
                    case PurchaseError.AlreadyOwned:
                        UserDialogs.Instance.Alert("Already purchased");
                        break;
                    case PurchaseError.NotOwned:
                        UserDialogs.Instance.Alert("Billing Unavailable, please try again later");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Issue connecting: " + ex);
            }
            finally
            {
                await billing.DisconnectAsync();
            }
            return false;
        }

        private async Task SaveRecord()
        {
            WebApiHelper helper = new WebApiHelper();
            var request = new
            {
                card_type = "",
                card_number = "",
                cvv = "",
                expiry_date = "",
                amount = "3.50",
                user_id = CurrentUser.ID,
                ref_id = ""
            };
            var response = await helper.PostAsync<BaseResponse[]>(Constants.BaseApiURL + "Payment",
                request);
        }

        private void PlayAnimationVideo()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                CrossMediaManager.Current.RepeatMode = MediaManager.Playback.RepeatMode.Off;
                await CrossMediaManager.Current.Play("https://buttonvideo.blob.core.windows.net/myshare/white%20backgrond%20final.mp4");
            });
        }
    }
}
