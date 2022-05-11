using FluentAssertions;
using NatCruise.Data;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class SetupInfoDataservice_Test : TestBase
    {
        public SetupInfoDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("01")]
        public void GetForests(string region)
        {
            var svc = new SetupInfoDataservice();

            var forests = svc.GetForests(region);
            forests.Should().NotBeEmpty();
        }

        [Fact]
        public void GetPurposes()
        {
            var svc = new SetupInfoDataservice();

            var purposes = svc.GetPurposes();
            purposes.Should().NotBeEmpty();
        }

        [Fact]
        public void GetRegions()
        {
            var svc = new SetupInfoDataservice();

            var regions = svc.GetRegions();
            regions.Should().NotBeEmpty();
        }

        [Fact]
        public void GetUOMCodes()
        {
            var svc = new SetupInfoDataservice();

            var uomCodes = svc.GetUOMCodes();
            uomCodes.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("01")]
        public void GetDistricts(string region)
        {
            var svc = new SetupInfoDataservice();

            var forests = svc.GetForests(region);
            forests.Should().NotBeEmpty();

            foreach (var f in forests)
            {
                var districts = svc.GetDistricts(f.RegionCode, f.ForestCode);
                districts.Should().NotBeEmpty();
            }
        }

        [Fact]
        public void GetFiaSpecies()
        {
            var svc = new SetupInfoDataservice();

            var fiaCodes = svc.GetFIASpecies();
            fiaCodes.Should().NotBeEmpty();
        }
    }
}