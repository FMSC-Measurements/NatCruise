using NatCruise.Cruise.Services;
using FScruiser.XF.Services;
using Prism;
using Prism.Ioc;
using FScruiser.XF.Views;
using FScruiser.XF.ViewModels;
using NatCruise.Services;

namespace FScruiser.XF
{
    public class XamarinPlatformInitializer : IPlatformInitializer
    {
        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ICruiseNavigationService, XamarinNavigationService>();
            containerRegistry.Register<IDialogService, XamarinDialogService>();
            containerRegistry.RegisterSingleton<ITallySettingsDataService, TallySettingsDataService>();
            containerRegistry.RegisterSingleton<ICruiseDialogService, XamarinCruiseDialogService>();
            containerRegistry.RegisterInstance<ILoggingService>(new AppCenterLoggerService());

            RegisterViews(containerRegistry);
        }

        protected virtual void RegisterViews(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MyNavigationView>("Navigation"); // override built in navigation page with custom one
            containerRegistry.RegisterForNavigation<ImportView>("Import");
            containerRegistry.RegisterForNavigation<SaleSelectView>("SaleSelect");
            containerRegistry.RegisterForNavigation<CruiseSelectView>("CruiseSelect");
            containerRegistry.RegisterForNavigation<MainView>("Main");
            containerRegistry.RegisterForNavigation<SaleView>("Sale");
            containerRegistry.RegisterForNavigation<CuttingUnitListView>("CuttingUnitList");
            containerRegistry.RegisterForNavigation<TallyView>("Tally");
            containerRegistry.RegisterForNavigation<TreeListView>("TreeList");
            containerRegistry.RegisterForNavigation<TreeEditView>("Tree");
            containerRegistry.RegisterForNavigation<PlotListView>("PlotList");
            containerRegistry.RegisterForNavigation<PlotTallyView>("PlotTally");
            containerRegistry.RegisterForNavigation<FixCntTallyView>("FixCNTTally");
            containerRegistry.RegisterForNavigation<PlotEditView>("PlotEdit");
            containerRegistry.RegisterForNavigation<TreeCountEditView>("TreeCountEdit");
            containerRegistry.RegisterForNavigation<TreeErrorEditView>("TreeErrorEdit");
            containerRegistry.RegisterForNavigation<SampleStateManagmentView>("SampleStateManagment");
            containerRegistry.RegisterForNavigation<SampleStateManagmentOtherDevicesView>("SampleStateManagmentOther");

            containerRegistry.RegisterForNavigation<ThreePPNTPlotView>("ThreePPNTPlot");
            containerRegistry.RegisterForNavigation<ManageCruisersView>("Cruisers");
            containerRegistry.RegisterForNavigation<SettingsView>("Settings");
            containerRegistry.RegisterForNavigation<LimitingDistanceView>("LimitingDistance");
            containerRegistry.RegisterForNavigation<LogsListView>("LogList");
            containerRegistry.RegisterForNavigation<LogEditView>("Log");
            containerRegistry.RegisterForNavigation<FeedbackView>("Feedback");
        }


    }
}