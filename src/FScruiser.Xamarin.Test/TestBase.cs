using Prism;
using Xunit.Abstractions;

namespace FScruiser.XF.Test
{
    public class TestBase
    {
        protected App App { get; }
        protected ITestOutputHelper Output { get; }
        public IPlatformInitializer PlatformInitializer { get; }

        protected Prism.Ioc.IContainerExtension Container => (Prism.Ioc.IContainerExtension)App.Container;

        public TestBase(ITestOutputHelper output) : this(output, null)
        { }

        public TestBase(ITestOutputHelper output, IPlatformInitializer platformInitializer)
        {
            PlatformInitializer = platformInitializer ?? new TestPlatformInitializer(output);
            Output = output;
            Xamarin.Forms.Mocks.MockForms.Init();

            App = new App(PlatformInitializer);
        }
    }
}