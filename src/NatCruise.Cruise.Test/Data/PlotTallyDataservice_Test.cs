using FluentAssertions;
using NatCruise.Cruise.Data;
using NatCruise.Test;
using System;
using Xunit;

namespace NatCruise.Cruise.Test.Data
{
    public class PlotTallyDataservice_Test
    {
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
                var datastore = new PlotTallyDataservice(database, cruiseID, init.DeviceID, new SamplerInfoDataservice(database, init.CruiseID, init.DeviceID));
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
    }
}