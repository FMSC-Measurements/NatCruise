using NatCruise.Cruise.Models;
using NatCruise.Data;

namespace NatCruise.Cruise.Data
{
    public interface IPlotTallyDataservice : IDataservice
    {
        void InsertTree(PlotTreeEntry tree, SamplerState samplerState);

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

        int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber);
    }
}