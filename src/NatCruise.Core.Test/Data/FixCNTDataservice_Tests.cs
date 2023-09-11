using FluentAssertions;
using NatCruise.Data;
using NatCruise.Test;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
{
    public class FixCNTDataservice_Tests : TestBase
    {
        public FixCNTDataservice_Tests(ITestOutputHelper output) : base(output)
        {
        }

        //[Fact]
        //public void CreateFixCNTTallyTree()
        //{
        //    var unitCode = "u1";
        //    var plotNumber = 1;
        //    var stratumCode = "st1";
        //    var sgCode = "sg1";
        //    var species = "sp1";
        //    var liveDead = "L";
        //    var countMeasure = "C";
        //    var treeCount = 1;
        //    var fieldName = "DBH";

        //    using (var database = CreateDatabase())
        //    {
        //        var datastore = new CuttingUnitDatastore(database);

        //        database.Execute(
        //            $"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES ({Guid.Empty.ToString()}, '{unitCode}', {plotNumber});" +
        //            $"INSERT INTO Plot_Stratum (CuttingUnitCode, PlotNumber, StratumCode) VALUES ('{unitCode}', {plotNumber}, '{stratumCode}');");

        //        var tree = datastore.CreateFixCNTTallyTree(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, fieldName, 1.0, treeCount);
        //        tree.Should().NotBeNull();
        //    }
        //}

        [Fact]
        public void IncrementFixCNTTreeCount()
        {
            var init = new DatastoreInitializer();

            var unitCode = init.Units.First();
            var plotNumber = 1;
            var fieldName = "DBH";
            var value = 10;

            var subpop = init.Subpops.First();

            var sgCode = subpop.SampleGroupCode;
            var stCode = subpop.StratumCode;
            var sp = subpop.SpeciesCode;
            var ld = subpop.LiveDead;

            using (var database = init.CreateDatabase())
            {
                var plotds = new PlotDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var treeds = new PlotTreeDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));

                var plotID = plotds.AddNewPlot(unitCode);

                var ds = new FixCNTDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(0);

                // after incrementing once, tree count should be 1
                // and there shold only be one tree in the tree table
                ds.IncrementFixCNTTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(1);
                database.ExecuteScalar<int>("SELECT count(*) FROM Tree;").Should().Be(1);

                // after incrementing a second time, tree count should be 2
                // and there shold still only be one tree in the tree table
                ds.IncrementFixCNTTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(2);

                database.ExecuteScalar<int>("SELECT count(*) FROM Tree;").Should().Be(1);

                var plotTrees = treeds.GetPlotTrees(unitCode, plotNumber);
                plotTrees.Should().HaveCount(1);
            }
        }

        [Fact]
        public void DecrementFixCNTTreeCount()
        {
            var init = new DatastoreInitializer();

            var unitCode = init.Units.First();
            var plotNumber = 1;
            var fieldName = "DBH";
            var value = 10;

            var subpop = init.Subpops.First();

            var sgCode = subpop.SampleGroupCode;
            var stCode = subpop.StratumCode;
            var sp = subpop.SpeciesCode;
            var ld = subpop.LiveDead;

            using (var database = init.CreateDatabase())
            {
                var plotds = new PlotDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var plotID = plotds.AddNewPlot(unitCode);

                var ds = new FixCNTDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                // check initial state
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(0);

                // increment once
                ds.IncrementFixCNTTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(1);

                // decrement back to zero
                ds.DecrementFixCNTTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(0);

                // try to decrement past zero should only result in count of zero
                ds.DecrementFixCNTTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(0);
            }
        }

        [Fact]
        public void AddFixCNTTree()
        {
            var init = new DatastoreInitializer();

            var unitCode = init.Units.First();
            var plotNumber = 1;
            var fieldName = "DBH";
            var value = 10;

            var subpop = init.Subpops.First();

            var sgCode = subpop.SampleGroupCode;
            var stCode = subpop.StratumCode;
            var sp = subpop.SpeciesCode;
            var ld = subpop.LiveDead;

            using (var database = init.CreateDatabase())
            {
                var plotds = new PlotDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var treeds = new PlotTreeDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));

                var plotID = plotds.AddNewPlot(unitCode);

                var ds = new FixCNTDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(0);

                ds.AddFixCNTTree(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(1);

                ds.AddFixCNTTree(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(2);

                var plotTrees = treeds.GetPlotTrees(unitCode, plotNumber);
                plotTrees.Should().HaveCount(2);
            }
        }

        [Fact]
        public void RemoveFixCNTTree()
        {
            var init = new DatastoreInitializer();

            var unitCode = init.Units.First();
            var plotNumber = 1;
            var fieldName = "DBH";
            var value = 10;

            var subpop = init.Subpops.First();

            var sgCode = subpop.SampleGroupCode;
            var stCode = subpop.StratumCode;
            var sp = subpop.SpeciesCode;
            var ld = subpop.LiveDead;

            using (var database = init.CreateDatabase())
            {
                var plotds = new PlotDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var treeds = new PlotTreeDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));

                var plotID = plotds.AddNewPlot(unitCode);

                var ds = new FixCNTDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(0);

                ds.AddFixCNTTree(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(1);

                ds.AddFixCNTTree(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(2);

                var plotTrees = treeds.GetPlotTrees(unitCode, plotNumber);
                plotTrees.Should().HaveCount(2);

                ds.RemoveFixCNTTree(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(1);
                treeds.GetPlotTrees(unitCode, plotNumber).Should().HaveCount(1);
            }
        }
    }
}