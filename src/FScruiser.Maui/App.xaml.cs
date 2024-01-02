using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; }

    protected App()
    {
        InitializeComponent();
    }

    public App(IServiceProvider serviceProvider) : this()
    {
        //TODO initialize database here, if has database issues set main page to DatabaseUtilities

        MainPage = new AppShell(serviceProvider.GetRequiredService<ShellViewModel>());
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