using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IStratumDataservice : IDataservice
    {
        IEnumerable<Stratum> GetStrata(string cuttingUnitCode = null);
        void AddStratum(Stratum stratum);

        void AddStratumToCuttingUnit(string cuttingUnitCode, string stratumCode);

        void DeleteStratum(Stratum stratum);

        void DeleteStratum(string stratumCode);

        string GetCruiseMethod(string stratumCode);

        IEnumerable<string> GetStratumCodes(string cuttingUnitCode = null);

        IEnumerable<string> GetStratumCodesByUnit(string unitCode);

        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<Stratum> GetPlotStrata(string unitCode);

        void RemoveStratumFromCuttingUnit(string cuttingUnitCode, string stratumCode);

        void UpdateStratum(Stratum stratum);

        void UpdateStratumCode(Stratum stratum);

        bool HasTreeCounts(string unitCode, string stratum);
    }
}