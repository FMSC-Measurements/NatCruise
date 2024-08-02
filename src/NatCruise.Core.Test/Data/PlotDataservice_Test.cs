using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Data;
using System;
using System.Linq;
using Xunit;

namespace NatCruise.Test.Data
{
    public class PlotDataservice_Test
    {
        [Fact]
        public void AddNewPlot()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new PlotDataservice(db, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(db, init.CruiseID, init.DeviceID);

                ds.AddNewPlot(unit);

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = plotStratumDs.GetPlot_Strata(unit, plot.PlotNumber);
                plotStrata.Should().HaveCount(2);
            }
        }

        [Fact]
        public void AddPlotRemarks()
        {
            var init = new DatastoreInitializer();
            var remarks = "something";
            var unitCode = init.Units.First();
            var plotNumber = 1;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);

                var plotID = datastore.AddNewPlot(unitCode);

                validatePlot(datastore, plotStratumDs, unitCode, plotID, plotNumber);

                var plot = datastore.GetPlot(plotID);

                datastore.AddPlotRemark(plot.CuttingUnitCode, plot.PlotNumber, remarks);

                var stuff = database.QueryGeneric("select * from plot").ToArray();

                var plotAgain = datastore.GetPlot(plotID);
                plotAgain.Remarks.Should().Be(remarks);

                datastore.AddPlotRemark(plot.CuttingUnitCode, plot.PlotNumber, remarks);

