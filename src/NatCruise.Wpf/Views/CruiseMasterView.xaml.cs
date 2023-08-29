﻿using NatCruise.Design.Services;
using NatCruise.Design.Views;
using NatCruise.Wpf.Navigation;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;
using NatCruise.Async;

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