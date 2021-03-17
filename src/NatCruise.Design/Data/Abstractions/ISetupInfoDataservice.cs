using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data 
{
    public interface ISetupInfoDataservice 
    {
        IEnumerable<Region> GetRegions();

        IEnumerable<Forest> GetForests(string regionCode);

        IEnumerable<Purpose> GetPurposes();

        IEnumerable<UOM> GetUOMCodes();

        IEnumerable<District> GetDistricts(string region, string forest);
    }
}