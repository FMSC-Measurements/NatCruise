using FluentAssertions;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Test;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
{
    public class SamplerStateDataservice_Tests : TestBase
    {
        public SamplerStateDataservice_Tests(ITestOutputHelper output) : base(output)
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
            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var ds = new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                ds.GetSamplerState(stratumCode, sampleGroupCode);
            }
        }

        [Fact]
        public void UpsertSamplerState()
        {
            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var sg = init.SampleGroups[0];
                string stratumCode = sg.StratumCode;
                string sampleGroupCode = sg.SampleGroupCode;

                var ds = new SamplerStateDataservice(database, init.CruiseID, init.DeviceID);

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
            var init = new DatastoreInitializer();

            var fromDeviceID = init.DeviceID;
            var toDeviceID = "toDeviceID";

            var stratum = init.SampleGroups[0].StratumCode;
            var sampleGroup = init.SampleGroups[0].SampleGroupCode;
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
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