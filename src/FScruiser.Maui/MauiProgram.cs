using CommunityToolkit.Maui;
using FScruiser.Maui.Data;
using Microsoft.Extensions.Logging;
using NatCruise.Data;
using NatCruise.Services.Logging;
using Backpack.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using NatCruise;

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
            .UseMauiCompatibility() //TODO remove when done refactoring TallyPage to remove RelativeLayout
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

        builder.Services.AddNatCruiseCoreDataservices();

#if DEBUG
        builder.Logging.AddDebug();
#else
        builder.Logging.AddAppCenterLogger();
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