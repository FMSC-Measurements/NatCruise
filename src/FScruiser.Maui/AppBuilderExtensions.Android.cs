using FScruiser.Maui.Effects;
using FScruiser.Maui.Platforms.Android.Services;
using FScruiser.Maui.Services;
using FScruiser.Platforms.Effects;
using NatCruise.Services;

namespace FScruiser.Maui;

public static class AppBuilderExtensions_Android
{
    public static MauiAppBuilder RegisterAndroidServices(this MauiAppBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<MainActivity>(x => ActivityStateManager.Default.GetCurrentActivity() as MainActivity);

        // Android.Content.Context will be registered by Maui when the app starts

        services.AddSingleton<IAppInfoService, AndroidAppInfoService>();
        services.AddSingleton<IFileSystemService, AndroidFileSystemService>();
        services.AddSingleton<IDeviceInfoService, AndroidDeviceInfoService>();
        services.AddSingleton<IFileDialogService, AndroidFileDialogService>();
        services.AddSingleton<ISoundService, AndroidSoundService>();

        builder.ConfigureEffects(effects =>
        {
            effects.Add<ViewLifecycleEffect, AndroidViewLifecycleEffect>();
        });

        return builder;
    }
}