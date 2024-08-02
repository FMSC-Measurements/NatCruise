using CruiseDAL;
using CruiseDAL.Schema;
using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Data;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class TallyPopulationDataservice_Tests : TestBase
    {
        public TallyPopulationDataservice_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetPlotTallyPopulationsByUnitCode()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var plotDs = new PlotDataservice(db, init.CruiseID, init.DeviceID);
                var plotStDs = new PlotStratumDataservice(db, init.CruiseID, init.DeviceID);
                var tallyPopDs = new TallyPopulationDataservice(db, init.CruiseID, init.DeviceID);

                plotDs.AddNewPlot(unit);

                var plots = plotDs.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = plotStDs.GetPlot_Strata(unit, plot.PlotNumber);
                plotStrata.Should().HaveCount(2);

                var tallyPops = tallyPopDs.GetPlotTallyPopulationsByUnitCode(unit, plot.PlotNumber);
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
                var datastore = new TallyPopulationDataservice(database, init.CruiseID, init.DeviceID);

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

        [Theory]
        [InlineData("u1", "st1", "sg1", "sp1", "L", true)]
        [InlineData("u1", "st1", "sg1", null, null, false)]
        public void GetPlotTallyPopulations_WithTreeCounts(string unitCode, string stratum, string sampleGroup, string species, string liveDead, bool tallyBySubpop)
        {
            var init = new DatastoreInitializer();
            //var tallyDescription = $"{stratum} {sampleGroup} {species} {liveDead}";
            //var hotKey = "A";
            var method = CruiseDAL.Schema.CruiseMethods.PNT;
            var cruiseID = init.CruiseID;

            var units = new[] { unitCode };
            var strata = new[]
            {
                new Stratum()
                {
                    StratumCode = stratum,
                    Method = method,
                }
            };
            var unit_strata = new[]
            {
                new CuttingUnit_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                }
            };

            var sampleGroups = new[]
            {
                new SampleGroup()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SamplingFrequency = 101,
                    TallyBySubPop = tallyBySubpop,
                }
            };

            var subPop = new[]
            {
                new SubPopulation()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SpeciesCode = species ?? "dummy",
                    LiveDead = liveDead ?? "L",
                }
            };

            

            using (var db = new CruiseDatastore_V3())
            {
                DatastoreInitializer.InitializeDatabase(db, init.DeviceID, cruiseID, init.SaleID, units, strata, unit_strata, sampleGroups, new[] { species ?? "dummy" }, null, subPop);

                var plotDs = new PlotDataservice(db, init.CruiseID, init.DeviceID);
                var plotStDs = new PlotStratumDataservice(db, init.CruiseID, init.DeviceID);
                plotDs.AddNewPlot(unitCode);

                var plots = plotDs.GetPlotsByUnitCode(unitCode);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = plotStDs.GetPlot_Strata(unitCode, plot.PlotNumber);
                plotStrata.Should().HaveCount(1);

                db.Insert(new TallyLedger {
                    TallyLedgerID = Guid.NewGuid().ToString(),
                    PlotNumber = plot.PlotNumber,
                    CruiseID = cruiseID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SpeciesCode = species,
                    LiveDead = liveDead ?? "L",
                    TreeCount = 101,
                    KPI = 201 });
                db.Insert(new TallyLedger {
                    TallyLedgerID = Guid.NewGuid().ToString(),
                    PlotNumber = plot.PlotNumber,
                    CruiseID = cruiseID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SpeciesCode = species,
                    LiveDead = liveDead ?? "L",
                    TreeCount = 103,
                    KPI = 203 });


                //db.Execute("INSERT INTO TallyDescription (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead, Description) VALUES " +
                //"(@p1, @p2, @p3, @p4, @p5, @p6);", new object[] { cruiseID, stratum, sampleGroup, species, liveDead, tallyDescription });
                //db.Execute("INSERT INTO TallyHotKey (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead, HotKey) VALUES " +
                //"(@p1, @p2, @p3, @p4, @p5, @p6);", new object[] { cruiseID, stratum, sampleGroup, species, liveDead, hotKey });
                var datastore = new TallyPopulationDataservice(db, cruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var gpops = db.QueryGeneric("SELECT * FROM TallyLedger_Plot_Totals;").ToArray();


                var pops = datastore.GetPlotTallyPopulationsByUnitCode(unitCode, plot.PlotNumber);

                var pop = pops.Single();
                pop.Should().NotBeNull();
                //VerifyTallyPopulation(pop);

                //pop.TallyDescription.Should().NotBeNullOrWhiteSpace();
                //pop.TallyHotKey.Should().NotBeNullOrWhiteSpace();
                pop.Method.Should().NotBeNullOrWhiteSpace();
                pop.TreeCount.Should().Be(204);
                pop.SumKPI.Should().Be(404);
                pop.PlotTreeCount.Should().Be(204);
            }
        }

        [Theory]
        [InlineData("u1", "st1", "sg1", "sp1", "L", true)]
        [InlineData("u1", "st1", "sg1", null, null, false)]
        public void GetTallyPopulation(string unitCode, string stratum, string sampleGroup, string species, string liveDead, bool tallyBySubpop)
        {
            var init = new DatastoreInitializer();
            var tallyDescription = $"{stratum} {sampleGroup} {species} {liveDead}";
            var hotKey = "A";
            var method = CruiseDAL.Schema.CruiseMethods.STR;
            var cruiseID = init.CruiseID;

            var units = new[] { unitCode };
            var strata = new[]
            {
                new Stratum()
                {
                    StratumCode = stratum,
                    Method = method,
                }
            };
            var unit_strata = new[]
            {
                new CuttingUnit_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                }
            };

            var sampleGroups = new[]
            {
                new SampleGroup()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SamplingFrequency = 101,
                    TallyBySubPop = tallyBySubpop,
                }
            };

            var subPop = new[]
            {
                new SubPopulation()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SpeciesCode = species ?? "dummy",
                    LiveDead = liveDead ?? "L",
                }
            };

            using (var database = new CruiseDatastore_V3())
            {
                DatastoreInitializer.InitializeDatabase(database, init.DeviceID, cruiseID, init.SaleID, units, strata, unit_strata, sampleGroups, new[] { species ?? "dummy" }, null, subPop);

                //database.Execute($"INSERT INTO CuttingUnit (CuttingUnitCode, CruiseID) VALUES ('{unitCode}', '{cruiseID}');");

                //database.Execute($"INSERT INTO Stratum (CruiseID, StratumCode, Method) VALUES ('{cruiseID}', '{stratum}', '{method}');");

                //database.Execute($"INSERT INTO CuttingUnit_Stratum (CruiseID, CuttingUnitCode, StratumCode) VALUES " +
                //    $"('{cruiseID}', '{unitCode}','{stratum}');");

                //database.Execute($"INSERT INTO SampleGroup (CruiseID, StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                //    $"('{cruiseID}', '{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                //database.Execute($"INSERT INTO SpeciesCode (CruiseID, SpeciesCode) VALUES ('{cruiseID}', '{((species == null || species == "") ? "dummy" : species)}');");

                //database.Execute(
                //"INSERT INTO SubPopulation (" +
                //"CruiseID, " +
                //"StratumCode, " +
                //"SampleGroupCode, " +
                //"Species, " +
                //"LiveDead)" +
                //"VALUES " +
                //$"('{cruiseID}', '{stratum}', '{sampleGroup}', " +
                //$"'{((species == null || species == "") ? "dummy" : species)}', " +
                //$"'{((liveDead == null || liveDead == "") ? "L" : liveDead)}');");

                database.Execute("INSERT INTO TallyDescription (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead, Description) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5, @p6);", new object[] { cruiseID, stratum, sampleGroup, species, liveDead, tallyDescription });

                database.Execute("INSERT INTO TallyHotKey (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead, HotKey) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5, @p6);", new object[] { cruiseID, stratum, sampleGroup, species, liveDead, hotKey });

                var datastore = new TallyPopulationDataservice(database, cruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var spResult = database.QueryGeneric("select * from SubPopulation;");
                var tpresult = database.QueryGeneric("select * from TallyPopulation;");

                var pop = datastore.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                pop.Should().NotBeNull();

                VerifyTallyPopulation(pop);

                pop.TallyDescription.Should().NotBeNullOrWhiteSpace();
                pop.TallyHotKey.Should().NotBeNullOrWhiteSpace();
                pop.Method.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Theory]
        [InlineData("u1", "st1", "sg1", "sp1", "L", true)]
        [InlineData("u1", "st1", "sg1", null, null, false)]
        public void GetTallyPopulation_With_TreeCountsAndKPIs(string unitCode, string stratum, string sampleGroup, string species, string liveDead, bool tallyBySubpop)
        {
            var init = new DatastoreInitializer();
            var tallyDescription = $"{stratum} {sampleGroup} {species} {liveDead}";
            var hotKey = "A";
            var method = CruiseDAL.Schema.CruiseMethods.STR;
            var cruiseID = init.CruiseID;

            var units = new[] { unitCode };
            var strata = new[]
            {
                new Stratum()
                {
                    StratumCode = stratum,
                    Method = method,
                }
            };
            var unit_strata = new[]
            {
                new CuttingUnit_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                }
            };

            var sampleGroups = new[]
            {
                new SampleGroup()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SamplingFrequency = 101,
                    TallyBySubPop = tallyBySubpop,
                }
            };

            var subPop = new[]
            {
                new SubPopulation()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SpeciesCode = species ?? "dummy",
                    LiveDead = liveDead ?? "L",
                }
            };

            using (var db = new CruiseDatastore_V3())
            {
                DatastoreInitializer.InitializeDatabase(db, init.DeviceID, cruiseID, init.SaleID, units, strata, unit_strata, sampleGroups, new[] { species ?? "dummy" }, null, subPop);

                db.Insert(new TallyLedger { TallyLedgerID = Guid.NewGuid().ToString(), CruiseID = cruiseID, CuttingUnitCode = unitCode, StratumCode = stratum, SampleGroupCode = sampleGroup, SpeciesCode = species, LiveDead = liveDead ?? "L", TreeCount = 101, KPI = 201 });
                db.Insert(new TallyLedger { TallyLedgerID = Guid.NewGuid().ToString(), CruiseID = cruiseID, CuttingUnitCode = unitCode, StratumCode = stratum, SampleGroupCode = sampleGroup, SpeciesCode = species, LiveDead = liveDead ?? "L", TreeCount = 103, KPI = 203 });

                db.Execute("INSERT INTO TallyDescription (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead, Description) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5, @p6);", new object[] { cruiseID, stratum, sampleGroup, species, liveDead, tallyDescription });

                db.Execute("INSERT INTO TallyHotKey (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead, HotKey) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5, @p6);", new object[] { cruiseID, stratum, sampleGroup, species, liveDead, hotKey });

                var datastore = new TallyPopulationDataservice(db, cruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var spResult = db.QueryGeneric("select * from SubPopulation;");
                var tpresult = db.QueryGeneric("select * from TallyPopulation;");

                var pop = datastore.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                pop.Should().NotBeNull();

                VerifyTallyPopulation(pop);

                pop.TallyDescription.Should().NotBeNullOrWhiteSpace();
                pop.TallyHotKey.Should().NotBeNullOrWhiteSpace();
                pop.Method.Should().NotBeNullOrWhiteSpace();
                pop.TreeCount.Should().Be(204);
                pop.SumKPI.Should().Be(404);
            }
        }

        //[Fact]
        //public void GetTallyPopulationsByUnitCode_multiCruise()
        //{
        //    var init1 = new DatastoreInitializer();
        //    var init2 = new DatastoreInitializer("1");

        //    var unitCode = init1.Units[0];

        //    using var db = init1.CreateDatabase();

        //    init2.InitializeCruise(db);

        //    var ds = new TallyPopulationDataservice(db, init1.CruiseID, init1.DeviceID);

        //    var tp = ds.GetTallyPopulationsByUnitCode(unitCode).ToArray();
        //    tp.Should().HaveCount(4);
        //}

        [Fact]
        public void GetTallyPopulationsByUnitCode_with_tallybysubpop_Test()
        {
            var init = new DatastoreInitializer();
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = true;
            var method = CruiseDAL.Schema.CruiseMethods.STR;

            var cruiseID = init.CruiseID;

            var units = new[] { unitCode };
            var strata = new[]
            {
                new Stratum()
                {
                    StratumCode = stratum,
                    Method = method,
                }
            };
            var unit_strata = new[]
            {
                new CuttingUnit_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                }
            };
            var sampleGroups = new[]
            {
                new SampleGroup()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SamplingFrequency = 101,
                    TallyBySubPop = tallyBySubpop,
                }
            };
            var subPop = species.Select(x => new SubPopulation()
            {
                StratumCode = stratum,
                SampleGroupCode = sampleGroup,
                SpeciesCode = x,
                LiveDead = liveDead,
            }).ToArray();

            using (var database = new CruiseDatastore_V3())
            {
                DatastoreInitializer.InitializeDatabase(database, init.DeviceID, cruiseID, init.SaleID, units, strata, unit_strata, sampleGroups, species, null, subPop);

                var datastore = new TallyPopulationDataservice(database, init.CruiseID, init.DeviceID);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                results.Should().HaveCount(species.Count());

                foreach (var pop in results)
                {
                    VerifyTallyPopulation(pop);
                }
            }
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_with_TallyBySG_Test()
        {
            var init = new DatastoreInitializer();
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = false;
            //var method = CruiseDAL.Schema.CruiseMethods.FIX;

            var cruiseID = init.CruiseID;

            var units = new[] { unitCode };
            var strata = new[]
            {
                new Stratum()
                {
                    StratumCode = stratum,
                    Method = CruiseDAL.Schema.CruiseMethods.STR,
                }
            };
            var unit_strata = new[]
            {
                new CuttingUnit_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                }
            };
            var sampleGroups = new[]
            {
                new SampleGroup()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SamplingFrequency = 101,
                    TallyBySubPop = tallyBySubpop,
                }
            };
            var subPop = species.Select(x => new SubPopulation()
            {
                StratumCode = stratum,
                SampleGroupCode = sampleGroup,
                SpeciesCode = x,
                LiveDead = liveDead,
            }).ToArray();

            using (var database = new CruiseDatastore_V3())
            {
                DatastoreInitializer.InitializeDatabase(database, init.DeviceID, cruiseID, init.SaleID, units, strata, unit_strata, sampleGroups, species, null, subPop);

                //database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                //database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                //database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                //    $"('{unitCode}','{stratum}');");

                //database.Execute($"INSERT INTO SampleGroup (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                //    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                //foreach (var sp in species)
                //{
                //    database.Execute($"INSERT INTO SpeciesCode (Species) VALUES ('{sp}');");

                //    database.Execute(
                //        "INSERT INTO SubPopulation (" +
                //        "StratumCode, " +
                //        "SampleGroupCode, " +
                //        "Species, " +
                //        "LiveDead)" +
                //        "VALUES " +
                //        $"('{stratum}', '{sampleGroup}', '{sp}', '{liveDead}');");
                //}

                var datastore = new TallyPopulationDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                results.Should().HaveCount(1);

                foreach (var pop in results)
                {
                    VerifyTallyPopulation(pop);
                }
            }
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_Test_with_clicker_tally()
        {
            var init = new DatastoreInitializer();
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = false;

            var cruiseID = init.CruiseID;

            var units = new[] { unitCode };
            var strata = new[]
            {
                new Stratum()
                {
                    StratumCode = stratum,
                    Method = CruiseDAL.Schema.CruiseMethods.STR,
                }
            };
            var unit_strata = new[]
            {
                new CuttingUnit_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    StratumCode = stratum,
                }
            };
            var sampleGroups = new[]
            {
                new SampleGroup()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    SamplingFrequency = 101,
                    TallyBySubPop = tallyBySubpop,
                    SampleSelectorType = CruiseMethods.CLICKER_SAMPLER_TYPE,
                }
            };
            var subPop = species.Select(x => new SubPopulation()
            {
                StratumCode = stratum,
                SampleGroupCode = sampleGroup,
                SpeciesCode = x,
                LiveDead = liveDead,
            }).ToArray();

            using (var database = new CruiseDatastore_V3())
            {
                DatastoreInitializer.InitializeDatabase(database, init.DeviceID, cruiseID, init.SaleID, units, strata, unit_strata, sampleGroups, species, null, subPop);

                var ds = new TallyPopulationDataservice(database, init.CruiseID, init.DeviceID);

                var results = ds.GetTallyPopulationsByUnitCode(unitCode);

                foreach (var pop in results)
                {
                    pop.IsClickerTally.Should().BeTrue();

                    VerifyTallyPopulation(pop);
                }
            }
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_mutiCruiseDb()
        {
            var init1 = new DatastoreInitializer();
            var init2 = new DatastoreInitializer();

            using var db = init1.CreateDatabase();

            // add a second cruise to the database
            DatastoreInitializer.InitializeDatabase(init2, db);


            var unit = init1.Units.First();
            var tallyPopDs = new TallyPopulationDataservice(db, init1.CruiseID, init1.DeviceID);

            var tp = tallyPopDs.GetTallyPopulationsByUnitCode(unit).First();

            // add some tally ledger entries
            foreach (var i in Enumerable.Range(0, 10))
            {
                var tl = new TallyLedger
                {
                    TallyLedgerID = Guid.NewGuid().ToString(),
                    CruiseID = init1.CruiseID,
                    CuttingUnitCode = unit,
                    StratumCode = tp.StratumCode,
                    SampleGroupCode = tp.SampleGroupCode,
                    SpeciesCode = tp.SpeciesCode,
                    LiveDead = tp.LiveDead,

                    TreeCount = 10,
                };
                db.Insert(tl);

                var tl2 = new TallyLedger
                {
                    TallyLedgerID = Guid.NewGuid().ToString(),
                    CruiseID = init2.CruiseID,
                    CuttingUnitCode = unit,
                    StratumCode = tp.StratumCode,
                    SampleGroupCode = tp.SampleGroupCode,
                    SpeciesCode = tp.SpeciesCode,
                    LiveDead = tp.LiveDead,

                    TreeCount = 10,
                };
                db.Insert(tl2);
            }

            var tpAgian = tallyPopDs.GetTallyPopulationsByUnitCode(unit).First();

            tpAgian.TreeCount.Should().Be(10 * 10);

        }

        [Fact]
        public void GetTallyPopulation_mutiCruiseDb()
        {
            var init1 = new DatastoreInitializer();
            var init2 = new DatastoreInitializer();

            using var db = init1.CreateDatabase();

            // add a second cruise to the database
            DatastoreInitializer.InitializeDatabase(init2, db);


            var unit = init1.Units.First();
            var tallyPopDs = new TallyPopulationDataservice(db, init1.CruiseID, init1.DeviceID);

            var tp = tallyPopDs.GetTallyPopulationsByUnitCode(unit).First();

            // add some tally ledger entries
            foreach (var i in Enumerable.Range(0, 10))
            {
                var tl = new TallyLedger
                {
                    TallyLedgerID = Guid.NewGuid().ToString(),
                    CruiseID = init1.CruiseID,
                    CuttingUnitCode = unit,
                    StratumCode = tp.StratumCode,
                    SampleGroupCode = tp.SampleGroupCode,
                    SpeciesCode = tp.SpeciesCode,
                    LiveDead = tp.LiveDead,

                    TreeCount = 10,
                };
                db.Insert(tl);

                var tl2 = new TallyLedger
                {
                    TallyLedgerID = Guid.NewGuid().ToString(),
                    CruiseID = init2.CruiseID,
                    CuttingUnitCode = unit,
                    StratumCode = tp.StratumCode,
                    SampleGroupCode = tp.SampleGroupCode,
                    SpeciesCode = tp.SpeciesCode,
                    LiveDead = tp.LiveDead,

                    TreeCount = 10,
                };
                db.Insert(tl2);
            }

            var tpAgian = tallyPopDs.GetTallyPopulation(unit, tp.StratumCode, tp.SampleGroupCode, tp.SpeciesCode, tp.LiveDead);

            tpAgian.TreeCount.Should().Be(10 * 10);

        }

        // Plot Tally Populations dont display tree counts.... yet?

        //[Fact]
        //public void GetPlotTallyPopulationsByUnitCode_mutiCruiseDb()
        //{
        //    var init1 = new DatastoreInitializer();
        //    var init2 = new DatastoreInitializer();

        //    using var db = init1.CreateDatabase();

        //    // add a second cruise to the database
        //    DatastoreInitializer.InitializeDatabase(init2, db);

        //    var tallyPopDs = new TallyPopulationDataservice(db, init1.CruiseID, init1.DeviceID);

        //    var unit = init1.Units.First();
        //    var plotNumber = 1;
        //    var stCode = init1.PlotStrata.First().StratumCode;


        //    var plot = new Plot
        //    {
        //        CruiseID = init1.CruiseID,
        //        PlotID = Guid.NewGuid().ToString(),
        //        CuttingUnitCode = unit,
        //        PlotNumber = plotNumber,
        //    };
        //    db.Insert(plot);
            

        //    var tp = tallyPopDs.GetPlotTallyPopulationsByUnitCode(unit, plotNumber).First();

        //    // add some tally ledger entries
        //    foreach (var i in Enumerable.Range(0, 10))
        //    {
        //        var tl = new TallyLedger
        //        {
        //            TallyLedgerID = Guid.NewGuid().ToString(),
        //            CruiseID = init1.CruiseID,
        //            CuttingUnitCode = unit,
        //            PlotNumber = plotNumber,
        //            StratumCode = tp.StratumCode,
        //            SampleGroupCode = tp.SampleGroupCode,
        //            SpeciesCode = tp.SpeciesCode,
        //            LiveDead = tp.LiveDead,

        //            TreeCount = 10,
        //        };
        //        db.Insert(tl);

        //        var tl2 = new TallyLedger
        //        {
        //            TallyLedgerID = Guid.NewGuid().ToString(),
        //            CruiseID = init2.CruiseID,
        //            CuttingUnitCode = unit,
        //            PlotNumber = plotNumber,
        //            StratumCode = tp.StratumCode,
        //            SampleGroupCode = tp.SampleGroupCode,
        //            SpeciesCode = tp.SpeciesCode,
        //            LiveDead = tp.LiveDead,

        //            TreeCount = 10,
        //        };
        //        db.Insert(tl2);
        //    }

        //    var tpAgian = tallyPopDs.GetPlotTallyPopulationsByUnitCode(unit, plotNumber).First();

        //    tpAgian.TreeCount.Should().Be(10 * 10);

        //}



        private static void VerifyTallyPopulation(Models.TallyPopulation result, string species = null)
        {
            if (species != null)
            {
                result.SpeciesCode.Should().Be(species);
            }

            result.SampleGroupCode.Should().NotBeNullOrEmpty();
            result.StratumCode.Should().NotBeNullOrEmpty();

            result.Frequency.Should().BeGreaterThan(0);
        }


        [Fact]
        public void GetTreeCount_Plot()
        {
            var init = new DatastoreInitializer();

            var unit = "u1";
            var stratum = init.PlotStrata[0].StratumCode;
            var sg = "sg1";
            var sp = "sp1";
            var ld = "L";

            var db = init.CreateDatabase();
            var tpDs = new TallyPopulationDataservice(db, init.CruiseID, init.DeviceID);
            var plotDs = new PlotDataservice(db, init.CruiseID, init.DeviceID);
            var treeDs = new PlotTreeDataservice(db, init.CruiseID, init.DeviceID, new SamplerStateDataservice(db, init.CruiseID, init.DeviceID));


            var plot = plotDs.GetPlot(plotDs.AddNewPlot(unit));

            var tallyPop = tpDs.GetPlotTallyPopulation(unit, plot.PlotNumber, stratum, sg, sp, ld);

            tpDs.GetTreeCount(tallyPop, plot.PlotNumber).Should().Be(0);
            treeDs.CreatePlotTree(unit, plot.PlotNumber, stratum, sg, sp, ld);
            tpDs.GetTreeCount(tallyPop, plot.PlotNumber).Should().Be(1);

            var plot2 = plotDs.GetPlot(plotDs.AddNewPlot(unit));

            // validate counts across plots
            tpDs.GetTreeCount(tallyPop, plot2.PlotNumber).Should().Be(0, "check plot 2 initial state");
            treeDs.CreatePlotTree(unit, plot2.PlotNumber, stratum, sg, sp, ld);
            tpDs.GetTreeCount(tallyPop, plot2.PlotNumber).Should().Be(1, "check plot 2");
            tpDs.GetTreeCount(tallyPop, plot.PlotNumber).Should().Be(1, "check plot 1 again");

            // validate counts across a tally population
            var tallyPop2 = tpDs.GetPlotTallyPopulation(unit, plot.PlotNumber, stratum, sg, "sp2", ld);
            tpDs.GetTreeCount(tallyPop2, plot2.PlotNumber).Should().Be(0, "check plot 2 tally pop 2 initial state");
            treeDs.CreatePlotTree(unit, plot2.PlotNumber, stratum, sg, tallyPop2.SpeciesCode, ld);
            tpDs.GetTreeCount(tallyPop2, plot2.PlotNumber).Should().Be(1, "check plot 2 tally pop 2 after");
        }
    }
}