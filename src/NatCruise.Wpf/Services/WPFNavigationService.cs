using MahApps.Metro.Controls.Dialogs;
using NatCruise.Design.Views;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Wpf.DialogViews;
using NatCruise.Wpf.FieldData.Views;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Views;
using Prism.Ioc;
using Prism.Regions;
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

        public IAppService AppService { get; }
        public IContainerProvider ContainerProvider { get; }

        public MainWindow MainWindow => (MainWindow)AppService.MainWindow;

        public WPFNavigationService(IRegionManager regionManager,
                                    IRegionNavigationService currentRegion,
                                    Prism.Services.Dialogs.IDialogService prismDialogService,
                                    INatCruiseDialogService dialogService,
                                    IContainerProvider containerProvider,
                                    IAppService appService)
        {
            RegionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            CurrentRegion = currentRegion ?? throw new ArgumentNullException(nameof(currentRegion));
            PrismDialogService = prismDialogService ?? throw new ArgumentNullException(nameof(prismDialogService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            AppService = appService ?? throw new ArgumentNullException(nameof(appService));
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
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
            if (string.IsNullOrEmpty(treeID))
            {
                throw new ArgumentException($"'{nameof(treeID)}' cannot be null or empty.", nameof(treeID));
            }

            if (string.IsNullOrEmpty(treeAuditRuleID))
            {
                throw new ArgumentException($"'{nameof(treeAuditRuleID)}' cannot be null or empty.", nameof(treeAuditRuleID));
            }

            var view = new TreeErrorEditView();
            Prism.Mvvm.ViewModelLocator.SetAutoWireViewModel(view, true);
            var viewModel = (TreeErrorEditViewModel)view.DataContext;
            var dialog = new CustomDialog()
            {
                Title = viewModel.Message,
                Content = view,
            };

            EventHandler hideDialogAction = (sender, ea) => MainWindow.HideMetroDialogAsync(dialog);
            viewModel.Saved = hideDialogAction;
            view.OnCancelButtonClicked = hideDialogAction;

            viewModel.Load(treeID, treeAuditRuleID);
            viewModel.IsResolved = !viewModel.IsResolved; 
            return MainWindow.ShowMetroDialogAsync(dialog);

            //await MainWindow.ShowMetroDialogAsync(dialog);
            //await dialog.WaitUntilUnloadedAsync();

            //return;
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            throw new NotImplementedException();
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowStratumInfo(string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowSampleGroups(string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowSubpopulations(string stratumCode, string sampleGroupCode)
        {
            throw new NotImplementedException();
        }

        public async Task ShowAbout()
        {
            var window = MainWindow;
            var settings = window.MetroDialogOptions;
            var appSettings = ContainerProvider.Resolve<IWpfApplicationSettingService>();
            var dialog = new AboutDialog(window, settings, appSettings)
            {
                Title = "About",
            };

            await window.ShowMetroDialogAsync(dialog, settings);

            // discard result
            _ = await dialog.WaitForResult();

            await window.HideMetroDialogAsync(dialog, settings);
        }

        public Task ShowPrivacyPolicy()
        {
            throw new NotImplementedException();
        }

        public Task ShowUserAgreement()
        {
            throw new NotImplementedException();
        }

        public Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            throw new NotImplementedException();
        }

        public Task ShowTallyPopulationInfo(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            throw new NotImplementedException();
        }

        public Task ShowLimitingDistance()
        {
            throw new NotImplementedException();
        }
    }
}