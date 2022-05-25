using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data 
{
    public interface ISetupInfoDataservice
    {
        IEnumerable<Region> GetRegions();

        IEnumerable<Forest> GetForests(string regionCode);

        IEnumerable<Purpose> GetPurposes();

        IEnumerable<UOM> GetUOMCodes();

        IEnumerable<District> GetDistricts(string region, string forest);

        IEnumerable<FIASpecies> GetFIASpecies();

        IEnumerable<Product> GetProducts();

        IEnumerable<CruiseMethod> GetCruiseMethods();

        IEnumerable<LoggingMethod> GetLoggingMethods();
    }
}