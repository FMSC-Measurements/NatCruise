using NatCruise.Design.Views;
using NatCruise.Wpf.Navigation;
using Prism.Regions;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CruiseMasterPage.xaml
    /// </summary>
    public partial class CruiseMasterPage : UserControl
    {
        public CruiseMasterPage()
        {
            InitializeComponent();
        }

        public CruiseMasterPage(IRegionManager regionManager) : this()
        {
            RegionManager = regionManager;
        }

        public IRegionManager RegionManager { get; }

        private void _unitsButton_Click(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(CuttingUnitListPage));
        }

        private void _strataButton_Click(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(StratumListPage));
        }

        private void _saleButton_Click(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(SalePage));
        }
    }
}