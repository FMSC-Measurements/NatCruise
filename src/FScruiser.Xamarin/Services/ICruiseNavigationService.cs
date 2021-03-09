using NatCruise.Services;
using Prism.Navigation;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public interface ICruiseNavigationService : ICoreNavigationService
    {
        Task ShowImport();

        Task ShowManageCruisers();

        Task ShowCruiseLandingLayout();

        Task ShowCruiseSelect(string saleID);

        Task ShowCuttingUnitList();

        Task ShowCuttingUnitInfo(string unitCode);

        Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode);

        Task<Prism.Navigation.INavigationResult> ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber);

        Task ShowLogEdit(string logID);

        Task<INavigationResult> ShowLogsList(string treeID);

        Task ShowPlotEdit(string plotID);

        Task ShowPlotEdit(string unitCode, int plotNumber);

        Task ShowPlotList(string unitCode);

        Task ShowPlotTally(string plotID);

        Task ShowPlotTally(string unitCode, int plotNumber);

        Task ShowSaleSelect();

        Task ShowSale(string cruiseID);

        Task ShowTally(string unitCode);

        Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber);

        Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        Task<INavigationResult> ShowTreeEdit(string treeID);

        Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID);

        Task ShowTreeList(string unitCode);

        Task GoBackAsync();
    }
}