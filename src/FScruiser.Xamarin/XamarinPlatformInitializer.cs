using NatCruise.Cruise.Services;
using FScruiser.XF.Services;
using Prism;
using Prism.Ioc;

namespace FScruiser.XF
{
    public class XamarinPlatformInitializer : IPlatformInitializer
    {
        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ICruiseDialogService, XamarinCruiseDialogService>();
            containerRegistry.RegisterInstance<Prism.Logging.ILoggerFacade>(new AppCenterLoggerService());
        }
    }
}