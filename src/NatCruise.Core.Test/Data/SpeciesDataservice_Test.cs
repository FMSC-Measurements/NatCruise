using FluentAssertions;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Species = NatCruise.Models.Species;

namespace NatCruise.Test.Data
{
    public class SpeciesDataservice_Test : TestBase
    {
        public SpeciesDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AddSpecies()
        {
            var speciesCode = "ABC";
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new SpeciesDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var sp = new Species
            {
                SpeciesCode = speciesCode,
            };

            ds.AddSpecies(sp);

            db.ExecuteScalar<int>("SELECT count(*) FROM Species WHERE SpeciesCode = @p1", speciesCode)
                .Should().Be(1);
        }

        [Fact]
        public void GetSpecies()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new SpeciesDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var species = ds.GetSpecies();
            species.Should().NotBeEmpty();
        }

        [Fact]
        public void UpsertSpecies_Insert()
        {
            var speciesCode = "ABC";
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new SpeciesDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var sp = new Species
            {
                SpeciesCode = speciesCode,
            };

            ds.UpsertSpecies(sp);

            db.ExecuteScalar<int>("SELECT count(*) FROM Species WHERE SpeciesCode = @p1", speciesCode)
                .Should().Be(1);

            sp.ContractSpecies = Rand.Word();
            sp.FIACode = Rand.Word();

            ds.UpsertSpecies(sp);

            var spAgain = db.From<Species>().Where("SpeciesCode = @p1").Query(speciesCode).Single();
            spAgain.Should().BeEquivalentTo(sp);
        }
    }
}
