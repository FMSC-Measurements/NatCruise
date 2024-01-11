using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Services
{
    public class MauiNavigationService : ICruiseNavigationService
    {
        public MauiNavigationService()
        {
        }

        public Shell Shell => Shell.Current;
            
        public IServiceProvider ServiceProvider { get; }

        public Task GoBackAsync()
        {
            //return Shell.Navigation.PopAsync();
            return Shell.GoToAsync("..");
        }

        public Task ShowAbout()
        {
            return Shell.GoToAsync("//About");
        }

        public Task ShowBlank()
        {
            return Shell.GoToAsync("Blank");
        }

        public Task ShowCruiseLandingLayout()
        {
            return Shell.GoToAsync("//Blank");
        }

        public Task ShowCruiseSelect(string saleNumber)
        {
            //return Shell.Current.GoToAsync($"CruiseSelect");
            return Shell.Current.GoToAsync($"CruiseSelect?" + $"{NavParams.SaleNumber}={saleNumber}");
        }

        public Task ShowCuttingUnitInfo(string unitCode)
        {
            return Shell.GoToAsync("//CuttingUnitInfo?" + $"{NavParams.UNIT}={unitCode}");
        }

        [Obsolete]
        public Task ShowCuttingUnitList()
        {
            return Shell.GoToAsync("CuttingUnitList");
        }

        public Task ShowDatabaseUtilities()
        {
            return Shell.GoToAsync("DatabaseUtilities");
        }

        [Obsolete]
        public Task ShowFeedback()
        {
            return Shell.GoToAsync("FeedBack");
        }

        public Task ShowFieldData(string cuttingUnit = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            return Shell.GoToAsync("FieldSetup?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            return Shell.GoToAsync("FixCNTTally?" + $"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}&{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowImport()
        {
            return Shell.GoToAsync("Import");
        }

        public Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber)
        {
            return Shell.GoToAsync("LimitingDistance?" + $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}");
        }

        public Task ShowLimitingDistance()
        {
            return Shell.GoToAsync("LimitingDistance");
        }

        public Task ShowLogEdit(string logID)
        {
            return Shell.GoToAsync("LogEdit?" + $"{NavParams.LogID}={logID}");
        }

        public Task ShowLogsList(string treeID)
        {
            return Shell.GoToAsync("LogList?" + $"{NavParams.TreeID}={treeID}");
        }

        public Task ShowManageCruisers()
        {
            return Shell.GoToAsync("//Cruisers");
        }

        public Task ShowPlotEdit(string plotID)
        {
            return Shell.GoToAsync("PlotEdit?" + $"{NavParams.PlotID}={plotID}");
        }

        public Task ShowPlotList(string unitCode)
        {
            return Shell.GoToAsync("//PlotList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowPlotTally(string? plotID)
        {
            return Shell.GoToAsync("PlotTally?" + $"{NavParams.PlotID}={plotID}");
        }

        public Task ShowPlotTreeList(string unitCode)
        {
            return Shell.GoToAsync("//PlotTreeList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowPrivacyPolicy()
        {
            return Shell.GoToAsync("PrivacyPolicy");
        }

        public Task ShowSale(string cruiseID)
        {
            return Shell.GoToAsync("Sale?" + $"{NavParams.CruiseID}={cruiseID}");
        }

        public Task ShowSaleSelect()
        {
            return Shell.GoToAsync($"SaleSelect");
        }

        public Task ShowSampleGroups(string stratumCode)
        {
            return Shell.GoToAsync("SampleGroups?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        [Obsolete]
        public Task ShowSampleStateManagment()
        {
            return Shell.GoToAsync("SampleStateManagment");
        }

        public Task ShowSettings()
        {
            return Shell.GoToAsync("//Settings");
        }

        public Task ShowStrata()
        {
            return Shell.GoToAsync("//Strata");
        }

        public Task ShowStratumInfo(string stratumCode)
        {
            return Shell.GoToAsync("StratumInfo?" + $"{NavParams.STRATUM}={stratumCode}");
        }

        public Task ShowSubpopulations(string stratumCode, string sampleGroupCode)
        {
            return Shell.GoToAsync("Subpopulations?" + $"{NavParams.STRATUM}={stratumCode}&{NavParams.SAMPLE_GROUP}={sampleGroupCode}");
        }

        public Task ShowTally(string unitCode)
        {
            return Shell.GoToAsync("//Tally?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
               $"&{NavParams.PLOT_NUMBER}={plotNumber}" +
               $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
               $"&{NavParams.SPECIES}={species}" +
               $"&{NavParams.LIVE_DEAD}={liveDead}";

            return Shell.GoToAsync("TallyPopulationDetails?" + parameters);
        }

        public Task ShowTallyPopulationInfo(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}";

            return Shell.GoToAsync("TallyPopulationDetails?" + parameters);
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            return Shell.GoToAsync("ThreePPNTPlot?" + $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}");
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            return Shell.GoToAsync("TreeAuditRuleEdit?" + $"{NavParams.TreeAuditRuleID}={tarID}");
        }

        public Task ShowTreeAuditRules()
        {
            return Shell.GoToAsync("TreeAuditRuleList");
        }

        public Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            var parameters = $"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}";

            return Shell.GoToAsync("TreeCountEdit?" + parameters);
        }

        public Task ShowTreeEdit(string treeID)
        {
            return Shell.GoToAsync("Tree?" + $"{NavParams.TreeID}={treeID}");
        }

        public Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID)
        {
            return Shell.GoToAsync("TreeErrorEdit?" + $"{NavParams.TreeID}={treeID}&{NavParams.TreeAuditRuleID}={treeAuditRuleID}");
        }

        public Task ShowTreeList(string unitCode)
        {
            return Shell.GoToAsync("TreeList?" + $"{NavParams.UNIT}={unitCode}");
        }

        public Task ShowUserAgreement()
        {
            return Shell.GoToAsync("//UserAgreement");
        }

        public Task ShowUtilities()
        {
            return Shell.GoToAsync("//Utilities");
        }
    }
}
