using FScruiser.XF.Services;
using Prism;
using Prism.Ioc;
using FScruiser.XF.Views;
using FScruiser.XF.ViewModels;
using NatCruise.Services;
using NatCruise.Navigation;
using NatCruise.MVVM;
using NatCruise.Data;
using FScruiser.XF.Data;
using NatCruise.Logic;

namespace FScruiser.XF
{
    public class XamarinPlatformInitializer : IPlatformInitializer
    {
        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDataContextService, DataContextService>();
            containerRegistry.Register<IFileDialogService, XamarinFileDialogService>();
            containerRegistry.Register<IApplicationSettingService, XamarinApplicationSettingService>();
            containerRegistry.Register<ICruiseNavigationService, XamarinNavigationService>();
            containerRegistry.Register<INatCruiseNavigationService, XamarinNavigationService>();
            containerRegistry.Register<INatCruiseDialogService, XamarinDialogService>();
            containerRegistry.RegisterSingleton<ITallySettingsDataService, TallySettingsDataService>();
            containerRegistry.RegisterInstance<ILoggingService>(new AppCenterLoggerService());
            containerRegistry.Register<IPlotTallyService, PlotTallyService>();
            containerRegistry.Register<ITreeBasedTallyService, TreeBasedTallyService>();
            containerRegistry.RegisterInstance<System.Random>(FMSC.Sampling.MersenneTwister.Instance);

            containerRegistry.RegisterSingleton<ISetupInfoDataservice, SetupInfoDataservice>();

            RegisterViews(containerRegistry);
        }

        protected virtual void RegisterViews(IContainerRegistry containerRegistry)
        {
            // override the built in navigation page with custom one
            // this is needed to set the color of the navigation bar and the navigation bar text color
            containerRegistry.RegisterForNavigation<MyNavigationView>("Navigation");

            containerRegistry.RegisterForNavigation<BlankView>("Blank");
            containerRegistry.RegisterForNavigation<ImportView>("Import");
            containerRegistry.RegisterForNavigation<SaleSelectView>("SaleSelect");
            containerRegistry.RegisterForNavigation<CruiseSelectView>("CruiseSelect");
            containerRegistry.RegisterForNavigation<MainView>("Main");
            containerRegistry.RegisterForNavigation<SaleView>("Sale");
            containerRegistry.RegisterForNavigation<CuttingUnitListView>("CuttingUnitList");
            containerRegistry.RegisterForNavigation<CuttingUnitInfoView, CuttingUnitInfoViewModel>("CuttingUnitInfo");
            containerRegistry.RegisterForNavigation<TallyView>("Tally");
            containerRegistry.RegisterForNavigation<TreeListView>("TreeList");
            containerRegistry.RegisterForNavigation<TreeEditView>("Tree");
            containerRegistry.RegisterForNavigation<PlotListView>("PlotList");
            containerRegistry.RegisterForNavigation<PlotTallyView>("PlotTally");
            containerRegistry.RegisterForNavigation<PlotTreeListView>("PlotTreeList");
            containerRegistry.RegisterForNavigation<FixCntTallyView, FixCNTTallyViewModel>("FixCNTTally");
            containerRegistry.RegisterForNavigation<PlotEditView>("PlotEdit");
            containerRegistry.RegisterForNavigation<TreeCountEditView>("TreeCountEdit");
            containerRegistry.RegisterForNavigation<TreeErrorEditView>("TreeErrorEdit");
            containerRegistry.RegisterForNavigation<SampleStateManagmentView>("SampleStateManagment");
            containerRegistry.RegisterForNavigation<SampleStateManagmentOtherDevicesView>("SampleStateManagmentOther");

            containerRegistry.RegisterForNavigation<ThreePPNTPlotView>("ThreePPNTPlot");
            containerRegistry.RegisterForNavigation<ManageCruisersView>("Cruisers");
            containerRegistry.RegisterForNavigation<SettingsView>("Settings");
            containerRegistry.RegisterForNavigation<PrivacyPolicyView>("PrivacyPolicy");
            containerRegistry.RegisterForNavigation<UserAgreementView>("UserAgreement");
            containerRegistry.RegisterForNavigation<LimitingDistanceView>("LimitingDistance");
            containerRegistry.RegisterForNavigation<LogsListView>("LogList");
            containerRegistry.RegisterForNavigation<LogEditView>("LogEdit");
            containerRegistry.RegisterForNavigation<FeedbackView>("Feedback");
            containerRegistry.RegisterForNavigation<UtilitiesView>("Utilities");
            containerRegistry.RegisterForNavigation<DatabaseUtilitiesView>("DatabaseUtilities");
            containerRegistry.RegisterForNavigation<TreeAuditRuleListView>("TreeAuditRuleList");
            containerRegistry.RegisterForNavigation<TreeAuditRuleEditView>("TreeAuditRuleEdit");
            containerRegistry.RegisterForNavigation<StratumListView>("Strata");
            containerRegistry.RegisterForNavigation<StratumInfoView>("StratumInfo");
            containerRegistry.RegisterForNavigation<SampleGroupListView>("SampleGroups");
            containerRegistry.RegisterForNavigation<SubpopulationListView>("Subpopulations");
            containerRegistry.RegisterForNavigation<StratumFieldSetupView>("FieldSetup");
            containerRegistry.RegisterForNavigation<TallyPopulationDetailsView>("TallyPopulationDetails");
            containerRegistry.RegisterForNavigation<AboutView>("About");

        }

    }
}