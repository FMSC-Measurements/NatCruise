using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Design.Data;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Design.Test.Data
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
    }
}
