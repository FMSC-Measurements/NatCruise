using NatCruise.Wpf.Models;
using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface IStratumDataservice
    {
        IEnumerable<Stratum> GetStrata();

        void AddStratum(Stratum stratum);

        void UpdateStratum(Stratum stratum);

        void DeleteStratum(Stratum stratum);

        void DeleteStratum(string stratumCode);

        void AddStratumToCuttingUnit(string cuttingUnitCode, string stratumCode);

        void RemoveStratumFromCuttingUnit(string cuttingUnitCode, string stratumCode);

        IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode);

        IEnumerable<Method> GetMethods();
    }
}