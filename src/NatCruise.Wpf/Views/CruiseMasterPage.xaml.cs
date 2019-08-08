using NatCruise.Wpf.Navigation;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
