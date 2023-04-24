using Android.App;
using Android.OS;
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

#if DEBUG
            // logs use of unencrypted network traffic
            // we only what to do this in when in debug mode,
            // however when running in release mode the app stalls in the splash screen.
            StrictMode.SetVmPolicy(new StrictMode.VmPolicy.Builder()
                .DetectCleartextNetwork()
                .PenaltyLog()
                .Build());
#endif
        }
    }
}