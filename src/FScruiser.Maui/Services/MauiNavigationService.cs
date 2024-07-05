using Microsoft.Extensions.Logging;
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
            Log.LogMethodCall();

            return Navigation.PopAsync();

            //return Shell.Navigation.PopAsync();
            //return Shell.GoToAsync("..");
        }

        public Task ShowView<TView>(IDictionary<string, object> parameters = null, bool showModal = false) where TView : Page
        {
            var view = Services.GetRequiredService<TView>();
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
                if(mainView is FlyoutPage fp)
                {
                    fp.IsPresented = false;
                }

                return Navigation.PushAsync(view);
            }
        }

        public Task ShowAbout()
        {
            Log.LogMethodCall();

            return ShowView<AboutView>(showModal: true);
            //return Shell.GoToAsync("//About");
        }

        public Task ShowBlank()
        {
            Log.LogMethodCall();

            return Navigation.PopToRootAsync();

            //return ShowView<BlankView>();

            //return Shell.GoToAsync("Blank");
        }

        public Task ShowCruiseLandingLayout()
        {
            Log.LogMethodCall();

            return Navigation.PopToRootAsync();

            //await ShowView<BlankView>();

            //return Shell.GoToAsync("//Blank");
        }

        public Task ShowCruiseSelect(string saleNumber)
        {
            Log.LogMethodCall();

            var navParams = new Dictionary<string, object>() { { NavParams.SaleNumber, saleNumber } };
            return ShowView<CruiseSelectView>(navParams);

            //return Shell.Current.GoToAsync($"CruiseSelect");
            //return Shell.Current.GoToAsync($"CruiseSelect?" + $"{NavParams.SaleNumber}={saleNumber}");
        }

        public Task ShowCuttingUnitInfo(string unitCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//CuttingUnitInfo?" + $"{NavParams.UNIT}={unitCode}");
        }

        [Obsolete]
        public Task ShowCuttingUnitList()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
        }

        public Task ShowDatabaseUtilities()
        {
            Log.LogMethodCall();

            return ShowView<DatabaseUtilitiesView>(showModal:true);
        }

        [Obsolete]
        public Task ShowFeedback()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
        }

        public Task ShowFieldData(string cuttingUnit = null)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("FieldSetup?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            Log.LogMethodCall();
            throw new NotImplementedException();
            return Shell.GoToAsync("FixCNTTally?" + $"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}&{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowImport()
        {
            Log.LogMethodCall();

            return ShowView<ImportView>();
        }

        public Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber)
        {
            Log.LogMethodCall();

            var navParams = new Dictionary<string, object>() { { NavParams.UNIT, unitCode }, { NavParams.STRATUM, stratumCode }, { NavParams.PLOT_NUMBER, plotNumber } };
            return ShowView<LimitingDistanceView>(navParams, showModal: true);
        }

        public Task ShowLimitingDistance()
        {
            Log.LogMethodCall();

            return ShowView<LimitingDistanceView>();
        }

        public Task ShowLogEdit(string logID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("LogEdit?" + $"{NavParams.LogID}={logID}");
        }

        public Task ShowLogsList(string treeID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("LogList?" + $"{NavParams.TreeID}={treeID}");
        }

        public Task ShowManageCruisers()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("//Cruisers");
        }

        public Task ShowPlotEdit(string plotID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("PlotEdit?" + $"{NavParams.PlotID}={plotID}");
        }

        public Task ShowPlotList(string unitCode)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("//PlotList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowPlotTally(string? plotID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("PlotTally?" + $"{NavParams.PlotID}={plotID}");
        }

        public Task ShowPlotTreeList(string unitCode)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("//PlotTreeList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowPrivacyPolicy()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("PrivacyPolicy");
        }

        public Task ShowSale(string cruiseID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("Sale?" + $"{NavParams.CruiseID}={cruiseID}");
        }

        public async Task ShowSaleSelect()
        {
            Log.LogMethodCall();

            await ShowView<SaleSelectView>();

            //return Shell.GoToAsync($"//Main/SaleSelect");
        }

        public Task ShowSampleGroups(string stratumCode)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("SampleGroups?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        [Obsolete]
        public Task ShowSampleStateManagment()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("SampleStateManagment");
        }

        public Task ShowSettings()
        {
            Log.LogMethodCall();

            return ShowView<SettingsView>();
        }

        public Task ShowStrata()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("//Strata");
        }

        public Task ShowStratumInfo(string stratumCode)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("StratumInfo?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowSubpopulations(string stratumCode, string sampleGroupCode)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("Subpopulations?" + $"{NavParams.STRATUM}={stratumCode}&{NavParams.SAMPLE_GROUP}={sampleGroupCode}");
        }

        public Task ShowTally(string unitCode)
        {
            Log.LogMethodCall();

            var navParams = new Dictionary<string, object>() { {NavParams.UNIT, unitCode } };
            return ShowView<TallyView>(navParams);
        }

        public Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            Log.LogMethodCall();

            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
               $"&{NavParams.PLOT_NUMBER}={plotNumber}" +
               $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
               $"&{NavParams.SPECIES}={species}" +
               $"&{NavParams.LIVE_DEAD}={liveDead}";

            throw new NotImplementedException();
            return Shell.GoToAsync("TallyPopulationDetails?" + parameters);
        }

        public Task ShowTallyPopulationInfo(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            Log.LogMethodCall();

            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}";

            throw new NotImplementedException();
            return Shell.GoToAsync("TallyPopulationDetails?" + parameters);
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("ThreePPNTPlot?" + $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}");
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("TreeAuditRuleEdit?" + $"{NavParams.TreeAuditRuleID}={tarID}");
        }

        public Task ShowTreeAuditRules()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("TreeAuditRuleList");
        }

        public Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            Log.LogMethodCall();

            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}";

            throw new NotImplementedException();
            return Shell.GoToAsync("TreeCountEdit?" + parameters);
        }

        public Task ShowTreeEdit(string treeID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("Tree?" + $"{NavParams.TreeID}={treeID}");
        }

        public Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("TreeErrorEdit?" + $"{NavParams.TreeID}={treeID}&{NavParams.TreeAuditRuleID}={treeAuditRuleID}");
        }

        public Task ShowTreeList(string unitCode)
        {
            Log.LogMethodCall();

            return ShowView<TreeListPage>(new Dictionary<string, object>() {{ NavParams.UNIT, unitCode }});

            //return Shell.GoToAsync("TreeList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowUserAgreement()
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
            return Shell.GoToAsync("//UserAgreement");
        }

        public Task ShowUtilities()
        {
            Log.LogMethodCall();
            return ShowView<UtilitiesView>(showModal: true);
        }
    }
}
