using FScruiser.Maui.Platforms.Android.Services;
using FScruiser.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui;

public static class AppBuilderExtensions_Android
{
    public static MauiAppBuilder RegisterAndroidServices(this MauiAppBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<MainActivity>(x => ActivityStateManager.Default.GetCurrentActivity() as MainActivity );

        // Android.Content.Context will be registered by Maui when the app starts

        services.AddSingleton<IAppInfoService, AndroidAppInfoService>();
        services.AddSingleton<IFileSystemService, AndroidFileSystemService>();
        services.AddSingleton<IDeviceInfoService, AndroidDeviceInfoService>();
        services.AddSingleton<IFileDialogService, AndroidFileDialogService>();
        services.AddSingleton<ISoundService, AndroidSoundService>();

        return builder;
    }
}
