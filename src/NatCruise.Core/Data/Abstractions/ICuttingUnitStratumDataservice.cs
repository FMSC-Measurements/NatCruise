using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ICuttingUnitStratumDataservice
    {
        IReadOnlyCollection<CuttingUnitStratum> GetCuttingUnitStrata();
    }
}