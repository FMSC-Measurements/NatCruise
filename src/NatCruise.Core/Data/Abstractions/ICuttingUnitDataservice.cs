using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ICuttingUnitDataservice : IDataservice
    {
        CuttingUnit GetCuttingUnit(string unitCode);

        IEnumerable<CuttingUnit> GetCuttingUnits();

        IEnumerable<string> GetCuttingUnitCodes();

        IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode);

        CuttingUnitStrataSummary GetCuttingUnitStrataSummary(string unitCode);

        void AddCuttingUnit(CuttingUnit unit);

        void UpdateCuttingUnit(CuttingUnit unit);

        void UpdateCuttingUnitCode(CuttingUnit unit);

        void DeleteCuttingUnit(CuttingUnit unit);
    }
}