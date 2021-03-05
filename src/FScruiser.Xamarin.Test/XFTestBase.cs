using NatCruise.Test;
using Prism;
using Xunit.Abstractions;

namespace FScruiser.XF.Test
{
    public class XFTestBase : TestBase
    {
        protected App App { get; }
        public IPlatformInitializer PlatformInitializer { get; }

        protected Prism.Ioc.IContainerExtension Container => (Prism.Ioc.IContainerExtension)App.Container;

        public XFTestBase(ITestOutputHelper output) : this(output, null)
        { }

        public XFTestBase(ITestOutputHelper output, IPlatformInitializer platformInitializer) : base(output)
        {
            PlatformInitializer = platformInitializer ?? new TestPlatformInitializer(output);
            Xamarin.Forms.Mocks.MockForms.Init();

            App = new App(PlatformInitializer);
        }
    }
}