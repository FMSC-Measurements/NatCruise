using CruiseDAL.V3.Models;
using NatCruise.Data;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
{
    public class SaleDataservice_Test : TestBase
    {
        public SaleDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void DeleteCruise()
        {
            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new SaleDataservice(db, init.CruiseID, init.DeviceID);


            var plotds = new Cruise.Data.PlotDataservice(db, init.CruiseID, init.DeviceID);
            var unit = init.Units.First();
            var plotID = plotds.AddNewPlot(unit);

            ds.DeleteCruise(init.CruiseID);
        }
    }
}
