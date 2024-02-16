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

        protected ILogger Logger { get; private set; }

        public App() : this(new XamarinPlatformInitializer())
        { }

        public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
        { }

        protected override async void OnInitialized()
        {
            Xamarin.Forms.DataGrid.DataGridComponent.Init();


#if RELEASE
            //start app center services
            Microsoft.AppCenter.AppCenter.Start($"ios={Secrets.APPCENTER_KEY_IOS};android={Secrets.APPCENTER_KEY_DROID};uwp={Secrets.APPCENTER_KEY_UWP}"
                , typeof(Microsoft.AppCenter.Analytics.Analytics)
                ,typeof(Crashes));

#endif

            // hook up our logging service to our utility TaskExtentions class
            // this helper extention class is used to get exceptions from
            // 'Fire and Forget' async actions
            var loggingService = Container.Resolve<ILoggingService>();
            TaskExtentions.LoggingService = loggingService;

            var logger = Logger = Container.Resolve<ILogger<App>>();

            TapGestureRecognizerHelper.SoundService = Container.Resolve<ISoundService>();

            this.InitializeComponent();

            if (InitDataContext())
            {
                await CruiseNavigationService.ShowCruiseLandingLayout();
            }
            else
            {
                await CruiseNavigationService.ShowDatabaseUtilities();
            }
        }

        protected bool InitDataContext()
        {
            var dataContext = Container.Resolve<IDataContextService>();

            if (!dataContext.IsReady)
            {
                var fileSystemService = Container.Resolve<IFileSystemService>();
                var cruiseDbPath = fileSystemService.DefaultCruiseDatabasePath;

                dataContext.OpenOrCreateDatabase(cruiseDbPath);
            }

            return dataContext.IsReady;
        }

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
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // primary location for type registration is in XamarinPlatFormInitializer
            // below is additional registration where we want hold on to the created instance here in the app
            // or where we want to initialize the instance with the app instance

            if (containerRegistry.IsRegistered<ICruisersDataservice>() == false)
            {
                containerRegistry.RegisterInstance<ICruisersDataservice>(_cruisersDataservice = new CruisersDataservice(this));
            }

            // register Microsoft.Extentions.Logging
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.ILoggerFactory), CreateLoggerFactory);
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.LoggerFactory), CreateLoggerFactory);
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(Microsoft.Extensions.Logging.Logger<>));

            LoggerFactory CreateLoggerFactory()
            {
                return new Microsoft.Extensions.Logging.LoggerFactory(new[] { new AppCenterLoggerProvider() });
            }

            // regester data services. these are defined using the Microsoft.Extentions.DependancyInjection pattern
            // using a service collection to store all our registration definitions
            // we can populate our existing container with these definitions.
            var servicesToRegister = new ServiceCollection();
            servicesToRegister.AddNatCruiseCoreDataservices();

            var container = containerRegistry.GetContainer();
            container.Populate(servicesToRegister);
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