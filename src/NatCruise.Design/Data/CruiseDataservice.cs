using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class CruiseDataservice : CruiseDataserviceBase, ICruiseDataservice
    {
        public CruiseDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public CruiseDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

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