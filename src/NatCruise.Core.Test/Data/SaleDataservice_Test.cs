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
    }
}