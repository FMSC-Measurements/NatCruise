﻿using Microsoft.Extensions.Logging;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NatCruise.Util;
using FScruiser.Maui.Util;
using NatCruise.MVVM;
using FScruiser.Maui.Views;

namespace FScruiser.Maui.Services
{
    public class MauiNavigationService : ICruiseNavigationService
    {
        public MauiNavigationService(ILogger<MauiNavigationService> log, INavigationProvider navProvider, IServiceProvider services)
        {
            Log = log;
            Services = services;
            NavigationProvider = navProvider;
        }

        public Shell Shell => Shell.Current;

        public INavigationProvider NavigationProvider { get; }
        public INavigation Navigation => NavigationProvider.Navigation;
        public ILogger<MauiNavigationService> Log { get; }
        public IServiceProvider Services { get; }

        public Page MainView => NavigationProvider.MainPage;

        public Task GoBackAsync()
        {
            Log.TraceMethodCall();

            return Navigation.PopAsync();
        }

        public Task ShowView(Type tView, IDictionary<string, object> parameters = null, bool showModal = false)
        {
            if(!tView.IsAssignableTo(typeof(Page)))
            {
                throw new ArgumentException("tView must be a type of Page", nameof(tView));
            }

            var view = Services.GetRequiredService(tView) as Page;
            return ShowViewInternal(view, parameters, showModal);
        }

        public Task ShowView<TView>(IDictionary<string, object> parameters = null, bool showModal = false) where TView : Page
        {
            var view = Services.GetRequiredService<TView>();
            return ShowViewInternal(view, parameters, showModal);
        }

        protected Task ShowViewInternal(Page view, IDictionary<string, object> parameters = null, bool showModal = false)
        {
            parameters ??= new Dictionary<string, object>();

            Log?.LogInformation("Showing View {viewType}", view.GetType().Name);
            Log.LogInformation("Parameters: {parameters}",
                (parameters.Count == 0) ? "none" : String.Join(",\r\n", parameters.Select(x => x.Key + "=" + x.Value).ToArray()));

            var viewModel = view.BindingContext as ViewModelBase;

            if (viewModel != null)
            {
                viewModel.Initialize(parameters);
            }

            if (showModal)
            {
                return Navigation.PushModalAsync(view);
            }
            else
            {
                var mainView = MainView;
                if (mainView is FlyoutPage fp && fp is IFlyoutPageController fpc
                    && fpc.CanChangeIsPresented
                    && !fpc.ShouldShowSplitMode)
                {
                    fp.IsPresented = false;
                }

                return Navigation.PushAsync(view);
            }
        }

        public Task ShowAbout()
        {
            return ShowView<AboutView>();
        }

        public Task ShowBlank()
        {
            Log.TraceMethodCall();

            return Navigation.PopToRootAsync();

            //return ShowView<BlankView>();

            //return Shell.GoToAsync("Blank");
        }

        public Task ShowCruiseLandingLayout()
        {
            Log.TraceMethodCall();

            return Navigation.PopToRootAsync();

            //await ShowView<BlankView>();

            //return Shell.GoToAsync("//Blank");
        }

        public Task ShowCruiseSelect(string saleNumber)
        {

            var navParams = new Dictionary<string, object>() { { NavParams.SaleNumber, saleNumber } };
            return ShowView<CruiseSelectView>(navParams);
        }

        public Task ShowCuttingUnitInfo(string unitCode)
        {
            return ShowView<CuttingUnitInfoView>(new Dictionary<string, object>() { { NavParams.UNIT, unitCode } });
        }

        [Obsolete]
        public Task ShowCuttingUnitList()
        {
            Log.TraceMethodCall();

            throw new NotImplementedException();
        }

        public Task ShowDatabaseUtilities()
        {
            return ShowView<DatabaseUtilitiesView>(showModal: true);
        }

        [Obsolete]
        public Task ShowFeedback()
        {
            throw new NotImplementedException();
        }

        // not used on mobile although maybe we could 
        public Task ShowFieldData(string cuttingUnit = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            return ShowView<StratumFieldSetupView>(new Dictionary<string, object>() { { NavParams.STRATUM, stratumCode } });
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            return ShowView<FixCntTallyView>(new Dictionary<string, object>() { { NavParams.UNIT, unitCode }, { NavParams.PLOT_NUMBER, plotNumber }, { NavParams.STRATUM, stratumCode } });
        }

        public Task ShowImport()
        {
            return ShowView<ImportView>();
        }

        public Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber)
        {
            var navParams = new Dictionary<string, object>() { { NavParams.UNIT, unitCode }, { NavParams.STRATUM, stratumCode }, { NavParams.PLOT_NUMBER, plotNumber } };
            return ShowView<LimitingDistanceView>(navParams, showModal: true);
        }

