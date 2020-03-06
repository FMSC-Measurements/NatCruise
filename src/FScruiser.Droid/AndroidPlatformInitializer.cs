using Android.App;
using FScruiser.Droid.Services;
using FScruiser.Services;
using FScruiser.XF.Services;
using Prism.Ioc;
using System;

namespace FScruiser.Droid
{
    public class AndroidPlatformInitializer : FScruiser.XF.BasePlatformInitializer
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
            containerRegistry.RegisterInstance<IFilePickerService>(new AndroidFilePickerService(HostActivity));
            containerRegistry.RegisterInstance<IDeviceInfoService>(new AndroidDeviceInfoService());
            containerRegistry.RegisterInstance<IAppInfoService>(new AndroidAppInfoService());
        }
    }
}