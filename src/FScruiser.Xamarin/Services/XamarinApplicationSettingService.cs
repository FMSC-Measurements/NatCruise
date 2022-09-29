using NatCruise.Services;
using System;

namespace FScruiser.XF.Services
{
    public class XamarinApplicationSettingService : IApplicationSettingService
    {
         const string PREF_USE_NEW_LIMITING_DISTANCE_CALCULATOR = "UseNewLimitingDistanceCalculator";


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

        public bool UseNewLimitingDistanceCalculator
        {
            get
            {
                try
                {
                    var pref = Xamarin.Essentials.Preferences.Get(PREF_USE_NEW_LIMITING_DISTANCE_CALCULATOR, bool.FalseString);
                    return (Boolean.TryParse(pref, out var value)) ? value : false;
                }
                catch { return false; }
            }
            set => Xamarin.Essentials.Preferences.Set(PREF_USE_NEW_LIMITING_DISTANCE_CALCULATOR, (value) ? bool.TrueString : bool.FalseString);
}

        public void Save()
        {
            
        }

    }
}