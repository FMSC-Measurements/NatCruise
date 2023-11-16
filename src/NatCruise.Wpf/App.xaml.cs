using CruiseDAL;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Design.Views;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Wpf.FieldData.Views;
using NatCruise.Wpf.Models;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Services;
using NatCruise.Wpf.ViewModels;
using NatCruise.Wpf.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NatCruise.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication, IAppService
    {
        private string[] StartupArgs { get; set; }
        protected ILoggingService LoggingService { get; private set; }

        public NatCruiseViewModelProvider ViewModelProvider { get; } = new NatCruiseViewModelProvider();
        public string StartupFilePath { get; private set; }

        public App()
        { }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // store our start up args for use later
            var startupArgs = StartupArgs = e.Args;

            if (startupArgs.Length == 1)
            {
                var file = new FileInfo(startupArgs[0]);

                if (file.Exists && file.Extension.ToLower() == ".crz3" || file.Extension == ".crz3t")
                {
                    StartupFilePath = file.FullName;
                }
            }

            //ThemeManager.ChangeAppStyle(Application.Current,
            //    ThemeManager.GetAccent("Orange"),
            //    ThemeManager.GetAppTheme("BaseLight"));

            base.OnStartup(e);
        }

        protected override void OnInitialized()
        {
#if !DEBUG
            Microsoft.AppCenter.AppCenter.Start(Secrets.APPCENTER_KEY_WINDOWS,
                               typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));
#endif

            var container = Container;

            var regionManager = container.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(CruiseMasterPage));
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(SaleView));
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(CruiseView));
            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(CombineFileView));

            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(CuttingUnitListView));
            regionManager.RegisterViewWithRegion(Regions.CuttingUnitDetailsRegion, typeof(CuttingUnitDetailView));

            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(StratumListView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(StratumDetailView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(StratumFieldSetupView));
            regionManager.RegisterViewWithRegion(Regions.StratumFieldsRegion, typeof(StratumTreeFieldSetupView));
            regionManager.RegisterViewWithRegion(Regions.StratumFieldsRegion, typeof(StratumLogFieldSetupView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(CuttingUnitStrataView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(SampleGroupListView));

            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SampleGroupDetailView));
            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SubpopulationListView));

            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(TreeAuditRuleListView));
            regionManager.RegisterViewWithRegion(Regions.TreeAuditRuleEdit, typeof(TreeAuditRuleEditView));

            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(TreeDefaultValueListView));

            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(SpeciesListView));

            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(StratumTemplateListView));
            regionManager.RegisterViewWithRegion(Regions.StratumTemplateRegion, typeof(StratumTemplateDetailsView));
            regionManager.RegisterViewWithRegion(Regions.StratumTemplateRegion, typeof(StratumTemplateFieldsView));
            regionManager.RegisterViewWithRegion(Regions.StratumTemplateRegion, typeof(StratumTemplateLogFieldSetupView));

            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(TreeFieldsView));
            //regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(LogFieldsView));

            // template

            //regionManager.RegisterViewWithRegion(Regions.TemplateContentRegion, typeof(TreeAuditRuleListView));
            //regionManager.RegisterViewWithRegion(Regions.TemplateContentRegion, typeof(TreeDefaultValueListView));
            //regionManager.RegisterViewWithRegion(Regions.TemplateContentRegion, typeof(SpeciesListView));
            //regionManager.RegisterViewWithRegion(Regions.TemplateContentRegion, typeof(StratumTemplateListView));
            //regionManager.RegisterViewWithRegion(Regions.TemplateContentRegion, typeof(TreeFieldsView));
            //regionManager.RegisterViewWithRegion(Regions.TemplateContentRegion, typeof(LogFieldsView));

            base.OnInitialized();

#if !DEBUG
            CheckAndRegisterFileAssociations().FireAndForget();
