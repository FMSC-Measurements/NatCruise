using Android.App;
using FScruiser.Droid.Services;
using FScruiser.XF.Services;
using NatCruise.Core.Services;
using NatCruise.Cruise.Services;
using NatCruise.Services;
using Prism.Ioc;
using System;

namespace FScruiser.Droid
{
    public class AndroidPlatformInitializer : FScruiser.XF.XamarinPlatformInitializer
    {
        public AndroidPlatformInitializer(MainActivity hostActivity)
        {
            HostActivity = hostActivity ?? throw new ArgumentNullException(nameof(hostActivity));
        }

        public MainActivity HostActivity { get; protected set; }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var context = HostActivity.ApplicationContext;

            containerRegistry.RegisterInstance<IActivityService>(new ActivityService(HostActivity));
            containerRegistry.RegisterInstance<IFileDialogService>(new AndroidFileDialogService(HostActivity));
            containerRegistry.RegisterInstance<IFileSystemService>(new AndroidFileSystemService(context));
            containerRegistry.RegisterInstance<ISoundService>(new AndroidSoundService(context));
            //containerRegistry.RegisterInstance<IFileDialogService>(new AndroidFileDialogService(HostActivity));
            containerRegistry.RegisterInstance<IDeviceInfoService>(new AndroidDeviceInfoService(context));
            containerRegistry.RegisterInstance<IAppInfoService>(new AndroidAppInfoService());

            base.RegisterTypes(containerRegistry);
        }
    }
}