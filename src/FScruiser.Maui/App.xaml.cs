using CruiseDAL;
using FMSC.ORM.Logging;
using FScruiser.Maui.ViewModels;
using FScruiser.Maui.Views;
using Microsoft.Extensions.Logging;
using NatCruise.Data;
using NatCruise.Services;

namespace FScruiser.Maui;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; }
    public ILogger<App> Log { get; }

    protected App()
    {
        InitializeComponent();
    }

    public App(IServiceProvider serviceProvider, ILogger<App> log) : this()
    {
        Log = log;
        //TODO initialize database here, if has database issues set main page to DatabaseUtilities

        var dataContext = serviceProvider.GetRequiredService<IDataContextService>();
        var fileSystemService = serviceProvider.GetRequiredService<IFileSystemService>();

        var databasePath = fileSystemService.DefaultCruiseDatabasePath;


        if(!dataContext.OpenOrCreateDatabase(databasePath))
        {
            MainPage = new DatabaseUtilitiesView();
        }


        ServiceProvider = serviceProvider;
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