using CommunityToolkit.Maui;
using FScruiser.Maui.Data;
using Microsoft.Extensions.Logging;
using NatCruise.Data;
using Backpack.Maui;

namespace FScruiser.Maui;

public static class MauiProgram
{
    //public static MauiApp CreateMauiApp(IEnumerable<ServiceDescriptor> platformServices)
    public static MauiApp CreateMauiApp()
    {
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBackpack()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwesomeSolid.otf", Constants.Fonts.FAsolid);
            })
            .RegisterServices()
            .RegisterViews()
            .RegisterViewModels();

#if DEBUG
        builder.Logging.AddDebug();
#endif

       

        //foreach (var service in platformServices)
        //{
        //    builder.Services.Add(service);
        //}

#if ANDROID
        builder.RegisterAndroidServices();
#endif

        return builder.Build();
    }
}