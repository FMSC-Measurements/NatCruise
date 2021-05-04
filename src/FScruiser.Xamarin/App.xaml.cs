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
            this.InitializeComponent();
#if RELEASE
            //start app center services
            Microsoft.AppCenter.AppCenter.Start($"ios={Secrets.APPCENTER_KEY_IOS};android={Secrets.APPCENTER_KEY_DROID};uwp={Secrets.APPCENTER_KEY_UWP}"
                , typeof(Microsoft.AppCenter.Analytics.Analytics)
                ,typeof(Crashes));

#endif

            //var ea = Container.Resolve<IEventAggregator>();

            // var cruiseFileSelectedEvent = ea.GetEvent<CruiseFileSelectedEvent>().Subscribe(async (path) =>
            // {
            //     await LoadCruiseFileAsync(path);
            // });

            //_cruiseFileOpenedEvent = ea.GetEvent<CruiseFileOpenedEvent>();

            //MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_SELECTED, async (sender, path) =>
            //{
            //    await LoadDatabase(path);
            //});
            //await NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits");

            await CruiseNavigationService.ShowCruiseLandingLayout();

            //await NavigationService.NavigateAsync("/Main");
        }

        //protected async System.Threading.Tasks.Task LoadDatabase(string path)
        //{
        //    //var cruiseFileType = CruiseDAL.DAL.ExtrapolateCruiseFileType(path);
        //    //if (cruiseFileType.HasFlag(CruiseDAL.CruiseFileType.Cruise))
        //    //{
        //    //    Analytics.TrackEvent("Error::::LoadCruiseFile|Invalid File Path", new Dictionary<string, string>() { { "FilePath", path } });
        //    //    return;
        //    //}

        //    try
        //    {
        //        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

        //        if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
        //        {
        //            if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage))
        //            {
        //                await DialogService.DisplayAlertAsync("Request Access To Storage", "FScruiser needs access to files on device", "OK");
        //            }

        //            var requestResults = await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
        //            status = requestResults.GetValueOrDefault(Plugin.Permissions.Abstractions.Permission.Storage);
        //        }

        //        if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
        //        {
        //            try
        //            {
        //                if (System.IO.Path.GetExtension(path).ToLowerInvariant() == ".cruise")
        //                {
        //                    var convertedPath = Migrator.GetConvertedPath(path);

        //                    // if converted file already exists let the user know that we
        //                    // are just going to open it instead of the file they selected
        //                    // otherwise convert the .cruise file and open the convered .crz3 file
        //                    if (System.IO.File.Exists(convertedPath) == true)
        //                    {
        //                        await DialogService.DisplayAlertAsync("Message",
        //                            $"Opening {convertedPath}",
        //                            "OK");
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            Migrator.MigrateFromV2ToV3(path, convertedPath);
        //                        }
        //                        catch(Exception e)
        //                        {
        //                            LogAndShowExceptionAsync("File Error", "Unable to Migrate File", e
        //                                , new Dictionary<string, string>(){ { "FileName", path } })
        //                                .FireAndForget();
        //                            return;
        //                        }

        //                        var fileName = System.IO.Path.GetFileName(path);
        //                        await DialogService.DisplayAlertAsync("Message",
        //                            $"Your cruise file has been updated and the file {fileName} has been created",
        //                            "OK");
        //                    }

        //                    path = convertedPath;
        //                }

        //                DataserviceProvider.OpenDatabase(path);
        //                path = DataserviceProvider.DatabasePath;

        //                Properties.SetValue("cruise_path", path);

        //                //var sampleInfoDS = DataserviceProvider.GetDataservice<ISampleInfoDataservice>();
        //                //if (sampleInfoDS.HasSampleStates() == false
        //                //    && sampleInfoDS.HasSampleStateEnvy()
        //                //    && await DialogService2.AskYesNoAsync("This file doesn't have any samplers associated with this device. Would you to continue by copying a state from another device?", "Copy samplers from another device?"))
        //                //{
        //                //    await NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits/SampleStateManagmentOther");
        //                //}
        //                //else
        //                //{
        //                //    await NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits");
        //                //}

        //                await NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits");

        //                MessagingCenter.Send<object, string>(this, Messages.CRUISE_FILE_OPENED, path);
        //            }
        //            catch (FileNotFoundException ex)
        //            {
        //                LogAndShowExceptionAsync("File Error", "File Not Found",
        //                    ex, new Dictionary<string, string> { { "FilePath", path } })
        //                    .FireAndForget();
        //            }
        //            catch (Exception ex)
        //            {
        //                 LogAndShowExceptionAsync("File Error", "File Could Not Be Opended",
        //                     ex, new Dictionary<string, string> { { "FilePath", path } })
        //                    .FireAndForget();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException("permissions", "request permissions error in method LoadCruiseFileAsync", ex);
        //    }
        //}

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
                var enableDiagnostics = await DialogService.DisplayAlertAsync("", $"FScruiser can automaticly send dianostics and crash reports.{Environment.NewLine}{Environment.NewLine} " +
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
                var deviceInfo = Container.Resolve<IDeviceInfoService>();
                var fileSystemService = Container.Resolve<IFileSystemService>();
                var cruiseDbPath = fileSystemService.DefaultCruiseDatabasePath;
                if (File.Exists(cruiseDbPath) == false)
                {
                    var newDb = new CruiseDatastore_V3(cruiseDbPath, true);
                }

                _dataserviceProvider = new DataserviceProvider(cruiseDbPath, deviceInfo);
                _dataserviceProvider.RegisterDataservices(containerRegistry);

                containerRegistry.RegisterInstance<IDataserviceProvider>(_dataserviceProvider);
            }

            if (containerRegistry.IsRegistered<ICruisersDataservice>() == false)
            {
                containerRegistry.RegisterInstance<ICruisersDataservice>(_cruisersDataservice = new CruisersDataservice(this));
            }
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