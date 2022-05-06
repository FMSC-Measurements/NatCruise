using CruiseDAL;
using FScruiser.XF.Data;
using FScruiser.XF.Services;
using FScruiser.XF.Util;
using Microsoft.AppCenter.Crashes;
using NatCruise.Core.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Services;
using NatCruise.Util;
using Prism;
using Prism.Ioc;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Prism.DryIoc.PrismApplication
    {
        public const string CURRENT_NAV_PATH = "current_nav_path";
        public const string CURRENT_NAV_PARAMS = "current_nav_params";

        private DataserviceProvider _dataserviceProvider;

        //private CruiseFileSelectedEvent _cruiseFileSelectedEvent;
        //private CruiseFileOpenedEvent _cruiseFileOpenedEvent;
        private ICruisersDataservice _cruisersDataservice;

        protected IPageDialogService DialogService => Container?.Resolve<IPageDialogService>();
        public ICruiseNavigationService CruiseNavigationService => Container?.Resolve<ICruiseNavigationService>();

        public IDataserviceProvider DataserviceProvider => _dataserviceProvider;

        public IApplicationSettingService Settings { get; } = new XamarinApplicationSettingService();

        public App() : this(new XamarinPlatformInitializer())
        { }

        public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
        { }

        protected override async void OnInitialized()
        {
            Xamarin.Forms.DataGrid.DataGridComponent.Init();

            // hook up our logging service to our utility TaskExtentions class
            // this helper extention class is used to get exceptions from
            // 'Fire and Forget' async actions
            var loggingService = Container.Resolve<ILoggingService>();
            NatCruise.Util.TaskExtentions.LoggingService = loggingService;

            this.InitializeComponent();

#if RELEASE
            //start app center services
            Microsoft.AppCenter.AppCenter.Start($"ios={Secrets.APPCENTER_KEY_IOS};android={Secrets.APPCENTER_KEY_DROID};uwp={Secrets.APPCENTER_KEY_UWP}"
                , typeof(Microsoft.AppCenter.Analytics.Analytics)
                ,typeof(Crashes));

#endif
            await CruiseNavigationService.ShowCruiseLandingLayout();


        }

        //protected void ReloadNavigation()
        //{
        //    var navPath = Properties.GetValueOrDefault(CURRENT_NAV_PATH) as string;

        //    if(navPath != null && !navPath.EndsWith("CuttingUnits"))
        //    {
        //        var navParams = Properties.GetValueOrDefault(CURRENT_NAV_PARAMS) as string;

        //        NavigationService.NavigateAsync(navPath, new NavigationParameters(navParams));
        //    }
        //}

        protected override async void OnStart()
        {
            // Handle when your app starts

            var isFirstRun = Properties.GetValueOrDefault<bool>("isFirstLaunch", true);
            if (isFirstRun)
            {
                var enableDiagnostics = await DialogService.DisplayAlertAsync("", $"FScruiser can automatically send diagnostics and crash reports.{Environment.NewLine}{Environment.NewLine} " +
                    $"This feature is optional, however, it helps us to create a better experience.{Environment.NewLine}{Environment.NewLine}" +
                    $"Would you like to enable this feature?{Environment.NewLine}{Environment.NewLine}" +
                    "You can also change this option in the settings page.", "Enable", "No Thanks");

                Settings.EnableAnalitics = Settings.EnableCrashReports = enableDiagnostics;

                Properties.SetValue("isFirstLaunch", false);
            }

            //var cruise_path = _cruisePath ?? Properties.GetValueOrDefault("cruise_path") as string;

            //if (!string.IsNullOrEmpty(cruise_path))
            //{
            //    await LoadDatabase(cruise_path);
            //}
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if(containerRegistry.IsRegistered<IFileDialogService>() == false)
            {
                containerRegistry.Register<IFileDialogService, XamarinFileDialogService>();
            }

            if (containerRegistry.IsRegistered<IDataserviceProvider>() == false)
            {
                var dataserviceProvider = GetDataserviceProvider();
                containerRegistry.RegisterInstance<IDataserviceProvider>(dataserviceProvider);
                dataserviceProvider.RegisterDataservices(containerRegistry);
                //containerRegistry.RegisterSingleton<IDataserviceProvider>(GetDataserviceProvider);
            }

            if (containerRegistry.IsRegistered<ICruisersDataservice>() == false)
            {
                containerRegistry.RegisterInstance<ICruisersDataservice>(_cruisersDataservice = new CruisersDataservice(this));
            }
        }

        protected DataserviceProvider GetDataserviceProvider()
        {
            if (_dataserviceProvider is null)
            {
                var deviceInfo = Container.Resolve<IDeviceInfoService>();
                var fileSystemService = Container.Resolve<IFileSystemService>();
                var cruiseDbPath = fileSystemService.DefaultCruiseDatabasePath;
                if (File.Exists(cruiseDbPath) == false)
                {
                    //throw new Exception();
                    var newDb = new CruiseDatastore_V3(cruiseDbPath, true);
                    _dataserviceProvider = new DataserviceProvider(newDb, deviceInfo);
                }
                else
                {
                    var db = new CruiseDatastore_V3(cruiseDbPath, false);
                    _dataserviceProvider = new DataserviceProvider(db, deviceInfo);
                }
            }
            return _dataserviceProvider;
        }

        public static void LogException(string catigory, string message, Exception ex, IDictionary<string, string> data = null)
        {
            Debug.WriteLine($"Error:::{catigory}::::{message}::::{ex.Message}::::");
            Debug.WriteLine(ex.StackTrace);

            if (data == null) { data = new Dictionary<string, string>(); }

            data.SetValue("error_catigory", catigory);
            data.SetValue("error_message", message);

            Crashes.TrackError(ex, data);
        }

        public async Task LogAndShowExceptionAsync(string catigory, string message, Exception ex, IDictionary<string, string> data = null)
        {
            LogException(catigory, message, ex, data);

            var result = await DialogService.DisplayAlertAsync(catigory, message, "Details", "OK");
            if (result == true)//user clicked Details
            {
                await DialogService.DisplayAlertAsync(catigory, ex.ToString(), "Close");
            }
        }
    }
}