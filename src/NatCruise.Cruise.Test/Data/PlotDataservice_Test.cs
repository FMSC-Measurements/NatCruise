using FluentAssertions;
using NatCruise.Cruise.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NatCruise.Cruise.Test.Data
{
    public class PlotDataservice_Test
    {
        [Fact]
        public void AddNewPlot()
        {
            var init = new DatabaseInitializer();
            var unit = init.Units[0];
            using(var db = init.CreateDatabase())
            {
                var ds = new CuttingUnitDatastore(db, init.CruiseID);

                ds.AddNewPlot(unit);

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = ds.GetPlot_Strata(unit, plot.PlotNumber, insertIfNotExists: false);
                plotStrata.Should().HaveCount(2);
            }
        }

        [Fact]
        public void GetPlotTallyPopulationsByUnitCode()
        {
            var init = new DatabaseInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new CuttingUnitDatastore(db, init.CruiseID);

                ds.AddNewPlot(unit);

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = ds.GetPlot_Strata(unit, plot.PlotNumber, insertIfNotExists: false);
                plotStrata.Should().HaveCount(2);

                var tallyPops = ds.GetPlotTallyPopulationsByUnitCode(unit, plot.PlotNumber);
                tallyPops.Should().NotBeEmpty();
            }
        }

        [Fact]
        public void GetPlot_Strata()
        {
            var init = new DatabaseInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new CuttingUnitDatastore(db, init.CruiseID);

                ds.AddNewPlot(unit);

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = ds.GetPlot_Strata(unit, plot.PlotNumber, insertIfNotExists: false);
                plotStrata.Should().HaveCount(2);
            }
        }

        [Fact]
        public void GetPlot_Strata_insertIfNotExists()
        {
            var init = new DatabaseInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new CuttingUnitDatastore(db, init.CruiseID);

                ds.AddNewPlot(unit);
                

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                db.Execute("DELETE FROM Plot_Stratum;");
                db.GetRowCount("Plot_Stratum", "").Should().Be(0);

                var plotStrata = ds.GetPlot_Strata(unit, plot.PlotNumber, insertIfNotExists: false);
                plotStrata.Should().HaveCount(2);
            }
        }
    }
}
