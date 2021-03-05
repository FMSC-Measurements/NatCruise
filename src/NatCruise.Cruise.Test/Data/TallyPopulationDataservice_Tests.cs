using CruiseDAL;
using FluentAssertions;
using NatCruise.Cruise.Test.Services;
using NatCruise.Cruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CruiseDAL.V3.Models;
using NatCruise.Test;

namespace NatCruise.Cruise.Test.Data
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
            var method = CruiseDAL.Schema.CruiseMethods.STR;
            var cruiseID = CruiseID;

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
                base.InitializeDatabase(database, cruiseID, SaleID, units, strata, unit_strata, sampleGroups, new[] { species ?? "dummy" }, null, subPop);

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

        [Fact]
        public void GetTallyPopulationsByUnitCode_with_tallybysubpop_Test()
        {
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = true;
            var method = CruiseDAL.Schema.CruiseMethods.STR;

            var cruiseID = CruiseID;

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
                base.InitializeDatabase(database, cruiseID, SaleID, units, strata, unit_strata, sampleGroups, species, null, subPop);

                var datastore = new TallyPopulationDataservice(database, CruiseID, TestDeviceInfoService.TEST_DEVICEID);

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

            var cruiseID = CruiseID;

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
                base.InitializeDatabase(database, cruiseID, SaleID, units, strata, unit_strata, sampleGroups, species, null, subPop);

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

                var datastore = new TallyPopulationDataservice(database, CruiseID, TestDeviceInfoService.TEST_DEVICEID);

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

            var cruiseID = CruiseID;

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
                    UseExternalSampler = true,
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
                InitializeDatabase(database, cruiseID, SaleID, units, strata, unit_strata, sampleGroups, species, null, subPop);

                //database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                //database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                //database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                //    $"('{unitCode}','{stratum}');");

                //database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop, UseExternalSampler) VALUES " +
                //    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop}, 1);");

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

                var ds = new TallyPopulationDataservice(database, CruiseID, TestDeviceInfoService.TEST_DEVICEID);

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
                result.SpeciesCode.Should().Be(species);
            }

            result.SampleGroupCode.Should().NotBeNullOrEmpty();
            result.StratumCode.Should().NotBeNullOrEmpty();

            result.Frequency.Should().BeGreaterThan(0);
        }
    }
}
