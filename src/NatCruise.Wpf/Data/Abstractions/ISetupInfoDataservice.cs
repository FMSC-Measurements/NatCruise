using NatCruise.Wpf.Models;
using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface ISetupInfoDataservice
    {
        IEnumerable<Region> GetRegions();

        IEnumerable<Forest> GetForests(string regionCode);

        IEnumerable<Purpose> GetPurposes();

        IEnumerable<UOM> GetUOMCodes();
    }
}