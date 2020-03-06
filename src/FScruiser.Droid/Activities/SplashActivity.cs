using Android.App;
using Android.OS;

namespace FScruiser.Droid.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", Label = "FScruiser", Icon = "@drawable/fscruiser_32dp", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartActivity(typeof(MainActivity));
        }
    }
}