﻿using NatCruise.Design.Services;
using NatCruise.Design.Views;
using NatCruise.Navigation;
using NatCruise.Wpf.FieldData.Views;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Views;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public class WPFNavigationService : IDesignNavigationService
    {
        public IRegionManager RegionManager { get; }
        public IRegionNavigationService CurrentRegion { get; }
        public Prism.Services.Dialogs.IDialogService PrismDialogService { get; }
        public INatCruiseDialogService DialogService { get; }

        public WPFNavigationService(IRegionManager regionManager, IRegionNavigationService currentRegion, Prism.Services.Dialogs.IDialogService prismDialogService, INatCruiseDialogService dialogService)
        {
            RegionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            CurrentRegion = currentRegion ?? throw new ArgumentNullException(nameof(currentRegion));
            PrismDialogService = prismDialogService ?? throw new ArgumentNullException(nameof(prismDialogService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public Task ShowFieldData(string cuttingUnit = null)
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(FieldDataView));
            return Task.CompletedTask;
        }

        public Task GoBackAsync()
        {
            CurrentRegion?.Journal.GoBack();
            return Task.CompletedTask;
        }

        public Task ShowCruiseSelect(string saleID)
        {
            throw new NotImplementedException();
        }

        public Task ShowFeedback()
        {
            throw new NotImplementedException();
        }

        public Task ShowSampleStateManagment()
        {
            throw new NotImplementedException();
        }

        public Task ShowSettings()
        {
            throw new NotImplementedException();
        }

        public Task ShowCruise()
        {

            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(CruiseView));
            return Task.CompletedTask;
        }

        public Task ShowStrata()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(StratumListView));
            return Task.CompletedTask;
        }

        public Task ShowCuttingUnitList()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(CuttingUnitListView));
            return Task.CompletedTask;
        }

        public Task ShowSale()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(SaleView));
            return Task.CompletedTask;
        }

        public Task ShowCruiseLandingLayout()
        {
            RegionManager.Regions[Regions.ContentRegion].RemoveAll();
            RegionManager.RequestNavigate(Regions.ContentRegion, nameof(CruiseMasterView));

            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(SaleView));
            return Task.CompletedTask;
        }

        public Task ShowTemplateLandingLayout()
        {
            RegionManager.Regions[Regions.ContentRegion].RemoveAll();
            RegionManager.RequestNavigate(Regions.ContentRegion, nameof(TemplateMasterView));

            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(TreeDefaultValueListView));
            return Task.CompletedTask;
        }

        public Task ShowTreeAuditRules()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(TreeAuditRuleListView));
            return Task.CompletedTask;
        }

        public Task ShowTreeDefaultValues()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(TreeDefaultValueListView));
            return Task.CompletedTask;
        }

        public Task ShowSpecies()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(SpeciesListView));
            return Task.CompletedTask;
        }

        public Task ShowDesignTemplates()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(StratumTemplateListView));
            return Task.CompletedTask;
        }

        public Task ShowTreeFields()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(TreeFieldsView));
            return Task.CompletedTask;
        }

        public Task ShowLogFields()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(LogFieldsView));
            return Task.CompletedTask;
        }

        public Task ShowDesignChecks()
        {
            RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(DesignChecksView));
            return Task.CompletedTask;
        }

        public Task ShowCombineFile()
        {
            RegionManager.RequestNavigate(Regions.ContentRegion, nameof(CombineFileView));
            return Task.CompletedTask;
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            return DialogService.ShowNotificationAsync("ThreePPNT tally not supported");
        }

        public Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber)
        {
            throw new NotImplementedException();
        }

        public Task ShowLogsList(string treeID)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            throw new NotImplementedException();
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowStratumDetail(string stratumCode)
        {
            throw new NotImplementedException();
        }
    }
}