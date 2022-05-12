using FluentAssertions;
using NatCruise.Data;
using NatCruise.Test;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Core.Test.Data
{
    public class SaleDataservice_Test : TestBase
    {
        public SaleDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetCruises_Test()
        {
            var initializer = new DatastoreInitializer();
            using (var db = initializer.CreateDatabase())
            {
                var cruiseID = initializer.CruiseID;
                var ds = new SaleDataservice(db, cruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var cruises = ds.GetCruises();
                cruises.Should().HaveCount(1);

                var cruise = cruises.Single();
                cruise.HasPlotStrata.Should().BeTrue();
            }
        }

        [Fact]
        public void DeleteCruise()
        {
            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new SaleDataservice(db, init.CruiseID, init.DeviceID);


            var plotds = new PlotDataservice(db, init.CruiseID, init.DeviceID);
            var unit = init.Units.First();
            var plotID = plotds.AddNewPlot(unit);

            ds.DeleteCruise(init.CruiseID);
        }
    }
}