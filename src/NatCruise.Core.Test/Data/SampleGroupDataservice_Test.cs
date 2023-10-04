using FluentAssertions;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NatCruise.Test.Data
{
    public class SampleGroupDataservice_Test
    {
        [Fact]
        public void GetSampleGroup()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var ds = new SampleGroupDataservice(db, init.CruiseID, init.DeviceID);

            var stCode = "st1";
            var sgCode = "sg1";
            var sg = ds.GetSampleGroup(stCode, sgCode);
            sg.Should().NotBeNull();
        }

        [Fact]
        public void UpdateSampleGroup()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var ds = new SampleGroupDataservice(db, init.CruiseID, init.DeviceID);

            var stCode = "st1";
            var sgCode = "sg1";
            var sg = ds.GetSampleGroup(stCode, sgCode);
            sg.Should().NotBeNull();

            sg.Description = "somethingElse";
            ds.UpdateSampleGroup(sg);

            var sgAgain = ds.GetSampleGroup(stCode, sgCode);
            sgAgain.Should().NotBeNull();
            sgAgain.Should().BeEquivalentTo(sg);
        }

        [Fact]
        public void DeleteSampleGroup()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var ds = new SampleGroupDataservice(db, init.CruiseID, init.DeviceID);

            var stCode = "st1";
            var sgCode = "sg1";
            var sg = ds.GetSampleGroup(stCode, sgCode);
            sg.Should().NotBeNull();

            ds.DeleteSampleGroup(sg);

            var sgAgain = ds.GetSampleGroup(stCode, sgCode);
            sgAgain.Should().BeNull();
        }

        [Fact]
        public void GetSampleGroupHasFieldData()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var ds = new SampleGroupDataservice(db, init.CruiseID, init.DeviceID);

            var stCode = "st1";
            var sgCode = "sg1";
            var hasFieldData = ds.GetSampleGroupHasFieldData(stCode, sgCode);
            hasFieldData.Should().BeFalse();
        }
    }
}
