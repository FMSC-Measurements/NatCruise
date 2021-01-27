using CruiseDAL;
using CruiseDAL.V3.Models;
using System.Linq;
using Xunit.Abstractions;
using NatCruise.Util;
using System;

namespace NatCruise.Cruise.Test.Services
{
    public class Datastore_TestBase : TestBase
    {
        public string CruiseID { get; }
        public string SaleID { get; }

        protected string[] Units { get; }
        protected Stratum[] Strata { get; }
        protected CuttingUnit_Stratum[] UnitStrata { get; }
        protected string[] Species { get; }
        protected SampleGroup[] SampleGroups { get; }
        protected TreeDefaultValue[] TreeDefaults { get; }
        protected SubPopulation[] Subpops { get; }
        public Stratum[] PlotStrata { get; }
        public Stratum[] NonPlotStrata { get; }

        public Datastore_TestBase(ITestOutputHelper output) : base(output)
        {
            CruiseID = Guid.NewGuid().ToString();
            SaleID = Guid.NewGuid().ToString();

            var units = Units = new string[] { "u1", "u2" };

            var plotStrata = PlotStrata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "PNT" },
                new Stratum{ StratumCode = "st2", Method = "PCM" },
            };

            var nonPlotStrata = NonPlotStrata = new[]
            {
                new Stratum{ StratumCode = "st3", Method = "STR" },
                new Stratum{ StratumCode = "st4", Method = "STR" },
            };

            var strata = Strata = plotStrata.Concat(nonPlotStrata).ToArray();

