using FluentAssertions;
using FScruiser.Core.Test.Services;
using FScruiser.Data;
using FScruiser.Services;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Data
{
    public class FixCNTDataservice_Tests : Datastore_TestBase
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
            var unitCode = Units.First();
            var plotNumber = 1;
            var fieldName = "DBH";
            var value = 10;

            var subpop = Subpops.First();

            var sgCode = subpop.SampleGroupCode;
            var stCode = subpop.StratumCode;
            var sp = subpop.Species;
            var ld = subpop.LiveDead;

            using (var database = CreateDatabase())
            {
                var plotds = new CuttingUnitDatastore(database);

                var plotID = plotds.AddNewPlot(unitCode);

                var ds = new FixCNTDataservice(database);

                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(0);

                // after incrementing once, tree count should be 1
                // and there shold only be one tree in the tree table
                ds.IncrementFixCNTTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(1);
                database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3;").Should().Be(1);

                // after incrementing a second time, tree count should be 2
                // and there shold still only be one tree in the tree table
                ds.IncrementFixCNTTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value);
                ds.GetTreeCount(unitCode, plotNumber, stCode, sgCode, sp, ld, fieldName, value)
                    .Should().Be(2);

                database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3;").Should().Be(1);

                var plotTrees = plotds.GetPlotTreeProxies(unitCode, plotNumber);
                plotTrees.Should().HaveCount(1);
            }
        }

        [Fact]
        public void DecrementFixCNTTreeCount()
        {
            var unitCode = Units.First();
            var plotNumber = 1;
            var fieldName = "DBH";
            var value = 10;

            var subpop = Subpops.First();

            var sgCode = subpop.SampleGroupCode;
            var stCode = subpop.StratumCode;
            var sp = subpop.Species;
            var ld = subpop.LiveDead;

            using (var database = CreateDatabase())
            {
                var plotds = new CuttingUnitDatastore(database);

                var plotID = plotds.AddNewPlot(unitCode);

                var ds = new FixCNTDataservice(database);

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
    }
}