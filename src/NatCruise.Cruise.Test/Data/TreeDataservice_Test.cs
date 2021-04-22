using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Services;
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
    public class TreeDataservice_Test : TestBase
    {
        public TreeDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetTreeFieldValues_nonExistantTree()
        {
            var initializer = new DatastoreInitializer();

            using var db = initializer.CreateDatabase();

            var dataservice = new CuttingUnitDatastore(db, initializer.CruiseID, initializer.DeviceID, new SamplerInfoDataservice(db, initializer.CruiseID, initializer.DeviceID));

            var treeID = Guid.NewGuid().ToString();
            var tfvs = dataservice.GetTreeFieldValues(treeID);
            tfvs.Should().BeEmpty();
        }

        [Fact]
        public void IsTreeNumberAvalible_noPlot()
        {

            var init = new DatastoreInitializer();
            var unit = init.Units[0];

            using var db = init.CreateDatabase();

            var ds = new CuttingUnitDatastore(db, init.CruiseID, init.DeviceID, new SamplerInfoDataservice(db, init.CruiseID, init.DeviceID));

            ds.IsTreeNumberAvalible(unit, 1).Should().BeTrue();
        }

        [Fact]
        public void IsTreeNumberAvalible_Plot_nummberAcrossStrata()
        {

            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            var stratum = init.PlotStrata[0].StratumCode;
            var altStratum = init.PlotStrata[1].StratumCode;
            var plotNumber = 1;

            using var db = init.CreateDatabase();

            db.Execute("UPDATE Cruise SET UseCrossStrataPlotTreeNumbering = 1 WHERE CruiseID = @p1;", init.CruiseID);

            var ds = new CuttingUnitDatastore(db, init.CruiseID, init.DeviceID, new SamplerInfoDataservice(db, init.CruiseID, init.DeviceID));

            var newPlot = new Plot()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = plotNumber,
                PlotID = Guid.NewGuid().ToString(),
            };
            db.Insert(newPlot);

            var newPlotStratum = new Plot_Stratum()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = newPlot.PlotNumber,
                StratumCode = stratum,
            };
            db.Insert(newPlotStratum);

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeTrue();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeTrue();

            db.Insert(new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = init.SampleGroups[0].SampleGroupCode,
                PlotNumber = plotNumber,
            });

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeFalse();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeFalse();
        }

        [Fact]
        public void IsTreeNumberAvalible_Plot_dontNummberAcrossStrata()
        {

            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            var stratum = init.PlotStrata[0].StratumCode;
            var altStratum = init.PlotStrata[1].StratumCode;
            var plotNumber = 1;

            using var db = init.CreateDatabase();

            db.Execute("UPDATE Cruise SET UseCrossStrataPlotTreeNumbering = 0 WHERE CruiseID = @p1;", init.CruiseID);

            var ds = new CuttingUnitDatastore(db, init.CruiseID, init.DeviceID, new SamplerInfoDataservice(db, init.CruiseID, init.DeviceID));

            var newPlot = new Plot()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = plotNumber,
                PlotID = Guid.NewGuid().ToString(),
            };
            db.Insert(newPlot);

            var newPlotStratum = new Plot_Stratum()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = newPlot.PlotNumber,
                StratumCode = stratum,
            };
            db.Insert(newPlotStratum);

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeTrue();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeTrue();

            db.Insert(new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = init.SampleGroups[0].SampleGroupCode,
                PlotNumber = plotNumber,
            });

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeFalse();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeTrue();
        }
    }
}
