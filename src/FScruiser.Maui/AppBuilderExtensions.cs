using CommunityToolkit.Maui;
using FScruiser.Maui.Data;
using FScruiser.Maui.Services;
using FScruiser.Maui.ViewModels;
using FScruiser.Maui.Views;
using Microsoft.Maui.LifecycleEvents;
using NatCruise.Data;
using NatCruise.Logic;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui
{
    internal static class AppBuilderExtensions
    {
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            var services = builder.Services;

            services.AddTransient<MainView>();// should only be instantiated once, maybe change to singleton but for now I might add a instance counter to check that assumption

            services.AddTransient<AboutView>();
            services.AddTransient<AskKpiView>();
            services.AddTransient<BlankView>();
            services.AddTransient<CruiseSelectView>();
            services.AddTransient<CuttingUnitInfoView>();
            services.AddTransient<DatabaseUtilitiesView>();
            services.AddTransient<FixCntTallyView>();
            services.AddTransient<ImportView>();
            services.AddTransient<LimitingDistanceView>();
            services.AddTransient<LogEditView>();
            services.AddTransient<LogsListView>();
            services.AddTransient<ManageCruisersView>();
            services.AddTransient<PlotEditView>();
            services.AddTransient<PlotListView>();
            services.AddTransient<PlotTallyView>();
            services.AddTransient<PrivacyPolicyView>();
            services.AddTransient<SaleSelectView>();
            services.AddTransient<SaleView>();
            services.AddTransient<SampleGroupListView>();
            services.AddTransient<SettingsView>();
            services.AddTransient<StratumListView>();
            services.AddTransient<StratumFieldSetupView>();
            services.AddTransient<StratumLogFieldSetupView>();
            services.AddTransient<SubpopulationListView>();
            services.AddTransient<TallyView>();
            services.AddTransient<TallyPopulationDetailsView>();
            services.AddTransient<ThreePPNTPlotView>();
            services.AddTransient<TreeAuditRuleEditView>();
            services.AddTransient<TreeAuditRuleListView>();
            services.AddTransient<TreeEditView>();
            services.AddTransient<TreeListPage>();
            services.AddTransient<UtilitiesView>();

#if DEBUG
            services.AddTransient<TestDialogServiceView>();
#endif

            //Routing.RegisterRoute("Blank", typeof(BlankView));
            //Routing.RegisterRoute("Import", typeof(ImportView));
            //Routing.RegisterRoute("SaleSelect", typeof(SaleSelectView));
            //Routing.RegisterRoute("/SaleSelect/CruiseSelect", typeof(CruiseSelectView));
            //Routing.RegisterRoute("DatabaseUtilities", typeof(DatabaseUtilitiesView));

            return builder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            var services = builder.Services;

            // NatCruise.Core view models
            services.AddTransient<AboutViewModel>();
            services.AddTransient<LogEditViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<SpeciesDetailViewModel>();
            services.AddTransient<StratumInfoViewModel>();
            services.AddTransient<StratumLogFieldSetupViewModel>();
            services.AddTransient<StratumTreeFieldSetupViewModel>();
            services.AddTransient<SubpopulationListViewModel>();
            services.AddTransient<TallyPopulationDetailsViewModel>();
            services.AddTransient<TreeAuditRuleEditViewModel>();
            services.AddTransient<TreeAuditRuleListViewModel>();
            services.AddTransient<TreeCountEditViewModel>();
            services.AddTransient<TreeEditViewModel>();
            services.AddTransient<TreeErrorEditViewModel>();

            // FScruiser.Maui view models
            services.AddTransient<CruiseSelectViewModel>();
            services.AddTransient<CuttingUnitInfoViewModel>();
            services.AddTransient<DatabaseUtilitiesViewModel>();
            services.AddTransient<FixCNTTallyViewModel>();
            services.AddTransient<ImportViewModel>();
            services.AddTransient<LimitingDistanceViewModel>();
            services.AddTransient<LogsListViewModel>();
            services.AddTransient<ManageCruisersViewModel>();
            services.AddTransient<PlotEditViewModel>();
            services.AddTransient<PlotListViewModel>();
            services.AddTransient<PlotTallyViewModel>();
            services.AddTransient<PlotTreeListViewModel>();
            services.AddTransient<SaleSelectViewModel>();
            services.AddTransient<SaleViewModel>();
            services.AddTransient<SampleGroupListViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<StratumFieldSetupViewModel>();
            services.AddTransient<StratumLogFieldSetupViewModel>();
            services.AddTransient<StratumTreeFieldSetupViewModel>();
            services.AddTransient<StratumListViewModel>();
            services.AddTransient<TallyViewModel>();
            services.AddTransient<ThreePPNTPlotViewModel>();
            services.AddTransient<TreeListViewModel>();

            return builder;
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<Random>(FMSC.Sampling.MersenneTwister.Instance);
            services.AddSingleton<INavigationProvider, NavigationProvider>();
            services.AddTransient<INavigation>(s => s.GetRequiredService<INavigationProvider>().Navigation);

            services.AddSingleton<IDataContextService, DataContextService>();

            services.AddSingleton<ICruisersDataservice, CruisersDataservice>();

            services.AddSingleton<ICruiseNavigationService, MauiNavigationService>();
            services.AddTransient<INatCruiseNavigationService>((x) => x.GetRequiredService<ICruiseNavigationService>());

            services.AddSingleton<INatCruiseDialogService, MauiDialogService>();
            services.AddSingleton<IApplicationSettingService, MauiApplicationSettingService>();
            //services.AddSingleton<IDataserviceProvider, DataserviceProviderBase>();

            services.AddSingleton<ITallySettingsDataService, TallySettingsDataService>();

            services.AddTransient<ITreeBasedTallyService, TreeBasedTallyService>();
            services.AddTransient<IPlotTallyService, PlotTallyService>();

            services.AddTransient<ILoggingService, AppCenterLoggerService>();

            services.AddTransient<IPreferences>(x => Microsoft.Maui.Storage.Preferences.Default);

            services.AddTransient<TreeFieldSetupValidator>();

            return builder;
        }
    }
}