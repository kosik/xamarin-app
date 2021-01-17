using Android.OS;
using Plugin.CurrentActivity;
using SaveTheWorldApp.Dependencies;
using SaveTheWorldApp.Droid.DependencyImplementations;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(StatusBarColorImplementation))]
namespace SaveTheWorldApp.Droid.DependencyImplementations
{
    public class StatusBarColorImplementation : IStatusBarColor
    {
        public void ChangeStatusBarColor(Color color)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var androidColor = color.AddLuminosity(-0.1).ToAndroid();
                //Just use the plugin
                CrossCurrentActivity.Current.Activity.Window.SetStatusBarColor(androidColor);
            }
        }
    }
}