        public Task ShowLimitingDistance()
        {
            return ShowView<LimitingDistanceView>();
        }

        public Task ShowLogEdit(string logID)
        {
            return ShowView<LogEditView>(new Dictionary<string, object> { { NavParams.LogID, logID } });
        }

        public Task ShowLogsList(string treeID)
        {
            return ShowView<LogsListView>(new Dictionary<string, object> { { NavParams.TreeID, treeID } });
        }

        public Task ShowManageCruisers()
        {
            return ShowView<ManageCruisersView>();
        }

        public Task ShowPlotEdit(string plotID)
        {
            return ShowView<PlotEditView>(new Dictionary<string, object> { { NavParams.PlotID, plotID } });
        }

        public Task ShowPlotList(string unitCode)
        {
            return ShowView<PlotListView>(new Dictionary<string, object> { { NavParams.UNIT, unitCode } });
        }

        public Task ShowPlotTally(string? plotID)
        {
            return ShowView<PlotTallyView>(new Dictionary<string, object> { { NavParams.PlotID, plotID } });
        }

        public Task ShowPlotTreeList(string unitCode)
        {
            // TODO implement plot trees page or consolidate into single trees page
            throw new NotImplementedException();
        }

        public Task ShowPrivacyPolicy()
        {
            return ShowView<PrivacyPolicyView>(showModal: true);
        }

        public Task ShowSale(string cruiseID)
        {
            return ShowView<SaleView>(new Dictionary<string, object> { { NavParams.CruiseID, cruiseID } });
        }

        public Task ShowSaleSelect()
        {
            return ShowView<SaleSelectView>();
        }

        public Task ShowSampleGroups(string stratumCode)
        {
            return ShowView<SampleGroupListView>(new Dictionary<string, object> { { NavParams.STRATUM, stratumCode } });
        }

        [Obsolete]
        public Task ShowSampleStateManagment()
        {
            throw new NotImplementedException();
        }

        public Task ShowSettings()
        {
            return ShowView<SettingsView>();
        }

        public Task ShowStrata()
        {
            return ShowView<StratumListView>();
        }

        public Task ShowStratumInfo(string stratumCode)
        {
            // on mobile we don't have a full details view for stratum
            throw new NotImplementedException();
        }

        public Task ShowSubpopulations(string stratumCode, string sampleGroupCode)
        {
            return ShowView<SubpopulationListView>(new Dictionary<string, object> { { NavParams.STRATUM, stratumCode }, { NavParams.SAMPLE_GROUP, sampleGroupCode } });
        }

        public Task ShowTally(string unitCode)
        {
            var navParams = new Dictionary<string, object>() { { NavParams.UNIT, unitCode } };
            return ShowView<TallyView>(navParams);
        }

        public Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            return ShowView<TallyPopulationDetailsView>(new Dictionary<string, object> {
                { NavParams.UNIT, unitCode },
                { NavParams.STRATUM, stratumCode },
                { NavParams.PLOT_NUMBER, plotNumber },
                { NavParams.SAMPLE_GROUP, sampleGroupCode },
                { NavParams.SPECIES, species },
                { NavParams.LIVE_DEAD, liveDead }
            });
        }

        public Task ShowTallyPopulationInfo(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            return ShowView<TallyPopulationDetailsView>(new Dictionary<string, object> {
                { NavParams.UNIT, unitCode },
                { NavParams.STRATUM, stratumCode },
                { NavParams.SAMPLE_GROUP, sampleGroupCode },
                { NavParams.SPECIES, species },
                { NavParams.LIVE_DEAD, liveDead }
            });
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            return ShowView<ThreePPNTPlotView>(new Dictionary<string, object> { { NavParams.UNIT, unitCode }, { NavParams.STRATUM, stratumCode }, { NavParams.PLOT_NUMBER, plotNumber } });
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            return ShowView<TreeAuditRuleEditView>(new Dictionary<string, object> { { NavParams.TreeAuditRuleID, tarID } });
        }

        public Task ShowTreeAuditRules()
        {
            return ShowView<TreeAuditRuleListView>();
        }

        public Task ShowTreeEdit(string treeID)
        {
            return ShowView<TreeEditView>(new Dictionary<string, object>() { { NavParams.TreeID, treeID } });
        }

        public Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID)
        {
            Log.TraceMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("TreeErrorEdit?" + $"{NavParams.TreeID}={treeID}&{NavParams.TreeAuditRuleID}={treeAuditRuleID}");
        }

        public Task ShowTreeList(string unitCode)
        {
            return ShowView<TreeListPage>(new Dictionary<string, object>() { { NavParams.UNIT, unitCode } });
        }

        public Task ShowUserAgreement()
        {
            Log.TraceMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("//UserAgreement");
        }

        public Task ShowUtilities()
        {
            return ShowView<UtilitiesView>();
        }
    }
}