            UnitStrata = new[]
            {
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = plotStrata[0].StratumCode },
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = plotStrata[1].StratumCode},
                new CuttingUnit_Stratum {CuttingUnitCode = units[1], StratumCode = plotStrata[1].StratumCode},

                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = nonPlotStrata[0].StratumCode },
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = nonPlotStrata[1].StratumCode},
                new CuttingUnit_Stratum {CuttingUnitCode = units[1], StratumCode = nonPlotStrata[1].StratumCode},
            };

            var species = Species = new string[] { "sp1", "sp2", "sp3" };

            var sampleGroups = SampleGroups = new[]
            {
                new SampleGroup {SampleGroupCode = "sg1", StratumCode = plotStrata[0].StratumCode, SamplingFrequency = 101, TallyBySubPop = true},
                new SampleGroup {SampleGroupCode = "sg2", StratumCode = plotStrata[1].StratumCode, SamplingFrequency = 102, TallyBySubPop = false},

                new SampleGroup {SampleGroupCode = "sg1", StratumCode = nonPlotStrata[0].StratumCode, SamplingFrequency = 101, TallyBySubPop = true},
                new SampleGroup {SampleGroupCode = "sg2", StratumCode = nonPlotStrata[1].StratumCode, SamplingFrequency = 102, TallyBySubPop = false},
            };

            TreeDefaults = new[]
            {
                new TreeDefaultValue {SpeciesCode = species[0], PrimaryProduct = "01"},
                new TreeDefaultValue {SpeciesCode = species[1], PrimaryProduct = "01"},
                new TreeDefaultValue {SpeciesCode = species[2], PrimaryProduct = "01"},
            };

            Subpops = new[]
            {
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[0],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[0],
                    LiveDead = "D",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[1],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[2],
                    LiveDead = "L",
                },

                // plot strata
                new SubPopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    SpeciesCode = species[0],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    SpeciesCode = species[1],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    SpeciesCode = species[2],
                    LiveDead = "L",
                },
            };
        }

        protected CruiseDatastore_V3 CreateDatabase()
        {
            var cruiseID = CruiseID;
            var saleID = SaleID;

            var units = Units;

            var strata = Strata;

            var unitStrata = UnitStrata;

            var sampleGroups = SampleGroups;

            var species = Species;

            var tdvs = TreeDefaults;

            var subPops = Subpops;

            var database = new CruiseDatastore_V3();

            InitializeDatabase(database, cruiseID, saleID, units, strata, unitStrata, sampleGroups, species, tdvs, subPops);

            return database;
        }

        protected CruiseDatastore_V3 CreateDatabase(string path, string cruiseID = null, string saleID = null)
        {
            cruiseID = cruiseID ?? Guid.NewGuid().ToString();
            saleID = saleID ?? Guid.NewGuid().ToString();

            var units = Units;

            var strata = Strata;

            var unitStrata = UnitStrata;

            var sampleGroups = SampleGroups;

            var species = Species;

            var tdvs = TreeDefaults;

            var subPops = Subpops;

            var database = new CruiseDatastore_V3(path, true);

            InitializeDatabase(database, cruiseID, saleID, units, strata, unitStrata, sampleGroups, species, tdvs, subPops);

            return database;
        }

        protected void InitializeDatabase(CruiseDatastore_V3 db,
            string cruiseID,
            string saleID,
            string[] units,
            CruiseDAL.V3.Models.Stratum[] strata,
            CruiseDAL.V3.Models.CuttingUnit_Stratum[] unitStrata,
            CruiseDAL.V3.Models.SampleGroup[] sampleGroups,
            string[] species,
            CruiseDAL.V3.Models.TreeDefaultValue[] tdvs,
            CruiseDAL.V3.Models.SubPopulation[] subPops)
        {

            db.Insert(new Sale()
            {
                SaleID = saleID,
                SaleNumber = (saleID.GetHashCode() % 10000).ToString(),
            }, option: Backpack.SqlBuilder.OnConflictOption.Ignore); ;

            db.Insert(new CruiseDAL.V3.Models.Cruise()
            {
                CruiseID = cruiseID,
                SaleID = saleID,
            }, option: Backpack.SqlBuilder.OnConflictOption.Ignore);


            //Cutting Units
            foreach (var unit in units.OrEmpty())
            {
                var unitID = Guid.NewGuid().ToString();
                db.Execute(
                    "INSERT INTO CuttingUnit (" +
                    "CruiseID, CuttingUnitID, CuttingUnitCode" +
                    ") VALUES " +
                    $"('{cruiseID}', '{unitID}', '{unit}');");
            }

            //Strata
            foreach (var st in strata.OrEmpty())
            {
                st.CruiseID = cruiseID;
                st.StratumID = Guid.NewGuid().ToString();
                db.Insert(st);
            }

            //Unit - Strata
            foreach (var cust in unitStrata.OrEmpty())
            {
                cust.CruiseID = cruiseID;
                db.Insert(cust);
            }

            //Sample Groups
            foreach (var sg in sampleGroups.OrEmpty())
            {
                sg.SampleGroupID = Guid.NewGuid().ToString();
                sg.CruiseID = cruiseID;
                db.Insert(sg);
            }

            foreach (var sp in species.OrEmpty())
            {
                db.Execute($"INSERT INTO Species (CruiseID, SpeciesCode) VALUES ('{cruiseID}', '{sp}');");
            }

            foreach (var tdv in tdvs.OrEmpty())
            {
                tdv.CruiseID = cruiseID;
                db.Insert(tdv);
            }

            foreach (var sub in subPops.OrEmpty())
            {
                sub.SubPopulationID = Guid.NewGuid().ToString();
                sub.CruiseID = cruiseID;
                db.Insert(sub);
            }

//            var treeID = Guid.NewGuid().ToString();

//            db.Execute("DROP TABLE Tree");

//            db.Execute(@"CREATE TABLE Tree (
//    Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
//    CruiseID TEXT NOT NULL COLLATE NOCASE,
//    TreeID TEXT NOT NULL ,
//    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
//    StratumCode TEXT NOT NULL COLLATE NOCASE,
//    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
//    SpeciesCode TEXT COLLATE NOCASE,
//    LiveDead TEXT COLLATE NOCASE,
//    PlotNumber INTEGER,
//    TreeNumber INTEGER NOT NULL,
//    CountOrMeasure TEXT DEFAULT 'M' COLLATE NOCASE, -- field is for compatibility with older schema. because plot cruising still requires a tree record to record non measure trees                               // initials of the cruiser taking measurments

//    CreatedBy TEXT DEFAULT 'none',
//    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
//    ModifiedBy TEXT,
//    Modified_TS DATETIME,

//    UNIQUE (TreeID),

//    CHECK (TreeID LIKE '________-____-____-____-____________'),
//    CHECK (CountOrMeasure IN ('C', 'M', 'I')),
//    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),

//    FOREIGN KEY (CuttingUnitCode, CruiseID)
//        REFERENCES CuttingUnit (CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
//    --FOREIGN KEY (SampleGroupCode, StratumCode, CruiseID)
//    --    REFERENCES SampleGroup (SampleGroupCode, StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
//    --FOREIGN KEY (PlotNumber, CuttingUnitCode, CruiseID)
//    --    REFERENCES Plot (PlotNumber, CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
//    --FOREIGN KEY (Species, LiveDead, SampleGroupCode, StratumCode)
//    --    REFERENCES SubPopulation (Species, LiveDead, SampleGroupCode, StratumCode),
//    --FOREIGN KEY (SpeciesCode, CruiseID)
//    --    REFERENCES Species (SpeciesCode, CruiseID)
//)");

//            var _cu = db.From<CuttingUnit>().Query().First();
//            var _sg = db.From<SampleGroup>().Query().First();
//            var tree = new Tree()
//            {
//                CruiseID = _cu.CruiseID,
//                TreeID = treeID,
//                TreeNumber = 1,
//                CuttingUnitCode = _cu.CuttingUnitCode,
//                StratumCode = _sg.StratumCode,
//                SampleGroupCode = _sg.SampleGroupCode,
//            };

//            db.Insert(tree);
        }
    }
}