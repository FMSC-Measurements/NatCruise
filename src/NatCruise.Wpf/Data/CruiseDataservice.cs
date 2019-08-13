using CruiseDAL;
using NatCruise.Wpf.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Wpf.Data
{
    public class CruiseDataservice : ICruiseDataservice
    {
        public CruiseDataservice(string path)
        {
            Database = new CruiseDatastore(path);
        }

        private CruiseDatastore Database { get; }

        public IEnumerable<Forest> GetForests(string regionCode)
        {
            return new Forest[0];
        }

        public IEnumerable<Purpose> GetPurposes()
        {
            return new Purpose[0];
        }

        public IEnumerable<Region> GetRegions()
        {
            return new Region[0];
        }

        public Sale GetSale()
        {
            return Database.From<Sale>().Query().FirstOrDefault();
        }

        public IEnumerable<string> GetUOMCodes()
        {
            return new string[0];
        }

        public void UpdateSale(Sale sale)
        {
        }
    }
}