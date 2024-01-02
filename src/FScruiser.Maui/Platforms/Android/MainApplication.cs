using Android.App;
using Android.OS;
using Android.Runtime;

namespace FScruiser.Maui;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
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

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}