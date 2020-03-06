using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Android.Runtime;
using FScruiser.XF;
using Plugin.Permissions;
using Xamarin.Forms.Platform.Android;
using Android.Views;

namespace FScruiser.Droid
{
    [Activity(Label = "FScruiser", Icon = "@drawable/fscruiser_32dp", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    

    //[IntentFilter(new[] { Intent.ActionView },
    //    DataScheme = "file",
    //    DataMimeType = "*/*",
    //    // for the DataPathPatterns we need to specify many addition patters because of an unfortunate limitation in androids pattern matching. See https://stackoverflow.com/questions/3400072/pathpattern-to-match-file-extension-does-not-work-if-a-period-exists-elsewhere-i/8599921
    //    DataPathPatterns = new[] { ".*\\\\.cruise", ".*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\..*\\\\.cruise" },
    //    DataHost = "*",
    //    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //    Icon = "@drawable/fscruiser_32dp")]

    //[IntentFilter(new[] { Intent.ActionView },
    //    DataScheme = "file",
    //    // for the DataPathPatterns we need to specify many addition patters because of an unfortunate limitation in androids pattern matching. See https://stackoverflow.com/questions/3400072/pathpattern-to-match-file-extension-does-not-work-if-a-period-exists-elsewhere-i/8599921
    //    DataPathPatterns = new[] { ".*\\\\.cruise", ".*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\..*\\\\.cruise" },
    //    DataHost = "*",
    //    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //    Icon = "@drawable/fscruiser_32dp")]

    //[IntentFilter(new[] { Intent.ActionView },
    //    DataScheme = "file",
    //    DataMimeType = "application/vnd.ni.custom",
    //    // for the DataPathPatterns we need to specify many addition patters because of an unfortunate limitation in androids pattern matching. See https://stackoverflow.com/questions/3400072/pathpattern-to-match-file-extension-does-not-work-if-a-period-exists-elsewhere-i/8599921
    //    DataPathPatterns = new[] { ".*\\\\.cruise", ".*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\..*\\\\.cruise" },
    //    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //    Icon = "@drawable/fscruiser_32dp")]

    public class MainActivity : FormsApplicationActivity
    {
        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            //var device = ev.Device;

            //if (ev.Source.HasFlag(InputSourceType.Stylus))
            //{
            //    return base.DispatchTouchEvent(ev);
            //}
            //else
            //{
            //    return true;
            //}

            return base.DispatchTouchEvent(ev);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MyTheme);//set theme to main theme, because it should be set at launch to the splash theme

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental" });
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Toolkit.Effects.Droid.Effects.Init();
            DLToolkit.Forms.Controls.FlowListView.Init();

            string cruisePath = null;

            //var intent = Intent;
            //if (intent != null && intent.Action == Intent.ActionView)
            //{
            //    var filePath = intent.DataString;
            //    if(string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            //    {
            //        cruisePath = filePath;
            //    }

            //}

            var app = new App(new AndroidPlatformInitializer(this), cruisePath);

            LoadApplication(app);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}