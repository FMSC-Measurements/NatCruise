using FluentAssertions;
using FScruiser.Core.Test.Services;
using FScruiser.Data;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Data
{
    public class SamplerInfoDataservice_Tests : Datastore_TestBase
    {
        public SamplerInfoDataservice_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void GetSamplerInfo(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, new TestDeviceInfoService());


                ds.GetSamplerInfo(stratumCode, sampleGroupCode);
            }


        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, new TestDeviceInfoService());


                ds.GetSamplerState(stratumCode, sampleGroupCode);
            }
        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void UpsertSamplerState(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, new TestDeviceInfoService());


                var ss = new SamplerState()
                {
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    BlockState = "something",
                };

                ds.UpsertSamplerState(ss);

                var ssAgain = ds.GetSamplerState(stratumCode, sampleGroupCode);

                ssAgain.Should().BeEquivalentTo(ss);
            }
        }

        [Fact]
        public void CopySamplerStates()
        {
            var fromDeviceID = "fromDeviceID";
            var toDeviceID = "toDeviceID";

            var stratum = "st1";
            var sampleGroup = "sg1";

            using (var database = CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database,
                    new TestDeviceInfoService(fromDeviceID, "fromDeviceName"));

                var ss = new SamplerState()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    BlockState = "something",
                };

                ds.UpsertSamplerState(ss);

                var toDevice = new Device
                {
                    DeviceID = toDeviceID,
                    Name = "toDeviceName",
                };
                database.Insert(toDevice);

                ds.CopySamplerStates(fromDeviceID, toDeviceID);

                var ssAgain = ds.GetSamplerState(stratum, sampleGroup, toDeviceID);
                ssAgain.Should().NotBeNull();


                var stuff = database.QueryGeneric("select * from samplerState ;").ToArray();
            }
        }
    }
}
