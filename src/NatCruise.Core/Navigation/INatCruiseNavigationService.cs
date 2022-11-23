using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public interface INatCruiseNavigationService
    {
        Task ShowFieldData(string cuttingUnit = null);

        Task ShowCruiseLandingLayout();

        #region design pages
        Task ShowCuttingUnitList();

        #endregion

        #region Cruising

        Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber);

        #endregion

        Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber);

        Task ShowFeedback();

        Task ShowSettings();

        Task ShowSampleStateManagment();

        //Task ShowImport();

        Task ShowCruiseSelect(string saleNumber);

        Task GoBackAsync();
    }
}