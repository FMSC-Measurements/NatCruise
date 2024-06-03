using Microsoft.Extensions.Logging;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NatCruise.Util;
using FScruiser.Maui.Util;

namespace FScruiser.Maui.Services
{
    public class MauiNavigationService : ICruiseNavigationService
    {
        public MauiNavigationService(ILogger<MauiNavigationService> log)
        {
            Log = log;
        }

        public Shell Shell => Shell.Current;
            
        public IServiceProvider ServiceProvider { get; }
        public ILogger<MauiNavigationService> Log { get; }

        public Task GoBackAsync()
        {
            Log.LogMethodCall();

            //return Shell.Navigation.PopAsync();
            return Shell.GoToAsync("..");
        }

        public Task ShowAbout()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//About");
        }

        public Task ShowBlank()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("Blank");
        }

        public Task ShowCruiseLandingLayout()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//Blank");
        }

        public Task ShowCruiseSelect(string saleNumber)
        {
            Log.LogMethodCall();

            //return Shell.Current.GoToAsync($"CruiseSelect");
            return Shell.Current.GoToAsync($"CruiseSelect?" + $"{NavParams.SaleNumber}={saleNumber}");
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

            return Shell.GoToAsync("CuttingUnitList");
        }

        public Task ShowDatabaseUtilities()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("DatabaseUtilities");
        }

        [Obsolete]
        public Task ShowFeedback()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("FeedBack");
        }

        public Task ShowFieldData(string cuttingUnit = null)
        {
            Log.LogMethodCall();

            throw new NotImplementedException();
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("FieldSetup?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("FixCNTTally?" + $"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}&{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowImport()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("Import");
        }

        public Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("LimitingDistance?" + $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}");
        }

        public Task ShowLimitingDistance()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("LimitingDistance");
        }

        public Task ShowLogEdit(string logID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("LogEdit?" + $"{NavParams.LogID}={logID}");
        }

        public Task ShowLogsList(string treeID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("LogList?" + $"{NavParams.TreeID}={treeID}");
        }

        public Task ShowManageCruisers()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//Cruisers");
        }

        public Task ShowPlotEdit(string plotID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("PlotEdit?" + $"{NavParams.PlotID}={plotID}");
        }

        public Task ShowPlotList(string unitCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//PlotList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowPlotTally(string? plotID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("PlotTally?" + $"{NavParams.PlotID}={plotID}");
        }

        public Task ShowPlotTreeList(string unitCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//PlotTreeList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowPrivacyPolicy()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("PrivacyPolicy");
        }

        public Task ShowSale(string cruiseID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("Sale?" + $"{NavParams.CruiseID}={cruiseID}");
        }

        public Task ShowSaleSelect()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync($"//Main/SaleSelect");
        }

        public Task ShowSampleGroups(string stratumCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("SampleGroups?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        [Obsolete]
        public Task ShowSampleStateManagment()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("SampleStateManagment");
        }

        public Task ShowSettings()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//Settings");
        }

        public Task ShowStrata()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//Strata");
        }

        public Task ShowStratumInfo(string stratumCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("StratumInfo?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowSubpopulations(string stratumCode, string sampleGroupCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("Subpopulations?" + $"{NavParams.STRATUM}={stratumCode}&{NavParams.SAMPLE_GROUP}={sampleGroupCode}");
        }

        public Task ShowTally(string unitCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//Tally?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            Log.LogMethodCall();

            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
               $"&{NavParams.PLOT_NUMBER}={plotNumber}" +
               $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
               $"&{NavParams.SPECIES}={species}" +
               $"&{NavParams.LIVE_DEAD}={liveDead}";

            return Shell.GoToAsync("TallyPopulationDetails?" + parameters);
        }

        public Task ShowTallyPopulationInfo(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            Log.LogMethodCall();

            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}";

            return Shell.GoToAsync("TallyPopulationDetails?" + parameters);
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("ThreePPNTPlot?" + $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}");
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("TreeAuditRuleEdit?" + $"{NavParams.TreeAuditRuleID}={tarID}");
        }

        public Task ShowTreeAuditRules()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("TreeAuditRuleList");
        }

        public Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            Log.LogMethodCall();

            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}";

            return Shell.GoToAsync("TreeCountEdit?" + parameters);
        }

        public Task ShowTreeEdit(string treeID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("Tree?" + $"{NavParams.TreeID}={treeID}");
        }

        public Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("TreeErrorEdit?" + $"{NavParams.TreeID}={treeID}&{NavParams.TreeAuditRuleID}={treeAuditRuleID}");
        }

        public Task ShowTreeList(string unitCode)
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("TreeList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowUserAgreement()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//UserAgreement");
        }

        public Task ShowUtilities()
        {
            Log.LogMethodCall();

            return Shell.GoToAsync("//Utilities");
        }
    }
}
