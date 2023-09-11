using CruiseDAL;
using FluentAssertions;
using NatCruise.Data;
using NatCruise.Test;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
{
    public class SampleSelectorRepository_Tests : TestBase
    {
        public SampleSelectorRepository_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetSamplerBySampleGroupCode()
        {
            var init = new DatastoreInitializer();

            var unitCode = "u1";
            var stratumCode = "st3";
            var sampleGroupCode = "sg1";
            var samplingFrequency = 5;
            var insuranceFreq = 2;
            var method = CruiseDAL.Schema.CruiseMethods.STR;

            using (var database = new CruiseDatastore_V3())
            {
                var saleID = init.SaleID;
                var cruiseID = init.CruiseID;

                DatastoreInitializer.InitializeDatabase(database,
                    init.DeviceID,
                    cruiseID,
                    saleID,
                    new[] { unitCode },
                    new CruiseDAL.V3.Models.Stratum[]
                    {
                        new CruiseDAL.V3.Models.Stratum() {StratumCode = stratumCode, Method = method},
                    },
                    new CruiseDAL.V3.Models.CuttingUnit_Stratum[] {
                        new CruiseDAL.V3.Models.CuttingUnit_Stratum() {CuttingUnitCode = unitCode, StratumCode = stratumCode},
                    },
                    new CruiseDAL.V3.Models.SampleGroup[] {
                        new CruiseDAL.V3.Models.SampleGroup() {
                            StratumCode = stratumCode,
                            SampleGroupCode = sampleGroupCode,
                            SamplingFrequency = samplingFrequency,
                            InsuranceFrequency = insuranceFreq,
                        },
                    },
                    new[] { "sp1", "sp2" },
                    null, null
                    );

                var sgds = new SampleGroupDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var ssds = new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var repo = new SampleSelectorRepository(ssds, sgds);

                var sampler = repo.GetSamplerBySampleGroupCode(stratumCode, sampleGroupCode);
                sampler.ITreeFrequency.Should().Be(insuranceFreq);
                ((FMSC.Sampling.IFrequencyBasedSelecter)sampler).Frequency.Should().Be(samplingFrequency);

                sampler.Should().NotBeNull();

                var samplerAgain = repo.GetSamplerBySampleGroupCode(stratumCode, sampleGroupCode);
                samplerAgain.Should().BeSameAs(sampler);
            }
        }
    }
}