                plotAgain = datastore.GetPlot(plotID);
                plotAgain.Remarks.Should().Be(remarks + ", " + remarks);
            }
        }

        private static void validatePlot(IPlotDataservice plotDs, IPlotStratumDataservice plotStratumDs, string unitCode, string plotID, int expectedPlotNumber)
        {
            plotID.Should().NotBeNullOrEmpty();

            var plot = plotDs.GetPlot(plotID);
            plot.Should().NotBeNull();
            plot.PlotNumber.Should().Be(expectedPlotNumber);

            var strat_plots = plotStratumDs.GetPlot_Strata(unitCode, plot.PlotNumber);
            strat_plots.Should().NotBeEmpty();

            strat_plots.All(x => x.PlotNumber == expectedPlotNumber).Should().BeTrue();
        }

        [Fact]
        public void GetNextPlotNumber()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                datastore.GetNextPlotNumber(unitCode).Should().Be(1, "unit with no plots, should return 1 for first plot number");

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES ('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber});");

                datastore.GetNextPlotNumber(unitCode).Should().Be(plotNumber + 1, "unit with a plot, should return max plot number + 1");
            }
        }

        [Fact]
        public void IsPlotNumberAvalible()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeTrue("no plots in unit yet");

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES ('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber});");

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just inserted a plot");
            }
        }

        [Fact]
        public void GetPlotsByUnitCode()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                datastore.GetPlotsByUnitCode(unitCode).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES ('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber});");

                datastore.GetPlotsByUnitCode(unitCode).Should().ContainSingle();
            }
        }



        [Fact]
        public void GetPlot_ByPlotID()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                var plot = datastore.GetPlot(plotID);

                plot.Should().NotBeNull();
            }
        }

        [Fact]
        public void GetPlot_ByUnitPlotNumber()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                var plot = datastore.GetPlot(unitCode, plotNumber);

                plot.Should().NotBeNull();
            }
        }

        [Fact]
        public void UpdatePlot()
        {
            var random = new Bogus.Randomizer();
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var stratumCode = "st1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = false,
                };

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                var plot = datastore.GetPlot(unitCode, plotNumber);

                var slope = random.Double();
                plot.Slope = slope;

                var aspect = random.Double();
                plot.Aspect = aspect;

                var remarks = random.String2(24);
                plot.Remarks = remarks;

                datastore.UpdatePlot(plot);

                var plotAgain = datastore.GetPlot(unitCode, plotNumber);

                plotAgain.Slope.Should().Be(slope);
                plotAgain.Aspect.Should().Be(aspect);
                plotAgain.Remarks.Should().Be(remarks);
            }
        }

        [Fact]
        public void UpdatePlotNumber()
        {
            var strata = new[]
                    {
                        new Stratum{ StratumCode = "st1", Method = "PNT" },
                        new Stratum{ StratumCode = "st2", Method = "PCM" },
                    };
            var sampleGroups = new[]
            {
                new SampleGroup {SampleGroupCode = "sg1", StratumCode = strata[0].StratumCode, SamplingFrequency = 101, TallyBySubPop = true},
                new SampleGroup {SampleGroupCode = "sg2", StratumCode = strata[1].StratumCode, SamplingFrequency = 102, TallyBySubPop = false},
            };

            var subpops  = new[]
            {
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = "sp1",
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = "sp1",
                    LiveDead = "D",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = "sp2",
                    LiveDead = "L",
                },

                new SubPopulation {
                    StratumCode = sampleGroups[1].StratumCode,
                    SampleGroupCode = sampleGroups[1].SampleGroupCode,
                    SpeciesCode = "sp1",
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[1].StratumCode,
                    SampleGroupCode = sampleGroups[1].SampleGroupCode,
                    SpeciesCode = "sp1",
                    LiveDead = "D",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[1].StratumCode,
                    SampleGroupCode = sampleGroups[1].SampleGroupCode,
                    SpeciesCode = "sp2",
                    LiveDead = "L",
                },
            };

            var init = new DatastoreInitializer()
            {
                Units = new[] { "u1", "u2" },
                Strata = strata,
                UnitStrata = new[]
                    {
                        new CuttingUnit_Stratum{ CuttingUnitCode = "u1", StratumCode = "st1" },
                        new CuttingUnit_Stratum{ CuttingUnitCode = "u2", StratumCode = "st1" },
                        new CuttingUnit_Stratum{ CuttingUnitCode = "u2", StratumCode = "st2" },
                    },
                SampleGroups = sampleGroups,
                Subpops = subpops,

            };
            var unitCode = "u1";
            //var plotNumber = 1;

            using (var database = init.CreateDatabase())
            {
                var plotDataservice = new PlotDataservice(database, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);
                var plotTallyds = new PlotTreeDataservice(database, init.CruiseID, init.DeviceID, new SamplerStateDataservice(database, init.CruiseID, init.DeviceID));
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var plotID = plotDataservice.AddNewPlot(unitCode);
                var plot = plotDataservice.GetPlot(plotID);
                plot.TreeCount.Should().Be(0);

                

                var treeID = plotTallyds.CreatePlotTree(unitCode, plot.PlotNumber, "st1", "sg1");

                var tallyLedgers = database.From<TallyLedger>().Where("PlotNumber = @p1").Query(plot.PlotNumber).ToArray();
                tallyLedgers.Should().HaveCount(1);

                plot = plotDataservice.GetPlot(plotID);
                plot.TreeCount.Should().Be(1);

                plotDataservice.UpdatePlotCuttingUnit(plotID, "u2");

                // verify plot record
                var plotAgain = plotDataservice.GetPlot(plotID);
                plotAgain.TreeCount.Should().Be(1);

                // verify tally ledger records
                var tallyLedgersAgain = database.From<TallyLedger>().Where("PlotNumber = @p1").Query(plot.PlotNumber).ToArray();
                tallyLedgersAgain.Should().HaveCount(1);
                tallyLedgersAgain.Should().OnlyContain(x => x.CuttingUnitCode == "u2");

                // verify tree records
                var treesAgain = treeDS.GetTrees("u2", plotNumber: plot.PlotNumber);
                treesAgain.Should().HaveCount(1);

            }
        }

        [Fact]
        public void UpdatePlotCuttingUnit()
        {
            var init = new DatastoreInitializer();
            var unitCode = init.Units.First();
            //var plotNumber = 1;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);
                var plotTallyds = new PlotTreeDataservice(database, init.CruiseID, init.DeviceID, new SamplerStateDataservice(database, init.CruiseID, init.DeviceID));
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var plotID = datastore.AddNewPlot(unitCode);

                validatePlot(datastore, plotStratumDs, unitCode, plotID, 1);

                var treeID = plotTallyds.CreatePlotTree(unitCode, 1, "st1", "sg1");

                datastore.UpdatePlotNumber(plotID, 2);

                validatePlot(datastore, plotStratumDs, unitCode, plotID, 2);

                // verify that plot number updates on tree records
                var treeAfter = treeDS.GetTree(treeID);
                treeAfter.PlotNumber.Should().Be(2);
            }
        }
    }
}