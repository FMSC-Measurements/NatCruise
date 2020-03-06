using CruiseDAL;
using FluentAssertions;
using FScruiser.Core.Test.Services;
using FScruiser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Data
{
    public class TallyPopulationDataservice_Tests : Datastore_TestBase
    {
        public TallyPopulationDataservice_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("u1", "st1", "sg1", "sp1", "L", true)]
        [InlineData("u1", "st1", "sg1", null, null, false)]
        public void GetTallyPopulation(string unitCode, string stratum, string sampleGroup, string species, string liveDead, bool tallyBySubpop)
        {
            var tallyDescription = $"{stratum} {sampleGroup} {species} {liveDead}";
            var hotKey = "A";
            var method = CruiseDAL.Schema.CruiseMethods.FIX;

            using (var database = new CruiseDatastore_V3())
            {
                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code, Method) VALUES ('{stratum}', '{method}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                database.Execute($"INSERT INTO SpeciesCode (Species) VALUES ('{((species == null || species == "") ? "dummy" : species)}');");

                database.Execute(
                "INSERT INTO SubPopulation (" +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead)" +
                "VALUES " +
                $"('{stratum}', '{sampleGroup}', " +
                $"'{((species == null || species == "") ? "dummy" : species)}', " +
                $"'{((liveDead == null || liveDead == "") ? "L" : liveDead)}');");

                database.Execute("INSERT INTO TallyDescription (StratumCode, SampleGroupCode, Species, LiveDead, Description) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5);", new object[] { stratum, sampleGroup, species, liveDead, tallyDescription });

                database.Execute("INSERT INTO TallyHotKey (StratumCode, SampleGroupCode, Species, LiveDead, HotKey) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5);", new object[] { stratum, sampleGroup, species, liveDead, hotKey });

                var datastore = new TallyPopulationDataservice(database);

                var pop = datastore.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                pop.Should().NotBeNull();

                VerifyTallyPopulation(pop);

                pop.TallyDescription.Should().NotBeNullOrWhiteSpace();
                pop.TallyHotKey.Should().NotBeNullOrWhiteSpace();
                pop.Method.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_with_tallybysubpop_Test()
        {
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = true;
            //var method = CruiseDAL.Schema.CruiseMethods.FIX;

            using (var database = new CruiseDatastore_V3())
            {
                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                foreach (var sp in species)
                {
                    database.Execute($"INSERT INTO SpeciesCode (Species) VALUES ('{sp}');");

                    database.Execute(
                        "INSERT INTO SubPopulation (" +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead)" +
                        "VALUES " +
                        $"('{stratum}', '{sampleGroup}', '{sp}', '{liveDead}');");
                }

                var datastore = new TallyPopulationDataservice(database);

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
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = false;
            //var method = CruiseDAL.Schema.CruiseMethods.FIX;

            using (var database = new CruiseDatastore_V3())
            {
                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                foreach (var sp in species)
                {
                    database.Execute($"INSERT INTO SpeciesCode (Species) VALUES ('{sp}');");

                    database.Execute(
                        "INSERT INTO SubPopulation (" +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead)" +
                        "VALUES " +
                        $"('{stratum}', '{sampleGroup}', '{sp}', '{liveDead}');");
                }

                var datastore = new TallyPopulationDataservice(database);

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
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = false;

            using (var database = new CruiseDatastore_V3())
            {
                var ds = new TallyPopulationDataservice(database);

                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop, UseExternalSampler) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop}, 1);");

                foreach (var sp in species)
                {
                    database.Execute($"INSERT INTO SpeciesCode (Species) VALUES ('{sp}');");

                    database.Execute(
                        "INSERT INTO SubPopulation (" +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead)" +
                        "VALUES " +
                        $"('{stratum}', '{sampleGroup}', '{sp}', '{liveDead}');");
                }

                //database.Execute($"INSERT INTO SamplerState (StratumCode, SampleGroupCode, SampleSelectorType) " +
                //    $"SELECT StratumCode, SampleGroupCode, '{CruiseDAL.Schema.CruiseMethods.CLICKER_SAMPLER_TYPE}' AS SampleSelectorType FROM SampleGroup_V3;");

                var results = ds.GetTallyPopulationsByUnitCode(unitCode);
                //results.Should().HaveCount(2);

                foreach (var pop in results)
                {
                    pop.UseExternalSampler.Should().BeTrue();

                    VerifyTallyPopulation(pop);
                }
            }
        }

        private static void VerifyTallyPopulation(Models.TallyPopulation result, string species = null)
        {
            if (species != null)
            {
                result.Species.Should().Be(species);
            }

            result.SampleGroupCode.Should().NotBeNullOrEmpty();
            result.StratumCode.Should().NotBeNullOrEmpty();

            result.Frequency.Should().BeGreaterThan(0);
        }
    }
}
