using NatCruise.Navigation;

namespace FScruiser.Maui.Services;

public interface ICruiseNavigationService : INatCruiseNavigationService
{
    INavigation Navigation { get; }

    Task ShowBlank();

    Task ShowCuttingUnitInfo(string unitCode);

    //Task ShowCruiseSelect(string saleID);

    Task ShowDatabaseUtilities();

    Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode);

    Task ShowImport();

    Task ShowLogEdit(string logID);

    Task ShowManageCruisers();

    Task ShowPlotEdit(string plotID);

    //Task ShowPlotEdit(string unitCode, int plotNumber);

    Task ShowPlotList(string unitCode);

    Task ShowPlotTally(string? plotID);

    Task ShowPlotTreeList(string unitCode);

    //Task ShowPlotTally(string unitCode, int plotNumber);

    Task ShowSale(string cruiseID);

    Task ShowSaleSelect();

    Task ShowTally(string unitCode);

    Task ShowTreeEdit(string treeID);

    Task ShowTreeList(string unitCode);

    Task ShowUtilities();
}