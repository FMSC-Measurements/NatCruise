using Android.App;
using Android.OS;
using Android.Runtime;
using NatCruise.Services;

[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaAudio)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaVideo)]

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

        NatCruise.Async.TaskExtentions.LoggingService = Services.GetRequiredService<ILoggingService>();// logging service required by taskExtentions. TODO transition taskExtentions to using ILogger

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