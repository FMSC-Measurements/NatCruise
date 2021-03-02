using CruiseDAL;
using NatCruise.Design.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class CruiseDataservice : ICruiseDataservice
    {
        public CruiseDataservice(string path)
        {
            Database = new CruiseDatastore(path);
        }

        private CruiseDatastore Database { get; }

        public Sale GetSale()
        {
            return Database.From<Sale>().Query().FirstOrDefault();
        }

        public void UpdateSale(Sale sale)
        {
            Database.Update(sale);

        }
    }
}