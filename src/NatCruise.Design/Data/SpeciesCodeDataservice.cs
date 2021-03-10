using CruiseDAL;
using NatCruise.Data;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class SpeciesCodeDataservice : CruiseDataserviceBase, ISpeciesCodeDataservice
    {
        public SpeciesCodeDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public SpeciesCodeDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public void AddSpeciesCode(string speciesCode)
        {
            Database.Execute("INSERT INTO SpeciesCode (Species) VALUES (@p1);", speciesCode);
        }

        public void DeleteSpeciesCode(string species)
        {
            Database.Execute("DELETE FROM SpeciesCode WHERE Species = @p1;", species);
        }

        public IEnumerable<string> GetSpeciesCodes()
        {
            return Database.QueryGeneric("SELECT Species FROM SpeciesCode;")
                .Select(x => x["Species"] as string);
        }

        public IEnumerable<string> GetSpeciesCodes(string productCode)
        {
            return Database.QueryGeneric2(
                "SELECT sc.Species FROM SpeciesCode AS sc " +
                "JOIN TreeDefaultValue AS tdv USING (Species) " +
                "WHERE tdv.PrimaryProduct = @productCode;",
                new { productCode })
                .Select(x => x["Species"] as string);
        }
    }
}