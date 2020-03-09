using CruiseDAL;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Linq;

namespace NatCruise.Cruise.Data
{
    public class SaleDataservice : DataserviceBase, ISaleDataservice
    {
        public SaleDataservice(string path) : base(path)
        {
        }

        public SaleDataservice(CruiseDatastore_V3 database) : base(database)
        {
        }

        public Sale GetSale()
        {
            return Database.Query<Sale>("SELECT * FROM Sale;").FirstOrDefault();
        }

        public void UpdateSale(Sale sale)
        {
            Database.Execute2("UPDATE Sale SET Remarks = @Remarks WHERE SaleNumber = @SaleNumber", sale);
        }
    }
}