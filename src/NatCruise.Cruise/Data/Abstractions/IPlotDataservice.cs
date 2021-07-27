using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;

namespace NatCruise.Cruise.Data
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

        #region plot stratum

        IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode);

        void InsertPlot_Stratum(Plot_Stratum stratumPlot);

        IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false);

        Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber);

        void UpdatePlot_Stratum(Plot_Stratum stratumPlot);

        void UpdatePlotNumber(string plotID, int plotNumber);

        void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber);

        #endregion plot stratum


        #region tree

        IEnumerable<PlotTreeEntry> GetPlotTreeProxies(string unitCode, int plotNumber);

        #endregion tree

        IEnumerable<PlotError> GetPlotErrors(string plotID);

        IEnumerable<PlotError> GetPlotErrors(string unit, int plotNumber);

        IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID);

        int GetNextPlotNumber(string unitCode);

        bool IsPlotNumberAvalible(string unitCode, int plotNumber);

        void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark);

        int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber);
    }
}