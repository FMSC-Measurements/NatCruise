using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;

namespace NatCruise.Cruise.Data
{
    public interface IPlotTreeDataservice : IDataservice
    {

        IEnumerable<PlotTreeEntry> GetPlotTrees(string unitCode, int plotNumber);

        void InsertTree(PlotTreeEntry tree, SamplerState samplerState);

        string CreatePlotTree(string unitCode,
            int plotNumber,
            string stratumCode,
            string sampleGroupCode,
            string species = null,
            string liveDead = "L",
            string countMeasure = "M",
            int treeCount = 1,
            int kpi = 0,
            bool stm = false);

        int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber);

        void RefreshErrorsAndWarnings(PlotTreeEntry treeEntry);
    }
}