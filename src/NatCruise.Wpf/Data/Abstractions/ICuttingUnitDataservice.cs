using NatCruise.Wpf.Models;
using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface ICuttingUnitDataservice
    {
        IEnumerable<CuttingUnit> GetCuttingUnits();

        IEnumerable<string> GetCuttingUnitCodes();

        void AddCuttingUnit(CuttingUnit unit);

        void UpdateCuttingUnit(CuttingUnit unit);

        void DeleteCuttingUnit(CuttingUnit unit);

        IEnumerable<LoggingMethod> GetLoggingMethods();
    }
}