#endif

            ProcessStartupArgs();
        }

        protected void ProcessStartupArgs()
        {
            var startupFilePath = StartupFilePath;
            if (startupFilePath != null)
            {
                try
                {
                    var mainWindowVM = base.MainWindow?.DataContext as MainWindowViewModel;
                    mainWindowVM.OpenFile(startupFilePath);
                }
                catch (Exception ex)
                {
                    LoggingService.LogException(nameof(App), nameof(ProcessStartupArgs), ex);
                }
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // initialize logging service
            var loggingService = new WpfLoggingService();
            containerRegistry.RegisterInstance<ILoggingService>(loggingService);
            // wire logging service up to our task extensions. this will pride logging
            // for our FireAndForget async tasks.
            TaskExtentions.LoggingService = loggingService;

            var fileAssociationService = new FileAssociationService("NCS.V3.NatCruise",
             new[] {
                new AssociatedFileTypeInfo{ Extension = ".crz3", Label = "NCS.V3.Cruise"},
                new AssociatedFileTypeInfo{ Extension = ".crz3t", Label = "NCS.V3.Template"},
            }, loggingService);
            containerRegistry.RegisterInstance<FileAssociationService>(fileAssociationService);

            // register other services
            containerRegistry.RegisterInstance<IAppService>(this);
            containerRegistry.RegisterManySingleton<WpfApplicationSettingService>(typeof(IWpfApplicationSettingService), typeof(IApplicationSettingService));
            containerRegistry.Register<INatCruiseDialogService, WpfDialogService>();
            containerRegistry.Register<IDesignNavigationService, WPFNavigationService>();
            containerRegistry.Register<INatCruiseNavigationService, WPFNavigationService>();
            containerRegistry.Register<IDeviceInfoService, WpfDeviceInfoService>();
            containerRegistry.RegisterSingleton<ISetupInfoDataservice, SetupInfoDataservice>();
            //containerRegistry.RegisterInstance<IFileDialogService>(new WpfFileDialogService());
            containerRegistry.RegisterSingleton<IFileDialogService, WpfFileDialogService>();
            containerRegistry.RegisterInstance<IRecentFilesDataservice>(new RecentFilesDataservice());

            // initialize Dataservice Provider
            var deviceInfo = Container.Resolve<IDeviceInfoService>();
            var dataserviceProvider = new WpfDataserviceProvider((CruiseDatastore_V3)null, deviceInfo);
            WpfDataserviceProvider.RegisterDataservices(containerRegistry);
            containerRegistry.RegisterInstance<IDataserviceProvider>(dataserviceProvider);

            // register views for navigation
            containerRegistry.RegisterDialog<NewCruiseView, NewCruiseViewModel>("NewCruise");
            containerRegistry.RegisterForNavigation<TemplateMasterView>();
            containerRegistry.RegisterForNavigation<CruiseMasterView>();
            //containerRegistry.RegisterForNavigation<CuttingUnitDetailView>();
            containerRegistry.RegisterForNavigation<SaleView>();
            containerRegistry.RegisterForNavigation<CruiseView>();
            containerRegistry.RegisterForNavigation<CuttingUnitListView>();
            containerRegistry.RegisterForNavigation<StratumListView>();
            containerRegistry.RegisterForNavigation<TreeAuditRuleListView>();
            containerRegistry.RegisterForNavigation<TreeDefaultValueListView>();
            containerRegistry.RegisterForNavigation<SpeciesListView>();
            containerRegistry.RegisterForNavigation<StratumTemplateListView>();
            containerRegistry.RegisterForNavigation<TreeFieldsView>();
            containerRegistry.RegisterForNavigation<LogFieldsView>();
            containerRegistry.RegisterForNavigation<DesignChecksView>();
            containerRegistry.RegisterForNavigation<CombineFileView>();
            containerRegistry.RegisterForNavigation<FieldDataView>();

            // register validators
            containerRegistry.Register<CruiseValidator>();
            containerRegistry.Register<CuttingUnitValidator>();
            containerRegistry.Register<SaleValidator>();
            containerRegistry.Register<SampleGroupValidator>();
            containerRegistry.Register<StratumValidator>();
        }

        //[RelayCommand]
        // called in onIninitialized but only for Release builds
        public async Task CheckAndRegisterFileAssociations()
        {
            var fileAssociationService = Container.Resolve<FileAssociationService>();
            var dialogService = Container.Resolve<INatCruiseDialogService>();

            try
            {
                var isAppRegistered = fileAssociationService.IsAppRegistered;
                var unregisteredTypes = fileAssociationService.CheckUnRegisteredFileTypes().ToArray();

                if (!isAppRegistered || unregisteredTypes.Any())
                {
                    if (await dialogService.AskYesNoAsync("Would You Like to Register V3 file types with NatCruise Desktop", "Register Cruise File Types"))
                    {
                        if (!isAppRegistered)
                        { fileAssociationService.RegisterApp(); }

                        foreach (var unregisteredType in unregisteredTypes)
                        {
                            fileAssociationService.RegisterFileType(unregisteredType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogException($"{nameof(App)}.{nameof(RegisterTypes)}", "Error Registering File Types", ex);
            }
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => ViewModelProvider.GetViewModel(viewType));
        }
    }
}