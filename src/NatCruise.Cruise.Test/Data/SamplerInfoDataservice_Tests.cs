﻿using FluentAssertions;
using NatCruise.Cruise.Test.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
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
                var ds = new SamplerInfoDataservice(database, CruiseID, new TestDeviceInfoService());


                ds.GetSamplerInfo(stratumCode, sampleGroupCode);
            }


        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, CruiseID, new TestDeviceInfoService());


                ds.GetSamplerState(stratumCode, sampleGroupCode);
            }
        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void UpsertSamplerState(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, CruiseID, new TestDeviceInfoService());


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
            var cruiseID = CruiseID;

            using (var database = CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, cruiseID,
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