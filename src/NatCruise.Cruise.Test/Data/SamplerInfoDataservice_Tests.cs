using FluentAssertions;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Test;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
{
    public class SamplerInfoDataservice_Tests : Datastore_TestBase
    {
        public SamplerInfoDataservice_Tests(ITestOutputHelper output) : base(output)
        {
        }

        //[Theory]
        //[InlineData("st1", "sg1")]
        //public void GetSamplerInfo(string stratumCode, string sampleGroupCode)
        //{
        //    using (var database = base.CreateDatabase())
        //    {
        //        var ds = new SamplerInfoDataservice(database, CruiseID, TestDeviceInfoService.TEST_DEVICEID);

        //        ds.GetSamplerInfo(stratumCode, sampleGroupCode);
        //    }
        //}

        [Theory]
        [InlineData("st1", "sg1")]
        public void GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerStateDataservice(database, CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                ds.GetSamplerState(stratumCode, sampleGroupCode);
            }
        }

        [Fact]
        public void UpsertSamplerState()
        {
            using (var database = base.CreateDatabase())
            {
                var sg = SampleGroups[0];
                string stratumCode = sg.StratumCode;
                string sampleGroupCode = sg.SampleGroupCode;

                var ds = new SamplerStateDataservice(database, CruiseID, Initializer.DeviceID);

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
            var fromDeviceID = Initializer.DeviceID;
            var toDeviceID = "toDeviceID";

            var stratum = SampleGroups[0].StratumCode;
            var sampleGroup = SampleGroups[0].SampleGroupCode;
            var cruiseID = CruiseID;

            using (var database = CreateDatabase())
            {
                var ds = new SamplerStateDataservice(database, cruiseID,
                    fromDeviceID);

                var ss = new SamplerState()
                {
                    StratumCode = stratum,
                    SampleGroupCode = sampleGroup,
                    BlockState = "something",
                };

                ds.UpsertSamplerState(ss);

                var toDevice = new Device
                {
                    CruiseID = cruiseID,
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