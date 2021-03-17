using NatCruise.Design.Services;
using NatCruise.Design.Views;
using NatCruise.Wpf.Navigation;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CruiseMasterPage.xaml
    /// </summary>
    public partial class CruiseMasterView : UserControl
    {
        public CruiseMasterView()
        {
            InitializeComponent();
        }

        IDesignNavigationService NavigationService { get; }

        public CruiseMasterView(IDesignNavigationService navigationService) : this()
        {
            //RegionManager = regionManager;
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        //public IRegionManager RegionManager { get; }

        private void _unitsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowCuttingUnitList();

            //RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(CuttingUnitListPage));
        }

        private void _strataButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowStrata();

            //RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(StratumListPage));
        }

        private void _saleButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowSale();

            //RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(SalePage));
        }
    }
}