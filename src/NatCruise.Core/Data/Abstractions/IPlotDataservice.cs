using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IPlotDataservice : IDataservice
    {
        #region plot

        string AddNewPlot(string cuttingUnitCode);

        string AddNewPlot(string cuttingUnitCode, int plotNumber);

        Plot GetPlot(string plotID);

        Plot GetPlot(string cuttingUnitCode, int plotNumber);

        IEnumerable<int> GetPlotNumbersOrdered(string cuttingUnitCode, string stratumCode = null);

        IEnumerable<Plot> GetPlotsByUnitCode(string unitCode);

        void DeletePlot(string unitCode, int plotNumber);

        void UpdatePlot(Plot plot);

        void UpdatePlotNumber(string plotID, int plotNumber);

        void UpdatePlotCuttingUnit(string plotID, string cuttingUnitCode);

        #endregion plot

        //IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID);

        int GetNextPlotNumber(string unitCode);

        bool IsPlotNumberAvalible(string unitCode, int plotNumber);

        void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark);

        int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber);
    }
}