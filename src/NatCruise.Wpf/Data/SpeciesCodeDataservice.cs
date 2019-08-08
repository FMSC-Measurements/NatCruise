using CruiseDAL;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Wpf.Data
{
    public class SpeciesCodeDataservice : ISpeciesCodeDataservice
    {
        public SpeciesCodeDataservice(string path)
        {
            Database = new CruiseDatastore(path);
        }

        private CruiseDatastore Database { get; }

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