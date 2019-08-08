using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NatCruise.Wpf.Data;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Services;
using NatCruise.Wpf.ViewModels;
using NatCruise.Wpf.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;

namespace NatCruise.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
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
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(CuttingUnitListPage));
            regionManager.RegisterViewWithRegion(Regions.CruiseContentRegion, typeof(StratumListPage));
            regionManager.RegisterViewWithRegion(Regions.CuttingUnitDetailsRegion, typeof(CuttingUnitDetailPage));

            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(StratumDetailPage));
            regionManager.RegisterViewWithRegion(Regions.StratumDetailsRegion, typeof(SampleGroupListPage));

            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SampleGroupDetailPage));
            regionManager.RegisterViewWithRegion(Regions.SampleGroupDetailsRegion, typeof(SubpopulationListPage));

            base.OnInitialized();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<CuttingUnitListPage, CuttingUnitListViewModel>();
            ViewModelLocationProvider.Register<CuttingUnitDetailPage, CuttingUnitDetailViewModel>();
            ViewModelLocationProvider.Register<StratumListPage, StratumListViewModel>();
            ViewModelLocationProvider.Register<StratumDetailPage, StratumDetailViewModel>();
            ViewModelLocationProvider.Register<SampleGroupListPage, SampleGroupListViewModel>();
            ViewModelLocationProvider.Register<SampleGroupDetailPage, SampleGroupDetailViewModel>();
            ViewModelLocationProvider.Register<SubpopulationListPage, SubpopulationListViewModel>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NewCruisePage, NewCruiseViewModel>("NewCruise");

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