using NatCruise.Wpf.Models;
using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface ICuttingUnitDataservice
    {
        IEnumerable<CuttingUnit> GetCuttingUnits();

        IEnumerable<string> GetCuttingUnitCodes();

        IEnumerable<CuttingUnit> GetCuttingUnitsByStratum(string stratumCode);

        IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode);

        void AddCuttingUnit(CuttingUnit unit);

        void UpdateCuttingUnit(CuttingUnit unit);

        void DeleteCuttingUnit(CuttingUnit unit);

        IEnumerable<LoggingMethod> GetLoggingMethods();
    }
}