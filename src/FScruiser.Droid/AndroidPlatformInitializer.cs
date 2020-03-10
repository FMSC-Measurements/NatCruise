using Android.App;
using FScruiser.Droid.Services;
using NatCruise.Cruise.Services;
using NatCruise.Services;
using Prism.Ioc;
using System;

namespace FScruiser.Droid
{
    public class AndroidPlatformInitializer : FScruiser.XF.XamarinPlatformInitializer
    {
        public AndroidPlatformInitializer(Activity hostActivity)
        {
            HostActivity = hostActivity ?? throw new ArgumentNullException(nameof(hostActivity));
        }

        public Activity HostActivity { get; protected set; }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterTypes(containerRegistry);

            containerRegistry.RegisterInstance<ISoundService>(new AndroidSoundService(HostActivity.ApplicationContext));
            containerRegistry.RegisterInstance<IFileDialogService>(new AndroidFileDialogService(HostActivity));
            containerRegistry.RegisterInstance<IDeviceInfoService>(new AndroidDeviceInfoService());
            containerRegistry.RegisterInstance<IAppInfoService>(new AndroidAppInfoService());
        }
    }
}