using CruiseDAL;
using FScruiser.XF.Controls;
using FScruiser.XF.Data;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.MVVM;
using NatCruise.Services;
using NatCruise.Services.Logging;
using NatCruise.Util;
using NatCruise;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using DryIoc.Microsoft.DependencyInjection;

namespace FScruiser.XF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Prism.DryIoc.PrismApplication
    {
        public const string CURRENT_NAV_PATH = "current_nav_path";
        public const string CURRENT_NAV_PARAMS = "current_nav_params";

        //private CruiseFileSelectedEvent _cruiseFileSelectedEvent;
        //private CruiseFileOpenedEvent _cruiseFileOpenedEvent;
        private ICruisersDataservice _cruisersDataservice;

        private Exception _dataContextInitError;

        protected IPageDialogService DialogService => Container?.Resolve<IPageDialogService>();
        public ICruiseNavigationService CruiseNavigationService => Container?.Resolve<ICruiseNavigationService>();

        public IApplicationSettingService Settings { get; } = new XamarinApplicationSettingService();
        public IViewModelTypeResolver ViewModelTypeResolver { get; } = new NatCruiseViewModelTypeResolver();

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
            TaskExtentions.LoggingService = loggingService;

            TapGestureRecognizerHelper.SoundService = Container.Resolve<ISoundService>();

            this.InitializeComponent();
            

#if RELEASE
            //start app center services
            Microsoft.AppCenter.AppCenter.Start($"ios={Secrets.APPCENTER_KEY_IOS};android={Secrets.APPCENTER_KEY_DROID};uwp={Secrets.APPCENTER_KEY_UWP}"
                , typeof(Microsoft.AppCenter.Analytics.Analytics)
                ,typeof(Crashes));

#endif
            //try
            //{
            //    var dsp = GetDataserviceProvider();

            //    await CruiseNavigationService.ShowCruiseLandingLayout();
            //}
            //catch (Exception ex)
            //{
            //    await CruiseNavigationService.ShowDatabaseUtilities();
            //}

            if (InitDataContext())
            {
                await CruiseNavigationService.ShowCruiseLandingLayout();
            }
            else
            {
                await CruiseNavigationService.ShowDatabaseUtilities();
            }
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
                var enableDiagnostics = await DialogService.DisplayAlertAsync("Send Diagnostic Data", $"FScruiser can automatically send diagnostics and crash reports.{Environment.NewLine}{Environment.NewLine} " +
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
            // primary location for type registration is in XamarinPlatFormInitializer
            // below is additional registration where we want hold on to the created instance here in the app
            // or where we want to initialize the instance with the app instance

            //InitDataContext();

            if (containerRegistry.IsRegistered<ICruisersDataservice>() == false)
            {
                containerRegistry.RegisterInstance<ICruisersDataservice>(_cruisersDataservice = new CruisersDataservice(this));
            }

            // register Microsoft.Extentions.Logging
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.ILoggerFactory), typeof(Microsoft.Extensions.Logging.LoggerFactory));
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.LoggerFactory), () => new Microsoft.Extensions.Logging.LoggerFactory(new[] {new AppCenterLoggerProvider()}));
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(Microsoft.Extensions.Logging.Logger<>));


            var servicesToRegister = new ServiceCollection();
            servicesToRegister.AddNatCruiseCoreDataservices();

            var container = containerRegistry.GetContainer();
            container.Populate(servicesToRegister);
        }


        protected bool InitDataContext()
        {
            var dataContext = Container.Resolve<IDataContextService>();

            if(!dataContext.IsReady)
            {
                var fileSystemService = Container.Resolve<IFileSystemService>();
                var cruiseDbPath = fileSystemService.DefaultCruiseDatabasePath;

                dataContext.OpenOrCreateDatabase(cruiseDbPath);
            }

            return dataContext.IsReady;
        }

        protected override void ConfigureViewModelLocator()
        {
            // note although some view models will be located using the convention established below
            // there are some that are registered to view names explicitly. See XamarinPlatformInitializer

            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => ViewModelTypeResolver.GetViewModelType(viewType));
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
            var dialogService = DialogService;
            var result = await dialogService.DisplayAlertAsync(catigory, message, "Details", "OK");
            if (result == true)//user clicked Details
            {
                await dialogService.DisplayAlertAsync(catigory, ex.ToString(), "Close");
            }
        }
    }
}