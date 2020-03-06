using CruiseDAL;
using FluentAssertions;
using FScruiser.Data;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Services
{
    public class SampleSelectorRepository_Tests : Datastore_TestBase
    {
        public SampleSelectorRepository_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetSamplerBySampleGroupCode()
        {
            var unitCode = "u1";
            var stratumCode = "st3";
            var sampleGroupCode = "sg1";
            var samplingFrequency = 5;
            var insuranceFreq = 2;
            var method = CruiseDAL.Schema.CruiseMethods.STR;



            using (var database = new CruiseDatastore_V3())
            {
                base.InitializeDatabase(database,
                    new[] { unitCode },
                    new CruiseDAL.V3.Models.Stratum[]
                    {
                        new CruiseDAL.V3.Models.Stratum() {Code = stratumCode, Method = method},

                    },
                    new CruiseDAL.V3.Models.CuttingUnit_Stratum[] {
                        new CruiseDAL.V3.Models.CuttingUnit_Stratum() {CuttingUnitCode = unitCode, StratumCode = stratumCode},
                    },
                    new CruiseDAL.V3.Models.SampleGroup_V3[] {
                        new CruiseDAL.V3.Models.SampleGroup_V3() {
                            StratumCode = stratumCode,
                            SampleGroupCode = sampleGroupCode,
                            SamplingFrequency = samplingFrequency,
                            InsuranceFrequency = insuranceFreq,
                        },
                    },
                    new[] { "sp1", "sp2" },
                    null, null
                    );

                var ds = new SamplerInfoDataservice(database, new TestDeviceInfoService());
                var repo = new SampleSelectorRepository(ds);

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
