using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IPlotDataservice : IDataservice
    {
        #region plot

        string AddNewPlot(string cuttingUnitCode);

        Plot GetPlot(string plotID);

        Plot GetPlot(string cuttingUnitCode, int plotNumber);

        IEnumerable<Plot> GetPlotsByUnitCode(string unitCode);

        void DeletePlot(string unitCode, int plotNumber);

        void UpdatePlot(Plot plot);

        #endregion plot

        //IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID);

        int GetNextPlotNumber(string unitCode);

        bool IsPlotNumberAvalible(string unitCode, int plotNumber);

        void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark);

        int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber);
    }
}