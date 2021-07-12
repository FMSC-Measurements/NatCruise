using CruiseDAL;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Services;
using NatCruise.Design.Validation;
using NatCruise.Design.Views;
using NatCruise.Services;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Services;
using NatCruise.Wpf.ViewModels;
using NatCruise.Wpf.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace NatCruise.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication, IAppService
    {
        private string[] StartupArgs { get; set; }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // store our start up args for use later
            StartupArgs = e.Args;

            //ThemeManager.ChangeAppStyle(Application.Current,
            //    ThemeManager.GetAccent("Orange"),
            //    ThemeManager.GetAppTheme("BaseLight"));

            base.OnStartup(e);
        }

        protected override void OnInitialized()
        {
#if !DEBUG
            AppCenter.Start(Secrets.APPCENTER_KEY_WINDOWS,
                               typeof(Analytics), typeof(Crashes));
#endif

            var container = Container;
            var regionManager = container.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(CruiseMasterPage));
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(SaleView));
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(CruiseView));

            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(CuttingUnitListView));
            regionManager.RegisterViewWithRegion(Regions.CuttingUnitDetailsRegion, typeof(CuttingUnitDetailView));

            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(StratumListView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(StratumDetailView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(StratumFieldSetupView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(CuttingUnitStrataView));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(SampleGroupListView));
            

            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SampleGroupDetailView));
            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SubpopulationListView));

            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(TreeAuditRuleListView));
            regionManager.RegisterViewWithRegion(Regions.TreeAuditSelectors, typeof(TreeAuditSelectorsView));

            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(TreeDefaultValueListView));

            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(SpeciesListView));

            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(StratumTemplateListView));
            regionManager.RegisterViewWithRegion(Regions.StratumTemplateRegion, typeof(StratumTemplateDetailsView));
            regionManager.RegisterViewWithRegion(Regions.StratumTemplateRegion, typeof(StratumTemplateFieldsView));
            

            base.OnInitialized();

            //var startupArgs = StartupArgs;

            //if (startupArgs.Length > 0)
            //{
            //    var arg1 = startupArgs[0];
            //    try
            //    {
            //        var path = Path.GetFullPath(arg1);
            //        if (File.Exists(path))
            //        {
            //            var mainWindowVM = base.MainWindow?.DataContext as MainWindowViewModel;
            //            mainWindowVM.OpenFile(path);
            //        }
            //    }
            //    catch
            //    { }
            //}
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IAppService>(this);
            containerRegistry.Register<IApplicationSettingService, WpfApplicationSettingService>();
            containerRegistry.Register<IDialogService, WpfDialogService>();
            containerRegistry.Register<IDesignNavigationService, WPFNavigationService>();
            containerRegistry.Register<IDeviceInfoService, WpfDeviceInfoService>();
            containerRegistry.RegisterSingleton<ISetupInfoDataservice, SetupInfoDataservice>();
            containerRegistry.RegisterInstance<ILoggingService>(new WpfLoggingService());
            containerRegistry.RegisterInstance<IFileDialogService>(new WpfFileDialogService());
            containerRegistry.RegisterInstance<IRecentFilesDataservice>(new RecentFilesDataservice());

            var deviceInfo = Container.Resolve<IDeviceInfoService>();
            var dataserviceProvider = new WpfDataserviceProvider((CruiseDatastore_V3)null, deviceInfo);
            dataserviceProvider.RegisterDataservices(containerRegistry);
            containerRegistry.RegisterInstance<IDataserviceProvider>(dataserviceProvider);

            containerRegistry.RegisterDialog<NewCruiseView, NewCruiseViewModel>("NewCruise");

            containerRegistry.RegisterForNavigation<CruiseMasterView>();
            containerRegistry.RegisterForNavigation<CuttingUnitListView>();
            containerRegistry.RegisterForNavigation<CuttingUnitDetailView>();


            // register validators
            containerRegistry.Register<CruiseValidator>();
            containerRegistry.Register<CuttingUnitValidator>();
            containerRegistry.Register<SaleValidator>();
            containerRegistry.Register<SampleGroupValidator>();
            containerRegistry.Register<StratumValidator>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                viewName = viewName.Replace(".Views.", ".ViewModels.");

                string viewAssemblyName = null;
                if (viewName.StartsWith("NatCruise.Design"))
                {
                    //var assemblyName = Assembly.GetAssembly(typeof(CuttingUnitDetailPage)).FullName;
                    viewAssemblyName = "NatCruise.Design";
                }
                else
                {
                    viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                }

                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

                return Type.GetType(viewModelName);
            });

            //ViewModelLocationProvider.SetDefaultViewModelFactory((viewType) =>
            //{
            //    var viewName =
            //})
        }
    }
}