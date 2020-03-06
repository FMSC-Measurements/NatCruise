using CruiseDAL;
using CruiseDAL.V3.Models;
using FScruiser.Util;
using System.Linq;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Services
{
    public class Datastore_TestBase : TestBase
    {
        protected string[] Units { get; }
        protected Stratum[] Strata { get; }
        protected CuttingUnit_Stratum[] UnitStrata { get; }
        protected string[] Species { get; }
        protected SampleGroup_V3[] SampleGroups { get; }
        protected TreeDefaultValue[] TreeDefaults { get; }
        protected Subpopulation[] Subpops { get; }
        public Stratum[] PlotStrata { get; }
        public Stratum[] NonPlotStrata { get; }

        public Datastore_TestBase(ITestOutputHelper output) : base(output)
        {
            var units = Units = new string[] { "u1", "u2" };

            var plotStrata = PlotStrata = new[]
            {
                new Stratum{ Code = "st1", Method = "PNT" },
                new Stratum{ Code = "st2", Method = "PCM" },
            };

            var nonPlotStrata= NonPlotStrata = new[]
            {
                new Stratum{ Code = "st3", Method = "STR" },
                new Stratum{ Code = "st4", Method = "STR" },
            };

            var strata = Strata = plotStrata.Concat(nonPlotStrata).ToArray();

            UnitStrata = new[]
            {
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = plotStrata[0].Code },
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = plotStrata[1].Code},
                new CuttingUnit_Stratum {CuttingUnitCode = units[1], StratumCode = plotStrata[1].Code},

                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = nonPlotStrata[0].Code },
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = nonPlotStrata[1].Code},
                new CuttingUnit_Stratum {CuttingUnitCode = units[1], StratumCode = nonPlotStrata[1].Code},
            };

            var species = Species = new string[] { "sp1", "sp2", "sp3" };

            var sampleGroups = SampleGroups = new[]
            {
                new SampleGroup_V3 {SampleGroupCode = "sg1", StratumCode = plotStrata[0].Code, SamplingFrequency = 101, TallyBySubPop = true},
                new SampleGroup_V3 {SampleGroupCode = "sg2", StratumCode = plotStrata[1].Code, SamplingFrequency = 102, TallyBySubPop = false},

                new SampleGroup_V3 {SampleGroupCode = "sg1", StratumCode = nonPlotStrata[0].Code, SamplingFrequency = 101, TallyBySubPop = true},
                new SampleGroup_V3 {SampleGroupCode = "sg2", StratumCode = nonPlotStrata[1].Code, SamplingFrequency = 102, TallyBySubPop = false},
            };

            TreeDefaults = new[]
            {
                new TreeDefaultValue {Species = species[0], LiveDead = "L", PrimaryProduct = "01"},
                new TreeDefaultValue {Species = species[0], LiveDead = "D", PrimaryProduct = "01"},
                new TreeDefaultValue {Species = species[1], LiveDead = "L", PrimaryProduct = "01"},
                new TreeDefaultValue {Species = species[2], LiveDead = "L", PrimaryProduct = "01"},
            };

            Subpops = new[]
            {
                new Subpopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    Species = species[0],
                    LiveDead = "L",
                },
                new Subpopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    Species = species[1],
                    LiveDead = "L",
                },
                new Subpopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    Species = species[2],
                    LiveDead = "L",
                },

                // plot strata
                new Subpopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    Species = species[0],
                    LiveDead = "L",
                },
                new Subpopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    Species = species[1],
                    LiveDead = "L",
                },
                new Subpopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    Species = species[2],
                    LiveDead = "L",
                },
            };
        }

        protected CruiseDatastore_V3 CreateDatabase()
        {
            var units = Units;

            var strata = Strata;

            var unitStrata = UnitStrata;

            var sampleGroups = SampleGroups;

            var species = Species;

            var tdvs = TreeDefaults;

            var subPops = Subpops;

            var database = new CruiseDatastore_V3();

            InitializeDatabase(database, units, strata, unitStrata, sampleGroups, species, tdvs, subPops);

            return database;
        }

        protected void InitializeDatabase(CruiseDatastore_V3 db,
            string[] units,
            CruiseDAL.V3.Models.Stratum[] strata,
            CruiseDAL.V3.Models.CuttingUnit_Stratum[] unitStrata,
            CruiseDAL.V3.Models.SampleGroup_V3[] sampleGroups,
            string[] species,
            CruiseDAL.V3.Models.TreeDefaultValue[] tdvs,
            CruiseDAL.V3.Models.Subpopulation[] subPops)
        {
            //Cutting Units
            foreach (var unit in units.OrEmpty())
            {
                db.Execute(
                    "INSERT INTO CuttingUnit (" +
                    "Code" +
                    ") VALUES " +
                    $"('{unit}');");
            }

            //Strata
            foreach (var st in strata.OrEmpty())
            {
                db.Insert(st);
            }

            //Unit - Strata
            foreach (var cust in unitStrata.OrEmpty())
            {
                db.Insert(cust);
            }

            //Sample Groups
            foreach (var sg in sampleGroups.OrEmpty())
            {
                db.Insert(sg);
            }

            foreach (var sp in species.OrEmpty())
            {
                db.Execute($"INSERT INTO SpeciesCode (Species) VALUES ('{sp}');");
            }

            foreach (var tdv in tdvs.OrEmpty())
            {
                db.Insert(tdv);
            }

            foreach (var sub in subPops.OrEmpty())
            {
                db.Insert(sub);
            }
        }

    //    private void InitializeDatabase(CruiseDatastore_V3 database, string[] units, string[][] strata,
    //string[][] unit_strata, dynamic[] sampleGroups,
    //string[] species, string[][] tdvs, string[][] subPops)
    //    {
    //        //Cutting Units
    //        foreach (var unit in units)
    //        {
    //            database.Execute(
    //                "INSERT INTO CuttingUnit (" +
    //                "Code" +
    //                ") VALUES " +
    //                $"('{unit}');");
    //        }

    //        //Strata
    //        foreach (var st in strata)
    //        {
    //            database.Execute($"INSERT INTO Stratum (Code, Method) VALUES ('{st[0]}', '{st[1]}');");
    //        }

    //        //Unit - Strata
    //        foreach (var cust in unit_strata)
    //        {
    //            database.Execute(
    //                "INSERT INTO CuttingUnit_Stratum " +
    //                "(CuttingUnitCode, StratumCode) " +
    //                "VALUES " +
    //                $"('{cust[0]}','{cust[1]}');");
    //        }

    //        //Sample Groups
    //        foreach (var sg in sampleGroups)
    //        {
    //            database.Execute(
    //                "INSERT INTO SampleGroup_V3 (" +
    //                "StratumCode, " +
    //                "SampleGroupCode," +
    //                "SamplingFrequency, " +
    //                "TallyBySubPop " +
    //                ") VALUES " +
    //                $"('{sg.StCode}', '{sg.SgCode}', {sg.Freq}, {sg.TallyBySp}); ");
    //        }

    //        //TreeDefaults

    //        foreach (var sp in species)
    //        {
    //            database.Execute($"INSERT INTO SpeciesCode (Species) VALUES ('{sp}');");
    //        }

    //        foreach (var tdv in tdvs)
    //        {
    //            database.Execute(
    //                "INSERT INTO TreeDefaultValue (" +
    //                "Species, " +
    //                "LiveDead, " +
    //                "PrimaryProduct" +
    //                ") VALUES " +
    //                $"('{tdv[0]}', '{tdv[1]}', '{tdv[2]}');");
    //        }

    //        foreach (var sub in subPops)
    //        {
    //            database.Execute(
    //                "INSERT INTO SubPopulation (" +
    //                "StratumCode, " +
    //                "SampleGroupCode, " +
    //                "Species, " +
    //                "LiveDead)" +
    //                "VALUES " +
    //                $"('{sub[0]}', '{sub[1]}', '{sub[2]}', '{sub[3]}');");
    //        }
    //    }
    }
}