using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data 
{
    public interface ISetupInfoDataservice : IDataservice
    {
        IEnumerable<Region> GetRegions();

        IEnumerable<Forest> GetForests(string regionCode);

        IEnumerable<Purpose> GetPurposes();

        IEnumerable<UOM> GetUOMCodes();
    }
}