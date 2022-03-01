using FluentAssertions;
using NatCruise.Cruise.Data;
using NatCruise.Test;
using System;
using System.Linq;
using Xunit;

namespace NatCruise.Cruise.Test.Data
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

                ds.AddNewPlot(unit);

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = ds.GetPlot_Strata(unit, plot.PlotNumber, insertIfNotExists: false);
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

                var plotID = datastore.AddNewPlot(unitCode);

                validatePlot(datastore, unitCode, plotID, plotNumber);

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

        private static void validatePlot(IPlotDataservice datastore, string unitCode, string plotID, int expectedPlotNumber)
        {
            plotID.Should().NotBeNullOrEmpty();

            var plot = datastore.GetPlot(plotID);
            plot.Should().NotBeNull();
            plot.PlotNumber.Should().Be(expectedPlotNumber);

            var strat_plots = datastore.GetPlot_Strata(unitCode, plot.PlotNumber);
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
        public void GetPlotTreeProxies()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var plotNumber = 1;
            using (var db = init.CreateDatabase())
            {
                var ds = new PlotDataservice(db, init.CruiseID, init.DeviceID);
                var plotTallyds = new PlotTreeDataservice(db, init.CruiseID, init.DeviceID, new SamplerInfoDataservice(db, init.CruiseID, init.DeviceID));
                var plotid = ds.AddNewPlot(unitCode);

                var plot_stratum = ds.GetPlot_Strata(unitCode, plotNumber).First();

                var tp = ds.GetPlotTallyPopulationsByUnitCode(unitCode, plotNumber).First();

                var firstTreeid = plotTallyds.CreatePlotTree(unitCode, plotNumber, tp.StratumCode, tp.SampleGroupCode);
                plotTallyds.CreatePlotTree(unitCode, plotNumber, tp.StratumCode, tp.SampleGroupCode);

                var trees = ds.GetPlotTreeProxies(unitCode, plotNumber).ToArray();

                trees.Should().HaveCount(2);
                trees.Select(x => x.TreeNumber).Should().BeInAscendingOrder();

                db.Execute("UPDATE Tree SET TreeNumber = 3 WHERE TreeNumber = 1;");

                var treesAgain = ds.GetPlotTreeProxies(unitCode, plotNumber).ToArray();
                treesAgain.Select(x => x.TreeNumber).Should().BeInAscendingOrder();
                treesAgain.Should().OnlyContain(x => x.Method != null);
            }
        }

        [Fact]
        public void GetPlot_Strata()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new PlotDataservice(db, init.CruiseID, init.DeviceID);

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
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new PlotDataservice(db, init.CruiseID, init.DeviceID);

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

        [Fact]
        public void GetPlot_Strata_PreInitialized()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var strata = new string[] { "st1", "st2" };
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                datastore.GetPlotsByUnitCode(unitCode).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                foreach (var st in strata)
                {
                    database.Execute($"INSERT INTO Plot_Stratum (CruiseID, CuttingUnitCode, PlotNumber, StratumCode) VALUES " +
                        $"('{cruiseID}', '{unitCode}', {plotNumber}, '{st}');");
                }

                var stPlots = datastore.GetPlot_Strata(unitCode, plotNumber);

                stPlots.Should().HaveCount(strata.Count());

                foreach (var ps in stPlots)
                {
                    ValidatePlot_Stratum(ps, true);
                }
            }
        }

        [Fact]
        public void GetPlot_Stratum()
        {
            var units = new string[] { "u1" };
            var strata = new[]
            {
                new CruiseDAL.V3.Models.Stratum {StratumCode = "st1", Method = "FIX" },
                new CruiseDAL.V3.Models.Stratum {StratumCode = "st2", Method = "3PPNT", KZ3PPNT = 101  },
                new CruiseDAL.V3.Models.Stratum {StratumCode = "st3", Method = "FIX"},
            };
            var unit_strata = new[]
            {
                new CruiseDAL.V3.Models.CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st1" },
                new CruiseDAL.V3.Models.CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st2" },
                new CruiseDAL.V3.Models.CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st3" },
            };

            var init = new DatastoreInitializer()
            {
                Units = units,
                Strata = strata,
                UnitStrata = unit_strata,
                SampleGroups = null,
                Subpops = null,
                Species = null,
            };

            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);

                datastore.GetPlotsByUnitCode(units[0]).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{init.CruiseID}', '{plotID}', '{units[0]}', {plotNumber})");

                foreach (var st in strata)
                {
                    if (st.StratumCode == "st3") { continue; }

                    database.Execute($"INSERT INTO Plot_Stratum (CruiseID, CuttingUnitCode, PlotNumber, StratumCode) VALUES " +
                        $"('{init.CruiseID}', '{units[0]}', {plotNumber}, '{st.StratumCode}');");
                }

                foreach (var st in strata)
                {
                    var plotStratum = datastore.GetPlot_Stratum(units[0], st.StratumCode, plotNumber);
                    ValidatePlot_Stratum(plotStratum, st.StratumCode != "st3");
                }

                var nonExistantPS = datastore.GetPlot_Stratum(units[0], "st3", plotNumber);
                ValidatePlot_Stratum(nonExistantPS, false);

                var tppnt = datastore.GetPlot_Stratum(units[0], "st2", plotNumber);
                tppnt.KZ3PPNT.Should().Be(101);

            }
        }

        private void ValidatePlot_Stratum(NatCruise.Cruise.Models.Plot_Stratum ps, bool inCruise)
        {
            ps.CuttingUnitCode.Should().NotBeNullOrWhiteSpace();
            ps.StratumCode.Should().NotBeNullOrWhiteSpace();
            ps.PlotNumber.Should().BeGreaterThan(0);
            ps.InCruise.Should().Be(inCruise);
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

        [Theory]
        [InlineData("FIX")]
        [InlineData("PCM")]
        [InlineData("FCM")]
        [InlineData("FIXCNT")]
        [InlineData("F3P")]
        [InlineData("P3P")]
        public void GetPlotStrataProxies(string method)
        {
            var init = new DatastoreInitializer()
            {
                Strata = null,
                UnitStrata = null,
                SampleGroups = null,
                Species = null,
                Subpops = null,
            };
            var unit = init.Units.First();
            var newStratumCode = "01";

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);

                datastore.GetPlotStrataProxies(unit).Should().HaveCount(0);

                var stratumID = Guid.NewGuid().ToString();
                database.Execute($"INSERT INTO Stratum (CruiseID, StratumID, StratumCode, Method) VALUES ('{init.CruiseID}', '{stratumID}', '{newStratumCode}', '{method}');");
                database.Execute($"INSERT INTO CuttingUnit_Stratum (CruiseID, CuttingUnitCode, StratumCode) VALUES ('{init.CruiseID}', '{unit}', '{newStratumCode}')");

                var results = datastore.GetPlotStrataProxies(unit).ToArray();

                results.Should().HaveCount(1);
            }
        }

        [Fact]
        public void InsertStratumPlot()
        {
            var init = new DatastoreInitializer();
            var plotNumber = 1;
            var stratumCode = "st1";
            var unitCode = "u1";
            var isEmpty = true;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;
            //var remarks = "something";

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = isEmpty,
                    //Remarks = remarks
                };

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                datastore.InsertPlot_Stratum(stratumPlot);

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just took that plot number");

                var plotStratumAgain = datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                plotStratumAgain.Should().NotBeNull();
                plotStratumAgain.PlotNumber.Should().Be(plotNumber);
                //ourStratumPlot.Remarks.Should().Be(remarks);
                plotStratumAgain.IsEmpty.Should().Be(isEmpty);
                plotStratumAgain.StratumCode.Should().Be(stratumCode);
            }
        }

        [Fact]
        public void Insert3PPNT_Plot_Stratum()
        {
            var init = new DatastoreInitializer();
            var plotNumber = 1;
            var stratumCode = "st1";
            var unitCode = "u1";
            var isEmpty = true;
            var kpi = 101;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;
            //var remarks = "something";

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);

                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = isEmpty,
                    KPI = kpi,

                    //Remarks = remarks
                };

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                datastore.Insert3PPNT_Plot_Stratum(stratumPlot);

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just took that plot number");

                var plotStratumAgain = datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                plotStratumAgain.Should().NotBeNull();
                plotStratumAgain.PlotNumber.Should().Be(plotNumber);
                //ourStratumPlot.Remarks.Should().Be(remarks);
                plotStratumAgain.KPI.Should().Be(kpi);
                plotStratumAgain.IsEmpty.Should().Be(isEmpty);
                plotStratumAgain.StratumCode.Should().Be(stratumCode);
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
            var init = new DatastoreInitializer();
            var unitCode = init.Units.First();
            //var plotNumber = 1;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);
                var plotTallyds = new PlotTreeDataservice(database, init.CruiseID, init.DeviceID, new SamplerInfoDataservice(database, init.CruiseID, init.DeviceID));
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var plotID = datastore.AddNewPlot(unitCode);

                validatePlot(datastore, unitCode, plotID, 1);

                var treeID = plotTallyds.CreatePlotTree(unitCode, 1, "st1", "sg1");

                datastore.UpdatePlotNumber(plotID, 2);

                validatePlot(datastore, unitCode, plotID, 2);

                // verify that plot number updates on tree records
                var treeAfter = treeDS.GetTree(treeID);
                treeAfter.PlotNumber.Should().Be(2);
            }
        }

        [Fact]
        public void UpdatePlot_Stratum()
        {
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

                datastore.InsertPlot_Stratum(stratumPlot);

                stratumPlot.IsEmpty = true;
                datastore.UpdatePlot_Stratum(stratumPlot);

                var ourStratumPlot = datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);

                ourStratumPlot.IsEmpty.Should().Be(true);
            }
        }

        [Fact]
        public void DeletePlot_Stratum()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var stratumCode = "st1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);

                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                };

                database.Execute("INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{init.CruiseID}', '{plotID}', '{unitCode}', {plotNumber});");

                datastore.InsertPlot_Stratum(stratumPlot);

                var echo = datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                echo.Should().NotBeNull("where's my echo");

                datastore.DeletePlot_Stratum(echo.CuttingUnitCode, echo.StratumCode, echo.PlotNumber);
            }
        }

        [Fact]
        public void GetPlotTallyPopulationsByUnitCode()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new PlotDataservice(db, init.CruiseID, init.DeviceID);

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetPlotTallyPopulationsByUnitCode_PNT_FIX_noPlot(bool tallyBySp)
        {
            var init = new DatastoreInitializer();
            var unitCode = "u3";
            var stCode = "st5";
            var sgCode = "sg4";

            var method = CruiseDAL.Schema.CruiseMethods.PNT;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);

                DatastoreInitializer.InitializeDatabase(
                    database,
                    init.DeviceID,
                    init.CruiseID,
                    init.SaleID,
                    new[] { unitCode },
                    new[]
                    {
                        new CruiseDAL.V3.Models.Stratum {StratumCode = stCode, Method = method},
                    },
                    new[]
                    {
                        new CruiseDAL.V3.Models.CuttingUnit_Stratum { CuttingUnitCode = unitCode, StratumCode = stCode},
                    },
                    new[]
                    {
                        new CruiseDAL.V3.Models.SampleGroup {StratumCode = stCode, SampleGroupCode = sgCode, SamplingFrequency = 101, TallyBySubPop = tallyBySp}
                    },
                    // species
                    new[] { "sp4" },
                    new[]
                    {
                        new CruiseDAL.V3.Models.TreeDefaultValue {SpeciesCode ="sp4", PrimaryProduct = "01"},
                    },
                    new[]
                    {
                        new CruiseDAL.V3.Models.SubPopulation {StratumCode = stCode, SampleGroupCode = sgCode, SpeciesCode = "sp4", LiveDead = "L" },
                        new CruiseDAL.V3.Models.SubPopulation {StratumCode = stCode, SampleGroupCode = sgCode, SpeciesCode = "sp4", LiveDead = "D" },
                    }
                );

                {
                    //we are going to check that the tally population returned is vallid for a
                    //tally population with no count tree record associated
                    //it should return one tally pop per sample group in the unit, that is associated with a FIX or PNT stratum
                    var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode(unitCode, 1);

                    if (tallyBySp == false)
                    {
                        unit3tallyPops.Should().HaveCount(1);

                        var tp = unit3tallyPops.Single();
                        tp.SpeciesCode.Should().BeNull("Species");
                        tp.LiveDead.Should().BeNull("liveDead");

                        ValidateTallyPop(tp);
                    }
                    else
                    {
                        unit3tallyPops.Should().HaveCount(2);

                        foreach (var tp in unit3tallyPops)
                        {
                            tp.SpeciesCode.Should().NotBeNullOrWhiteSpace();
                            tp.LiveDead.Should().NotBeNullOrWhiteSpace();
                        }
                    }

                    void ValidateTallyPop(Models.TallyPopulation_Plot tp)
                    {
                        tp.StratumCode.Should().Be(stCode);
                        tp.SampleGroupCode.Should().Be(sgCode);
                        tp.InCruise.Should().BeFalse();
                    }
                }
            }
        }
    }
}