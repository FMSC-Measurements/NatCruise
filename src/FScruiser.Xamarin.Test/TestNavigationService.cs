using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
using System.Threading.Tasks;

namespace FScruiser.XF
{
    public class TestNavigationService : PageNavigationService
    {
        // MOQ requires a parameterless constructor:
        public TestNavigationService()
            : base(null, null, null, null)
        { }

        public TestNavigationService(IContainerExtension container, IApplicationProvider applicationProvider,
                                     IPageBehaviorFactory pageBehaviorFactory, ILoggerFacade logger)
            : base(container, applicationProvider, pageBehaviorFactory, logger)
        {
        }

        protected override Task<INavigationResult> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return base.GoBackInternal(parameters, useModalNavigation, animated);
        }

        protected override Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return base.NavigateInternal(name, parameters, useModalNavigation, animated);
        }

        // Attempt at 'replacing' the NavigateAsync extension method, though I suspect this is not the right thing to do.
        public virtual Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return null;
        }
    }
}