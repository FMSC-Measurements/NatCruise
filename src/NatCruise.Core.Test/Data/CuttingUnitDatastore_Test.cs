using FluentAssertions;
using NatCruise.Data;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class CuttingUnitDatastore_Test : TestBase
    {
        public CuttingUnitDatastore_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetCuttingUnit()
        {
            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new CuttingUnitDataservice(db, init.CruiseID, init.DeviceID);
            var cu = ds.GetCuttingUnit("u1");
            cu.Should().NotBeNull();
        }

        [Fact]
        public void GetCuttingUnits()
        {
            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new CuttingUnitDataservice(db, init.CruiseID, init.DeviceID);
            var units = ds.GetCuttingUnits();
            units.Should().NotBeNull();
            units.Should().HaveCount(init.Units.Length);
        }

        [Fact]
        public void GetCuttingUnitsByStratum()
        {
            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new CuttingUnitDataservice(db, init.CruiseID, init.DeviceID);

            foreach (var st in init.Strata)
            {
                var units = ds.GetCuttingUnitsByStratum(st.StratumCode);
                units.Should().NotBeNull();
                units.Should().HaveCountGreaterThan(0);
            }
        }
    }
}