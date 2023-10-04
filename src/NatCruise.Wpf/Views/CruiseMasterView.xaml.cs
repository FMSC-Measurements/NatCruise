using NatCruise.Async;
using NatCruise.Wpf.Services;
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

        private IDesignNavigationService NavigationService { get; }

        public CruiseMasterView(IDesignNavigationService navigationService) : this()
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        private void _unitsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowCuttingUnitList();
        }

        private void _strataButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowStrata();
        }

        private void _saleButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowSale();
        }

        private void _cruiseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowCruise();
        }

        private void _auditRuleButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowTreeAuditRules();
        }

        private void _tdv_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowTreeDefaultValues();
        }

        private void _speciesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowSpecies();
        }

        private void _designTemplatesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowDesignTemplates();
        }

        private void _treeFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowTreeFields();
        }

        private void _logFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowLogFields();
        }

        private void _designChecks_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowDesignChecks();
        }

        private void _combineFile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowCombineFile().FireAndForget();
        }

        private void _fieldDataButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.ShowFieldData().FireAndForget();
        }
    }
}