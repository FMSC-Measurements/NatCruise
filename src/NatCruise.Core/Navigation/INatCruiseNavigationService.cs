using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public interface INatCruiseNavigationService
    {
        Task ShowFieldData(string cuttingUnit = null);

        Task ShowCruiseLandingLayout();

        #region design pages
        Task ShowStrata();

        Task ShowCuttingUnitList();

        Task ShowTreeAuditRules();

        Task ShowTreeAuditRuleEdit(string tarID);

        Task ShowStratumDetail(string stratumCode);

        Task ShowFieldSetup(string stratumCode);

        Task ShowSampleGroups(string stratumCode);

        #endregion

        #region Cruising

        Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber);

        #endregion

        Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber);

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