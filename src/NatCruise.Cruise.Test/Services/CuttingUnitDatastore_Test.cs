using FluentAssertions;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Services
{
    public class CuttingUnitDatastore_Test : Datastore_TestBase
    {
        public CuttingUnitDatastore_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("u1", "st3", "st4")]
        [InlineData("u2", "st4")]
        public void GetStrataByUnitCode_Test(string unitCode, params string[] expectedStrataCodes)
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var strata = database.Query<Stratum>
                    ("select * from stratum;").ToArray();

                var stuff = database.QueryGeneric("select * from Stratum;").ToArray();

                var results = datastore.GetStrataByUnitCode(unitCode);

                var strata_codes = results.Select(x => x.Code);
                strata_codes.Should().Contain(expectedStrataCodes);
                strata_codes.Should().HaveSameCount(expectedStrataCodes);
            }
        }



        #region tally entry

        

        #endregion tally entry
    }
}