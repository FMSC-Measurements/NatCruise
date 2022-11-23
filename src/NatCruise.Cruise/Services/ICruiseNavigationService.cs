using NatCruise.Navigation;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public interface ICruiseNavigationService : INatCruiseNavigationService
    {
        Task ShowBlank();

        Task ShowCuttingUnitInfo(string unitCode);


        //Task ShowCruiseSelect(string saleID);

        Task ShowDatabaseUtilities();

        Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode);

        Task ShowImport();

        Task ShowLogEdit(string logID);

        Task ShowLogsList(string treeID);

        Task ShowManageCruisers();

        Task ShowPlotEdit(string plotID);

        //Task ShowPlotEdit(string unitCode, int plotNumber);

        Task ShowPlotList(string unitCode);

        Task ShowPlotTally(string plotID);

        Task ShowPlotTreeList(string unitCode);

        //Task ShowPlotTally(string unitCode, int plotNumber);

        Task ShowPrivacyPolicy();

        Task ShowSale(string cruiseID);

        Task ShowSaleSelect();

        Task ShowTally(string unitCode);



        Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        Task ShowTreeEdit(string treeID);

        Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID);

        Task ShowTreeList(string unitCode);

        Task ShowUserAgreement();

        Task ShowUtilities();
    }
}