using CruiseDAL;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Design.Views;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Services.Logging;
using NatCruise.Wpf.Data;
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
using System.Collections.Generic;
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

        protected ILogger Logger { get; private set; }

        public IViewModelTypeResolver ViewModelTypeResolver { get; } = new NatCruiseViewModelTypeResolver();
        public string StartupFilePath { get; private set; }

        public App()
        {
            LoggingService = new WpfLoggingService();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            Microsoft.AppCenter.AppCenter.Start(Secrets.APPCENTER_KEY_WINDOWS,
                               typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));
#endif

            

            // store our start up args for use later
            var startupArgs = StartupArgs = e.Args;

            LoggingService.LogEvent("StartUpArgs", new Dictionary<string, string>()
            {
                { "ArgsCount", startupArgs.Length.ToString() },
                { "Args", string.Join(", ", startupArgs) }
            });

            if (startupArgs.Length == 1)
            {
                var file = new FileInfo(startupArgs[0]);

                if (file.Exists
                    && (file.Extension.ToLower() == ".crz3" || file.Extension.ToLower() == ".crz3t"))
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
            var container = Container;

            Logger = container.Resolve<ILogger<App>>();
            Logger.LogTrace("Logger Resolved");

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
            var loggingService = LoggingService;
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

            // register Microsoft.Extentions.Logging
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.ILoggerFactory), CreateLoggerFactory);
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.LoggerFactory), CreateLoggerFactory);
            containerRegistry.RegisterSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(Microsoft.Extensions.Logging.Logger<>));

            LoggerFactory CreateLoggerFactory()
            {
                return new Microsoft.Extensions.Logging.LoggerFactory(new[] { new AppCenterLoggerProvider() });
            }

            // should this be changed to scoped?
            containerRegistry.RegisterSingleton<IDataContextService, DataContextService>();

            // register data services.
            var servicesToRegister = new ServiceCollection();
            servicesToRegister.AddNatCruiseCoreDataservices();

            var container = containerRegistry.GetContainer();
            container.Populate(servicesToRegister);
            containerRegistry.Register<IDesignCheckDataservice, DesignCheckDataservice>();
            containerRegistry.Register<ICruisersDataservice, CruisersDataservice>();

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

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => ViewModelTypeResolver.GetViewModelType(viewType));
        }
    }
}