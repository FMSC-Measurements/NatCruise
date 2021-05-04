using NatCruise.Services;

namespace NatCruise.Wpf.Services
{
    public class WpfApplicationSettingService : IApplicationSettingService
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