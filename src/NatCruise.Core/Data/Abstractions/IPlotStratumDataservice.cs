using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IPlotStratumDataservice : IDataservice
    {
        void InsertPlot_Stratum(Plot_Stratum plotStratum);

        void Insert3PPNT_Plot_Stratum(Plot_Stratum plotStratum);

        IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false);

        Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber);

        void UpdatePlot_Stratum(Plot_Stratum stratumPlot);

        void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber);
    }
}