using CruiseDAL.V3.Models;
using NatCruise.Test;
using NatCruise.Wpf.Data;
using Xunit.Abstractions;

namespace NatCruise.Wpf.Test.UnitTests.Data
{
    public class DesignCheckDataservice_Test : TestBase
    {
        public DesignCheckDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// Test properly set up TDV doesn't return any errors
        /// </summary>
        [Fact]
        public void GetSubPopTDVChecks_Test_WithTDVSetup()
        {
            var init = new DatastoreInitializer();

            var strata = init.Strata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "STR" },
            };

            init.UnitStrata = new[]
            {
                new CuttingUnit_Stratum {CuttingUnitCode = init.Units[0], StratumCode = strata[0].StratumCode },
            };

            var sampleGroups = init.SampleGroups = new[]
            {
                new SampleGroup {
                    SampleGroupCode = "sg1",
                    StratumCode = strata[0].StratumCode,
                    SamplingFrequency = 101,
                    TallyBySubPop = true,
                    PrimaryProduct = "01"
                },
            };

            var species = init.Species;

            var tdvs = init.TreeDefaults = new[]
            {
                new TreeDefaultValue {SpeciesCode = species[0], PrimaryProduct = "01"},
            };

            var subPops = init.Subpops = new[]
            {
                    new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[0],
                    LiveDead = "L",
                },
            };

            using var db = init.CreateDatabase();

            var ds = new DesignCheckDataservice(db, init.CruiseID, init.DeviceID);

            var result = ds.GetSubPopTDVChecks();
            result.Should().BeEmpty();
        }

        /// <summary>
        /// Test TDV check with a catch all TDV (a TDV with Sp, LD, and Prod set to null/any)
        /// </summary>
        [Fact]
        public void GetSubPopTDVChecks_Test_WithCatchAll()
        {
            var init = new DatastoreInitializer();

            var strata = init.Strata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "STR" },
            };

            init.UnitStrata = new[]
            {
                new CuttingUnit_Stratum {CuttingUnitCode = init.Units[0], StratumCode = strata[0].StratumCode },
            };

            var sampleGroups = init.SampleGroups = new[]
            {
                new SampleGroup {
                    SampleGroupCode = "sg1",
                    StratumCode = strata[0].StratumCode,
                    SamplingFrequency = 101,
                    PrimaryProduct = "01",
                    TallyBySubPop = true
                },
            };

            var species = init.Species;

            var tdvs = init.TreeDefaults = new[]
            {
                new TreeDefaultValue {SpeciesCode = species[0], PrimaryProduct = "01"},
                new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = null},
            };

            var subPops = init.Subpops = new[]
            {
                    new SubPopulation
                    {
                        StratumCode = sampleGroups[0].StratumCode,
                        SampleGroupCode = sampleGroups[0].SampleGroupCode,
                        SpeciesCode = species[0],
                        LiveDead = "L",
                    },
                    new SubPopulation
                    {
                        StratumCode = sampleGroups[0].StratumCode,
                        SampleGroupCode = sampleGroups[0].SampleGroupCode,
                        SpeciesCode = species[1],
                        LiveDead = "L",
                    },
            };

            using var db = init.CreateDatabase();

            var ds = new DesignCheckDataservice(db, init.CruiseID, init.DeviceID);

            var result = ds.GetSubPopTDVChecks();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetSubPopTDVChecks_Test_MissingTDV()
        {
            var init = new DatastoreInitializer();

            var strata = init.Strata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "STR" },
            };

            init.UnitStrata = new[]
            {
                new CuttingUnit_Stratum {CuttingUnitCode = init.Units[0], StratumCode = strata[0].StratumCode },
            };

            var sampleGroups = init.SampleGroups = new[]
            {
                new SampleGroup {
                    SampleGroupCode = "sg1",
                    StratumCode = strata[0].StratumCode,
                    SamplingFrequency = 101,
                    TallyBySubPop = true,
                    PrimaryProduct = "01",
                },
            };

            var species = init.Species;

            var tdvs = init.TreeDefaults = new[]
            {
                new TreeDefaultValue {SpeciesCode = species[0], PrimaryProduct = "01"},
            };

            var subPops = init.Subpops = new[]
            {
                    // has TDV
                    new SubPopulation
                    {
                        StratumCode = sampleGroups[0].StratumCode,
                        SampleGroupCode = sampleGroups[0].SampleGroupCode,
                        SpeciesCode = species[0],
                        LiveDead = "L",
                    },
                    // doesn't have TDV
                    new SubPopulation
                    {
                        StratumCode = sampleGroups[0].StratumCode,
                        SampleGroupCode = sampleGroups[0].SampleGroupCode,
                        SpeciesCode = species[1],
                        LiveDead = "L",
                    },
            };

            using var db = init.CreateDatabase();

            var ds = new DesignCheckDataservice(db, init.CruiseID, init.DeviceID);

            var results = ds.GetSubPopTDVChecks();
            results.Should().HaveCount(1);

            var result = results.First();
            Output.WriteLine(result.Message);
        }

        [Fact]
        public void GetTreeFieldSetupChecks()
        {
            var init = new DatastoreInitializer();

            init.UnitStrata = null;
            init.SampleGroups = null;
            init.TreeDefaults = null;
            init.Subpops = null;

            var strata = init.Strata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "STR" },
            };

            using var db = init.CreateDatabase();

            var ds = new DesignCheckDataservice(db, init.CruiseID, init.DeviceID);

            var results = ds.GetTreeFieldSetupChecks();
            results.Should().HaveCount(1);

            var result = results.First();
            Output.WriteLine(result.Message);

            var tfs = new TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                StratumCode = "st1",
                Field = nameof(TreeMeasurment.DBH),
                FieldOrder = 0,
            };
            db.Insert(tfs);

            var results2 = ds.GetTreeFieldSetupChecks();
            results2.Should().HaveCount(0);
        }

        [Fact]
        public void GetTreeFieldSetupHeightFieldChecks()
        {
            var init = new DatastoreInitializer();

            init.UnitStrata = null;
            init.SampleGroups = null;
            init.TreeDefaults = null;
            init.Subpops = null;

            var strata = init.Strata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "STR" },
            };

            using var db = init.CreateDatabase();

            var ds = new DesignCheckDataservice(db, init.CruiseID, init.DeviceID);

            var results = ds.GetTreeFieldSetupChecks();
            results.Should().HaveCount(1);

            var result = results.First();
            Output.WriteLine(result.Message);

            var tfs = new TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                StratumCode = "st1",
                Field = nameof(TreeMeasurment.TotalHeight),
                FieldOrder = 0,
            };
            db.Insert(tfs);

            var results2 = ds.GetTreeFieldSetupHeightFieldChecks();
            results2.Should().HaveCount(0);
        }

        [Fact]
        public void GetTreeFieldSetupDiameterFieldChecks()
        {
            var init = new DatastoreInitializer();

            init.UnitStrata = null;
            init.SampleGroups = null;
            init.TreeDefaults = null;
            init.Subpops = null;

            var strata = init.Strata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "STR" },
            };

            using var db = init.CreateDatabase();

            var ds = new DesignCheckDataservice(db, init.CruiseID, init.DeviceID);

            var results = ds.GetTreeFieldSetupChecks();
            results.Should().HaveCount(1);

            var result = results.First();
            Output.WriteLine(result.Message);

            var tfs = new TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                StratumCode = "st1",
                Field = nameof(TreeMeasurment.DBH),
                FieldOrder = 0,
            };
            db.Insert(tfs);

            var results2 = ds.GetTreeFieldSetupDiameterFieldChecks();
            results2.Should().HaveCount(0);
        }

        [Fact]
        public void GetFIACodeChecks()
        {
            var init = new DatastoreInitializer();

            init.UnitStrata = null;
            init.SampleGroups = null;
            init.TreeDefaults = null;
            init.Subpops = null;
            init.Species = null;

            using var db = init.CreateDatabase();

            var species = new Species
            {
                CruiseID = init.CruiseID,
                SpeciesCode = "sp1"
            };
            db.Insert(species);

            var ds = new DesignCheckDataservice(db, init.CruiseID, init.DeviceID);

            var results = ds.GetFIACodeChecks();
            results.Should().HaveCount(1);

            var result = results.First();
            Output.WriteLine(result.Message);
        }
    }
}