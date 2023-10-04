using FluentAssertions;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Test;
using System;
using System.Linq;
using Xunit;

namespace NatCruise.Cruise.Test.Data
{
    public class PlotTreeDataservice_Test
    {
        [Fact]
        public void InsertTree()
        {
            var unitCode = "u1";
            var plotNumber = 1;
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var countMeasure = "C";
            var treeCount = 1;
            var plotID = Guid.NewGuid().ToString();

            var init = new DatastoreInitializer();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotTreeDataservice(database, cruiseID, init.DeviceID, new SamplerStateDataservice(database, init.CruiseID, init.DeviceID));
                var treeDS = new TreeDataservice(database, cruiseID, init.DeviceID);

                database.Execute(
                    $"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES ('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber});" +
                    $"INSERT INTO Plot_Stratum (CruiseID, CuttingUnitCode, PlotNumber, StratumCode) VALUES ('{cruiseID}', '{unitCode}', {plotNumber}, '{stratumCode}');");

                var treeID = Guid.NewGuid().ToString();
                var tree = new PlotTreeEntry
                {
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = species,
                    CountOrMeasure = countMeasure,
                    TreeCount = treeCount,
                    PlotNumber = plotNumber,
                    LiveDead = liveDead,
                };

                datastore.InsertTree(tree, (SamplerState)null);

                database.ExecuteScalar<int>("SELECT count(*) from Tree;").Should().Be(1);

                //var mytree = database.QueryGeneric("SELECT * FROM Tree_V3;").ToArray();

                var treeAgain = treeDS.GetTree(treeID);
                treeAgain.Should().NotBeNull();

                treeAgain.TreeID.Should().Be(treeID);
                treeAgain.StratumCode.Should().Be(stratumCode);
                treeAgain.SampleGroupCode.Should().Be(sgCode);
                treeAgain.SpeciesCode.Should().Be(species);
                treeAgain.LiveDead.Should().Be(liveDead);
                treeAgain.CountOrMeasure.Should().Be(countMeasure);
                //tree.TreeCount.Should().Be(treeCount);
            }
        }

        [Fact]
        public void CreatePlotTree()
        {
            var unitCode = "u1";
            var plotNumber = 1;
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var countMeasure = "C";
            var treeCount = 1;
            var plotID = Guid.NewGuid().ToString();

            var init = new DatastoreInitializer();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotTreeDataservice(database, cruiseID, init.DeviceID, new SamplerStateDataservice(database, init.CruiseID, init.DeviceID));
                var treeDS = new TreeDataservice(database, cruiseID, init.DeviceID);

                database.Execute(
                    $"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES ('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber});" +
                    $"INSERT INTO Plot_Stratum (CruiseID, CuttingUnitCode, PlotNumber, StratumCode) VALUES ('{cruiseID}', '{unitCode}', {plotNumber}, '{stratumCode}');");

                var treeID = datastore.CreatePlotTree(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, countMeasure, treeCount);

                database.ExecuteScalar<int>("SELECT count(*) from Tree;").Should().Be(1);

                //var mytree = database.QueryGeneric("SELECT * FROM Tree_V3;").ToArray();

                var tree = treeDS.GetTree(treeID);
                tree.Should().NotBeNull();

                tree.TreeID.Should().Be(treeID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.SpeciesCode.Should().Be(species);
                tree.LiveDead.Should().Be(liveDead);
                tree.CountOrMeasure.Should().Be(countMeasure);
                //tree.TreeCount.Should().Be(treeCount);
            }
        }

        [Fact]
        public void GetPlotTrees()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var plotNumber = 1;
            using (var db = init.CreateDatabase())
            {
                var plotDs = new PlotDataservice(db, init.CruiseID, init.DeviceID);
                var tpDs = new TallyPopulationDataservice(db, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(db, init.CruiseID, init.DeviceID);
                var plotTreeds = new PlotTreeDataservice(db, init.CruiseID, init.DeviceID, new SamplerStateDataservice(db, init.CruiseID, init.DeviceID));
                var plotid = plotDs.AddNewPlot(unitCode);

                var plot_stratum = plotStratumDs.GetPlot_Strata(unitCode, plotNumber).First();

                var tp = tpDs.GetPlotTallyPopulationsByUnitCode(unitCode, plotNumber).First();

                var firstTreeid = plotTreeds.CreatePlotTree(unitCode, plotNumber, tp.StratumCode, tp.SampleGroupCode);
                plotTreeds.CreatePlotTree(unitCode, plotNumber, tp.StratumCode, tp.SampleGroupCode);

                var trees = plotTreeds.GetPlotTrees(unitCode, plotNumber).ToArray();

                trees.Should().HaveCount(2);
                trees.Select(x => x.TreeNumber).Should().BeInAscendingOrder();

                db.Execute("UPDATE Tree SET TreeNumber = 3 WHERE TreeNumber = 1;");

                var treesAgain = plotTreeds.GetPlotTrees(unitCode, plotNumber).ToArray();
                treesAgain.Select(x => x.TreeNumber).Should().BeInAscendingOrder();
                treesAgain.Should().OnlyContain(x => x.Method != null);
            }
        }

        [Fact]
        public void CreatePlotTree_WithNullLiveDead()
        {
            var unitCode = "u1";
            var plotNumber = 1;
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = (string)null;
            var countMeasure = "C";
            var treeCount = 1;
            var plotID = Guid.NewGuid().ToString();

            var init = new DatastoreInitializer();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotTreeDataservice(database, cruiseID, init.DeviceID, new SamplerStateDataservice(database, init.CruiseID, init.DeviceID));
                var treeDS = new TreeDataservice(database, cruiseID, init.DeviceID);

                database.Execute(
                    $"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES ('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber});" +
                    $"INSERT INTO Plot_Stratum (CruiseID, CuttingUnitCode, PlotNumber, StratumCode) VALUES ('{cruiseID}', '{unitCode}', {plotNumber}, '{stratumCode}');");

                var treeID = datastore.CreatePlotTree(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, countMeasure, treeCount);

                database.ExecuteScalar<int>("SELECT count(*) from Tree;").Should().Be(1);

                //var mytree = database.QueryGeneric("SELECT * FROM Tree_V3;").ToArray();

                var defaultLiveDead = database.ExecuteScalar<string>(
                    "SELECT DefaultLiveDead " +
                    "FROM SampleGroup " +
                    "WHERE CruiseID = @p1 " +
                    "AND StratumCode = @p2 " +
                    "AND SampleGroupCode = @p3;", cruiseID, stratumCode, sgCode);

                var tree = treeDS.GetTree(treeID);
                tree.Should().NotBeNull();

                tree.TreeID.Should().Be(treeID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.SpeciesCode.Should().Be(species);
                tree.LiveDead.Should().Be(defaultLiveDead);
                tree.CountOrMeasure.Should().Be(countMeasure);
                //tree.TreeCount.Should().Be(treeCount);
            }
        }
    }
}