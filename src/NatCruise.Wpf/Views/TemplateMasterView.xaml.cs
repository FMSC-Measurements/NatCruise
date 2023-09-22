using NatCruise.Wpf.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for TemplateMasterView.xaml
    /// </summary>
    public partial class TemplateMasterView : UserControl
    {
        public TemplateMasterView(IDesignNavigationService navigationService)
        {
            InitializeComponent();

            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public IDesignNavigationService NavigationService { get; }

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
    }
}