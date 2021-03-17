using NatCruise.Services;

namespace FScruiser.XF.Services
{
    public class XamarinApplicationSettingService : IApplicationSettingService
    {

        public bool EnableCrashReports
        {
            get => Microsoft.AppCenter.Crashes.Crashes.IsEnabledAsync().Result;
            set => Microsoft.AppCenter.Crashes.Crashes.SetEnabledAsync(value);
        }

        public bool EnableAnalitics
        {
            get => Microsoft.AppCenter.Analytics.Analytics.IsEnabledAsync().Result;
            set => Microsoft.AppCenter.Analytics.Analytics.SetEnabledAsync(value);
        }

        public void Save()
        {
            
        }

    }
}