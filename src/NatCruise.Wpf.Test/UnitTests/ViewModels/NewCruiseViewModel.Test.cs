using CruiseDAL;
using CruiseDAL.V3.Models;
using Moq;
using NatCruise.Data;
using NatCruise.Services;
using NatCruise.Test;
using NatCruise.Wpf.ViewModels;
using Xunit.Abstractions;

using Purpose = NatCruise.Models.Purpose;

namespace NatCruise.Wpf.Test.UnitTests.ViewModels
{
    public class NewCruiseViewModel_Test : TestBase
    {
        public NewCruiseViewModel_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SelectTemplate_With_V2_Template()
        {
            var templatePath = GetTestFile("R8 Template 2015.08.05.cut");

            var dsp_moq = new Mock<IDataserviceProvider>();
            var setupInfo_moq = new Mock<ISetupInfoDataservice>();
            var fileDialogService_moq = new Mock<IFileDialogService>();
            var deviceInfo_moq = new Mock<IDeviceInfoService>();
            deviceInfo_moq.Setup(x => x.DeviceID).Returns(Guid.Empty.ToString());

            var vm = new NewCruiseViewModel(dsp_moq.Object, setupInfo_moq.Object, fileDialogService_moq.Object, deviceInfo_moq.Object);

            vm.SelectTemplate(templatePath);

            vm.TemplatePath.Should().Be(templatePath);
        }

        [Fact]
        public async void CreateCruise_With_V2_Template()
        {
            var templatePath = GetTestFile("R8 Template 2015.08.05.cut");
            var cruisePath = GetTempFilePath(".crz3", "CreateCruise_With_V2_Template");
            File.Delete(cruisePath);

            //var dsp_moq = new Mock<IDataserviceProvider>();
            var setupInfo_moq = new Mock<ISetupInfoDataservice>();
            var deviceInfo_moq = new Mock<IDeviceInfoService>();
            deviceInfo_moq.Setup(x => x.DeviceID).Returns(Guid.Empty.ToString());

            var fileDialogService_moq = new Mock<IFileDialogService>();
            fileDialogService_moq.Setup(x => x.SelectCruiseFileDestinationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<string>.FromResult(cruisePath));
            fileDialogService_moq.Setup(x => x.SelectTemplateFileAsync()).ReturnsAsync(templatePath);

            var dataServiceProvider = new WpfDataserviceProvider((CruiseDatastore_V3)null, deviceInfo_moq.Object);

            var vm = new NewCruiseViewModel(dataServiceProvider, setupInfo_moq.Object, fileDialogService_moq.Object, deviceInfo_moq.Object)
            {
                SaleNumber = "12345",
                SaleName = "something",
                UOM = "03",
                Region = "08",
                Forest = "08",
                District = "08",
                Purpose = new Purpose { PurposeCode = "Timber Sale", ShortCode = "TS" },
            };

            vm.SelectTemplate(templatePath);
            vm.TemplatePath.Should().Be(templatePath);

            await vm.CreateCruise();
            vm.HasErrors.Should().BeFalse();

            File.Exists(cruisePath).Should().BeTrue();

            ValidateTemplate(dataServiceProvider.Database);
        }

        protected void ValidateTemplate(CruiseDatastore db)
        {
            db.From<Species>().Count().Should().BeGreaterThan(0);
            db.From<StratumTemplate>().Count().Should().BeGreaterThan(0);
            db.From<Reports>().Count().Should().BeGreaterThan(0);
            db.From<TreeAuditRule>().Count().Should().BeGreaterThan(0);
            db.From<TreeDefaultValue>().Count().Should().BeGreaterThan(0);
            //db.From<BiomassEquation>().Count().Should().BeGreaterThan(0);



        }
    }
}