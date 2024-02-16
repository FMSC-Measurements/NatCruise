using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public interface INatCruiseNavigationService
    {
        Task ShowAbout();

        Task ShowPrivacyPolicy();

        Task ShowUserAgreement();

        Task ShowFieldData(string cuttingUnit = null);

        Task ShowCruiseLandingLayout();

        #region design pages
        Task ShowStrata();

        Task ShowCuttingUnitList();

        Task ShowTreeAuditRules();

        Task ShowTreeAuditRuleEdit(string tarID);

        Task ShowStratumInfo(string stratumCode);

        Task ShowFieldSetup(string stratumCode);

        Task ShowSampleGroups(string stratumCode);

        Task ShowSubpopulations(string stratumCode, string sampleGroupCode);

        #endregion


        #region tally

        Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead);

        Task ShowTallyPopulationInfo(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        #endregion

        #region Cruising

        Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber);

        #endregion

        Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber);

        Task ShowLimitingDistance();

        Task ShowLogsList(string treeID);

        Task ShowFeedback();

        Task ShowSettings();

        Task ShowSampleStateManagment();

        //Task ShowImport();

        Task ShowCruiseSelect(string saleNumber);

        Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID);

        Task GoBackAsync();
        
    }
}