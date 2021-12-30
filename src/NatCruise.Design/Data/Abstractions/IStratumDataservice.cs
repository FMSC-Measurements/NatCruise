using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface IStratumDataservice : IDataservice
    {
        IEnumerable<Stratum> GetStrata();

        void AddStratum(Stratum stratum);

        void UpdateStratum(Stratum stratum);

        void UpdateStratumCode(Stratum stratum);

        void DeleteStratum(Stratum stratum);

        void DeleteStratum(string stratumCode);

        void AddStratumToCuttingUnit(string cuttingUnitCode, string stratumCode);

        void RemoveStratumFromCuttingUnit(string cuttingUnitCode, string stratumCode);

        IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode);

        bool HasTreeCounts(string unitCode, string stratum);
    }
}