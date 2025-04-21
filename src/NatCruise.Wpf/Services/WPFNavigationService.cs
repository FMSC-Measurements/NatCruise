using DryIoc;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NatCruise.Design.Views;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Wpf.CheckCruise.Views;
using NatCruise.Wpf.DialogViews;
using NatCruise.Wpf.FieldData.Views;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Views;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        public IServiceProvider Services { get; }
        public ILogger Log { get; }

        public MainWindow MainWindow => (MainWindow)AppService.MainWindow;

        public WPFNavigationService(IRegionManager regionManager,
                                    ILogger<WPFNavigationService> logger,
                                    IServiceProvider services,
                                    IRegionNavigationService currentRegion,
                                    Prism.Services.Dialogs.IDialogService prismDialogService,
                                    INatCruiseDialogService dialogService,
                                    IContainerProvider containerProvider,
                                    IAppService appService)
        {
            Log = logger;
            Services = services ?? throw new ArgumentNullException(nameof(services));
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

        public Task ShowCreateCheckCruise()
        {
            return ShowView<CreateCheckCruiseView>();

            //RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(CreateCheckCruiseView));
            //return Task.CompletedTask;
        }

        public Task ShowCheckCruiseReports()
        {
            return ShowView<CheckCruiseReportsView>();

            //RegionManager.RequestNavigate(Regions.CruiseContentRegion, nameof(CheckCruiseReportsView));
            //return Task.CompletedTask;
        }



        // Show view methods try to replace the navigation service used by prism
        // and provice a common set of methods that can be used across newer IOC
        // used by MAUI 

        public Task ShowView(Type tView, IDictionary<string, object> parameters = null, bool showModal = false)
        {
            if (!tView.IsAssignableTo(typeof(FrameworkElement)))
            {
                throw new ArgumentException("tView must be a type of Page", nameof(tView));
            }

            parameters ??= new Dictionary<string, object>();


            Log?.LogInformation("Showing View {viewType}", tView.Name);
            Log.LogInformation("Parameters: {parameters}",
                (parameters.Count == 0) ? "none" : String.Join(",\r\n", parameters.Select(x => x.Key + "=" + x.Value).ToArray()));



            if (showModal)
            {
                throw new NotImplementedException();
                //return Navigation.PushModalAsync(view);
            }
            else
            {
                var region = RegionManager.Regions[Regions.CruiseContentRegion];

                var view = region.Views.FirstOrDefault(x => x.GetType() == tView) as FrameworkElement;
                if (view == null)
                {
                    view = Services.GetRequiredService(tView) as FrameworkElement;
                    region.Add(view);
                }

                var viewModel = view.DataContext as ViewModelBase;

                // TODO industrialize VM async
                if (viewModel != null)
                {
                    viewModel.Initialize(parameters);
                }

                region.Activate(view);

                return Task.CompletedTask;
            }
        }

        public Task ShowView<TView>(IDictionary<string, object> parameters = null, bool showModal = false) where TView : FrameworkElement
        {
            var viewType = typeof(TView);

            return ShowView(viewType, parameters, showModal);
        }

    }
}