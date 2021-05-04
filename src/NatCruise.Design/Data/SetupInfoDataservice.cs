using CruiseDAL;
using CruiseDAL.V3.Models;
using NatCruise.Design.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class SetupInfoDataservice : ISetupInfoDataservice
    {
        CruiseDatastore_V3 Database { get; }

        public SetupInfoDataservice()
        {
            Database = new CruiseDatastore_V3();
        }

        public IEnumerable<Forest> GetForests(string regionCode)
        {
            return Database.From<Forest>().Where("Region = @p1").Query(regionCode).ToArray();
        }

        public IEnumerable<Purpose> GetPurposes()
        {
            return Database.From<Purpose>().Query().ToArray();
        }

        public IEnumerable<Region> GetRegions()
        {
            return Database.From<Region>().Query().ToArray();
        }

        public IEnumerable<UOM> GetUOMCodes()
        {
            return Database.From<UOM>().Query().ToArray();
        }

        public IEnumerable<District> GetDistricts(string region, string forest)
        {
            return Database.From<District>().Where("Region = @p1 AND Forest = @p2").Query(region, forest).ToArray();
        }

        public IEnumerable<FIASpecies> GetFIASpecies()
        {
            return Database.From<FIASpecies>().Query().ToArray();
        }

        public IEnumerable<Product> GetProducts()
        {
            return Database.From<Product>().Query().ToArray();
        }

        public IEnumerable<CruiseMethod> GetCruiseMethods()
        {
            return Database.From<CruiseMethod>().Query().ToArray();
        }
    }
}