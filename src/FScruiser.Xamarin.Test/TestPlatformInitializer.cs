using FScruiser.Services;
using FScruiser.XF.Services;
using Moq;
using Prism.Ioc;
using System;
using Xunit.Abstractions;

namespace FScruiser.XF
{
    public class TestPlatformInitializer : Prism.IPlatformInitializer
    {

        public Xunit.Abstractions.ITestOutputHelper TestOutput { get; set; }
        public TestDialogService TestDialogService { get; private set; }

        public TestPlatformInitializer(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput ?? throw new ArgumentNullException(nameof(testOutput));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var mockSoundService = new Mock<ISoundService>();
            var mockDialogService = new Mock<IDialogService>();

            containerRegistry.RegisterInstance<ISoundService>(mockSoundService.Object);
            containerRegistry.RegisterInstance<IDialogService>(mockDialogService.Object);
            containerRegistry.RegisterInstance<Prism.Logging.ILoggerFacade>(new TestLogger(TestOutput));
        }
    }
}