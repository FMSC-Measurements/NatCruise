using FScruiser.Services;
using FScruiser.XF.Services;
using Prism;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF
{
    public class BasePlatformInitializer : IPlatformInitializer
    {
        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDialogService, XamarinDialogService>();
            containerRegistry.RegisterInstance<Prism.Logging.ILoggerFacade>(new AppCenterLoggerService());
        }
    }
}
