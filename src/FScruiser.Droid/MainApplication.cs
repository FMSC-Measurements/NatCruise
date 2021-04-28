using Android.App;
using Android.Runtime;
using System;

namespace FScruiser.Droid
{
#if DEBUG
	[Application(Debuggable = true)]
#else

    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }
         
        public override void OnCreate()
        {
            base.OnCreate();
            //CrossCurrentActivity.Current.Init(this);
            Xamarin.Essentials.Platform.Init(this); // used for FilePicker
        }
    }
}