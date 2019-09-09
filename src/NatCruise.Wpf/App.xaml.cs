using NatCruise.Wpf.Data;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Services;
using NatCruise.Wpf.ViewModels;
using NatCruise.Wpf.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using System.IO;
using System.Windows;

namespace NatCruise.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
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
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(SalePage));
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(CuttingUnitListPage));
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(StratumListPage));
            regionManager.RegisterViewWithRegion(Regions.CuttingUnitDetailsRegion, typeof(CuttingUnitDetailPage));

            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(StratumDetailPage));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(CuttingUnitStrataPage));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(SampleGroupListPage));

            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SampleGroupDetailPage));
            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SubpopulationListPage));

            base.OnInitialized();

            var startupArgs = StartupArgs;

            if (startupArgs.Length > 0)
            {
                var arg1 = startupArgs[0];
                try
                {
                    var path = Path.GetFullPath(arg1);
                    if (File.Exists(path))
                    {
                        var mainWindowVM = base.MainWindow?.DataContext as MainWindowViewModel;
                        mainWindowVM.OpenFile(path);
                    }
                }
                catch
                { }
            }
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            //ViewModelLocationProvider.Register<SalePage, SalePageViewModel>();
            //ViewModelLocationProvider.Register<CuttingUnitListPage, CuttingUnitListPageViewModel>();
            //ViewModelLocationProvider.Register<CuttingUnitDetailPage, CuttingUnitDetailPageViewModel>();
            //ViewModelLocationProvider.Register<CuttingUnitStrataPage, CuttingUnitStrataPageViewModel>();
            //ViewModelLocationProvider.Register<StratumListPage, StratumListPageViewModel>();
            //ViewModelLocationProvider.Register<StratumDetailPage, StratumDetailPageViewModel>();
            //ViewModelLocationProvider.Register<SampleGroupListPage, SampleGroupListPageViewModel>();
            //ViewModelLocationProvider.Register<SampleGroupDetailPage, SampleGroupDetailPageViewModel>();
            //ViewModelLocationProvider.Register<SubpopulationListPage, SubpopulationListPageViewModel>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NewCruisePage, NewCruisePageViewModel>("NewCruise");

            containerRegistry.RegisterForNavigation<CruiseMasterPage>();
            containerRegistry.RegisterForNavigation<CuttingUnitListPage>();
            containerRegistry.RegisterForNavigation<CuttingUnitDetailPage>();

            containerRegistry.RegisterInstance<ILoggingService>(new WpfLoggingService());
            containerRegistry.RegisterInstance<IDataserviceProvider>(new DataserviceProvider());
            containerRegistry.RegisterInstance<IFileDialogService>(new FileDialogService());
            containerRegistry.RegisterInstance<IRecentFilesDataservice>(new RecentFilesDataservice());
        }
    }
}