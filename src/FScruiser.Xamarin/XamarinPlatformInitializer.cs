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
            containerRegistry.RegisterSingleton<IDialogService, XamarinDialogService>();
            containerRegistry.RegisterInstance<Prism.Logging.ILoggerFacade>(new AppCenterLoggerService());
        }
    }
}