using CommunityToolkit.Maui;
using FScruiser.Maui.Data;
using Microsoft.Extensions.Logging;
using NatCruise.Data;
using NatCruise.Services.Logging;
using Backpack.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using NatCruise;
using DevExpress.Maui;

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
            .UseDevExpress()
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

        builder.Services.AddLogging(logging =>
        {
#if DEBUG && WINDOWS
            logging.AddDebug();
#elif DEBUG
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Trace);
#else
            logging.AddAppCenterLogger();
#endif
        });

        builder.ConfigureEssentials(essentials =>
        {
            essentials.UseVersionTracking(); // used to track if app is being launched for first time. Stores info using the Preferences API
        });


#if ANDROID
        builder.RegisterAndroidServices();
#endif

        return builder.Build();
    }
}