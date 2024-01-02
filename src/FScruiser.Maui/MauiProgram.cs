using FScruiser.Maui.Data;
using Microsoft.Extensions.Logging;
using NatCruise.Data;

namespace FScruiser.Maui;

public static class MauiProgram
{
    //public static MauiApp CreateMauiApp(IEnumerable<ServiceDescriptor> platformServices)
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
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