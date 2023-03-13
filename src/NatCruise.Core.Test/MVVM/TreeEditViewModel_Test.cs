using FluentAssertions;
using Moq;
using NatCruise.Data;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.MVVM
{
    public class TreeEditViewModel_Test : TestBase
    {
        public TreeEditViewModel_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Load_With_TreeID()
        {
            var init = new DatastoreInitializer();
            var db = init.CreateDatabase();

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            var treeDataService = dsp.GetDataservice<ITreeDataservice>();

            var mockDialogService = new Mock<INatCruiseDialogService>();
            var mockNavService = new Mock<INatCruiseNavigationService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruisersService = new Mock<ICruisersDataservice>();

            var treeEditVM = new TreeEditViewModel(dsp.GetDataservice<IStratumDataservice>(),
                dsp.GetDataservice<ISampleGroupDataservice>(),
                dsp.GetDataservice<ISubpopulationDataservice>(),
                treeDataService,
                dsp.GetDataservice<ITreeErrorDataservice>(),
                dsp.GetDataservice<ITreeFieldValueDataservice>(),
                mockDialogService.Object,
                mockNavService.Object,
                mockCruisersService.Object,
                mockLoggingService.Object);


            var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1");

            treeEditVM.Load(treeID);

            treeEditVM.Tree.Should().NotBeNull();
        }

        [Fact]
        public void EditTreeRemarks()
        {
            var init = new DatastoreInitializer();
            var db = init.CreateDatabase();

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            var treeDataService = dsp.GetDataservice<ITreeDataservice>();

            var mockDialogService = new Mock<INatCruiseDialogService>();
            var mockNavService = new Mock<INatCruiseNavigationService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruisersService = new Mock<ICruisersDataservice>();

            var treeEditVM = new TreeEditViewModel(dsp.GetDataservice<IStratumDataservice>(),
                dsp.GetDataservice<ISampleGroupDataservice>(),
                dsp.GetDataservice<ISubpopulationDataservice>(),
                treeDataService,
                dsp.GetDataservice<ITreeErrorDataservice>(),
                dsp.GetDataservice<ITreeFieldValueDataservice>(),
                mockDialogService.Object,
                mockNavService.Object,
                mockCruisersService.Object,
                mockLoggingService.Object);


            var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1");

            treeEditVM.Load(treeID);

            var remarksValue = "something";
            treeEditVM.Remarks = remarksValue;

            var tree = db.From<CruiseDAL.V3.Models.Tree>().Query().Single();
            var treeMeasurment = db.From<CruiseDAL.V3.Models.TreeMeasurment>().Query().Single();
            var tallyLedger = db.From<CruiseDAL.V3.Models.TallyLedger>().Query().Single();
            treeMeasurment.Remarks.Should().Be(remarksValue);
        }
    }
}
