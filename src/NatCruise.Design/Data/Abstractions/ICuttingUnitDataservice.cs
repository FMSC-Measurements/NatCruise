using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface ICuttingUnitDataservice : IDataservice
    {
        IEnumerable<CuttingUnit> GetCuttingUnits();

        IEnumerable<string> GetCuttingUnitCodes();

        void AddCuttingUnit(CuttingUnit unit);

        void UpdateCuttingUnit(CuttingUnit unit);

        void DeleteCuttingUnit(CuttingUnit unit);

        IEnumerable<LoggingMethod> GetLoggingMethods();
    }
}