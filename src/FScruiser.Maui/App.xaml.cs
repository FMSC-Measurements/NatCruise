using CruiseDAL;
using DevExpress.Data.Browsing;
using FMSC.ORM.Logging;
using FScruiser.Maui.Services;
using FScruiser.Maui.ViewModels;
using FScruiser.Maui.Views;
using Microsoft.Extensions.Logging;
using NatCruise.Data;
using NatCruise.Services;

namespace FScruiser.Maui;

public partial class App : Application
{
    public IServiceProvider Services { get; }
    public ILogger<App> Log { get; }

    protected App()
    {
        InitializeComponent();
    }

    public App(IServiceProvider serviceProvider, ILogger<App> log) : this()
    {
        Log = log;
        Services = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var services = activationState!.Context.Services;
        var dataContext = services.GetRequiredService<IDataContextService>();
        var fileSystemService = services.GetRequiredService<IFileSystemService>();

        var databasePath = fileSystemService.DefaultCruiseDatabasePath;

        //initialize database, If Database Fails to open display database utilities view

        Page mainPage = dataContext.OpenOrCreateDatabase(databasePath) ? services.GetRequiredService<MainView>()
            : services.GetRequiredService<DatabaseUtilitiesView>();

        var navProvider = services.GetRequiredService<INavigationProvider>();
        navProvider.MainPage = mainPage;

        return new Window(mainPage);
    }

    //protected static IDataserviceProvider? GetDataserviceProvider(IServiceProvider sp, out Exception dspInitError)
    //{
    //    dspInitError = null;
    //    var deviceInfo = sp.GetRequiredService<IDeviceInfoService>();
    //    var fileSystemService = sp.GetRequiredService<IFileSystemService>();
    //    var cruiseDbPath = fileSystemService.DefaultCruiseDatabasePath;

    //    try
    //    {
    //        if (File.Exists(cruiseDbPath) == false)
    //        {
    //            var db = new CruiseDatastore_V3(cruiseDbPath, true);
    //            return new DataserviceProviderBase(db, deviceInfo);
    //        }
    //        else
    //        {
    //            var db = new CruiseDatastore_V3(cruiseDbPath, false);
    //            return new DataserviceProviderBase(db, deviceInfo);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        dspInitError = ex;
    //        return null;
    //    }
    //}
}