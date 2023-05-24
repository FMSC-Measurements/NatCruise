using NatCruise.Services;
using System;

namespace FScruiser.XF.Services
{
    public class XamarinApplicationSettingService : IApplicationSettingService
    {
        const string PREF_USE_NEW_LIMITING_DISTANCE_CALCULATOR = "UseNewLimitingDistanceCalculator";
        const string PREF_SELECT_PREV_NEXT_TREE_SKIPS_COUNT_TREES = "SelectPrevNextTreeSkipsCountTrees";


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
                    return Boolean.TryParse(pref, out var value) ? value : false;
                }
                catch { return false; }
            }
            set => Xamarin.Essentials.Preferences.Set(PREF_USE_NEW_LIMITING_DISTANCE_CALCULATOR, (value) ? bool.TrueString : bool.FalseString);
        }

        public bool SelectPrevNextTreeSkipsCountTrees
        {
            get
            {
                try
                {
                    var pref = Xamarin.Essentials.Preferences.Get(PREF_SELECT_PREV_NEXT_TREE_SKIPS_COUNT_TREES, bool.FalseString);
                    return Boolean.TryParse(pref, out var value) ? value : false;
                }
                catch { return false; }
            }
            set => Xamarin.Essentials.Preferences.Set(PREF_SELECT_PREV_NEXT_TREE_SKIPS_COUNT_TREES, (value) ? bool.TrueString : bool.FalseString);
        }

        public void Save()
        {

        }

    }
}