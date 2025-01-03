using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class CuttingUnitStratumDataservice : TestBase
    {
        public CuttingUnitStratumDataservice(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public  void GetCuttingUnitStrata()
        {
            var init = new DatastoreInitializer();
            using var database = init.CreateDatabase();

            var ds = new NatCruise.Data.CuttingUnitStratumDataservice(database, init.CruiseID, init.DeviceID);

            var unitStrata = ds.GetCuttingUnitStrata();

            Assert.NotNull(unitStrata);
            Assert.NotEmpty(unitStrata);
            unitStrata.Should().HaveSameCount(init.UnitStrata);
        }
    }
}
