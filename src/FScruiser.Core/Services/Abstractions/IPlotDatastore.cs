using FScruiser.Models;
using System.Collections.Generic;

namespace FScruiser.Services
{
    public interface IPlotDatastore
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

        void InsertPlot_Stratum(Plot_Stratum stratumPlot);

        IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false);

        Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber);

        void UpdatePlot_Stratum(Plot_Stratum stratumPlot);

        void UpdatePlotNumber(string plotID, int plotNumber);

        void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber);

        #endregion plot stratum

        #region tally populations

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);

        #endregion tally populations

        #region tree

        void InsertTree(TreeStub_Plot tree);

        string CreatePlotTree(string unitCode,
            int plotNumber,
            string stratumCode,
            string sampleGroupCode = null,
            string species = null,
            string liveDead = "L",
            string countMeasure = "M",
            int treeCount = 1,
            int kpi = 0,
            bool stm = false);

        IEnumerable<TreeStub_Plot> GetPlotTreeProxies(string unitCode, int plotNumber);

        int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber, bool isRecon);

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