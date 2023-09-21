using NatCruise.Models;
using NatCruise.Util;
using Newtonsoft.Json;
using Prism.Mvvm;
using System.Collections.Generic;

namespace FScruiser.XF.Data
{
    public class TallySettings
    {
        public bool EnableCruiserPopup { get; set; }

        public bool EnableAskEnterTreeData { get; set; }

        public List<string> Cruisers { get; set; } = new List<string>();
    }

    public class TallySettingsDataService : BindableBase, ITallySettingsDataService
    {
        const string PERF_TallyButtonTrayVerticalSize = "TallyButtonTrayVerticalSize";
        const string PERF_PlotTallyButtonTrayVerticalSize = "PlotTallyButtonTrayVerticalSize";

        private const double DEFAULT_TALLY_BUTTON_TRAY_VERTICAL_SIZE = 0.4;
        private const double DEFAULT_PLOT_TALLY_BUTTON_TRAY_VERTICAL_SIZE = 0.4;

        protected TallySettings Data { get; set; }
        protected object DataSyncLock { get; } = new object();

        public bool EnableCruiserPopup
        {
            get
            {
                EnsureLoaded();
                return Data.EnableCruiserPopup;
            }
            set => Data.EnableCruiserPopup = value;
        }

        public bool EnableAskEnterTreeData
        {
            get
            {
                EnsureLoaded();
                return Data.EnableAskEnterTreeData;
            }
            set => Data.EnableAskEnterTreeData = value;
        }

        public IEnumerable<string> Cruisers
        {
            get
            {
                EnsureLoaded();
                return Data.Cruisers.ToArray();
            }
        }

        public double TallyButtonTrayVerticalSize
        {
            get
            {
                try
                {
                    var perf = Xamarin.Essentials.Preferences.Get(PERF_TallyButtonTrayVerticalSize, DEFAULT_TALLY_BUTTON_TRAY_VERTICAL_SIZE);
                    return perf;
                }
                catch { return 0.6; }
            }
            set
            {
                Xamarin.Essentials.Preferences.Set(PERF_TallyButtonTrayVerticalSize, value);
                RaisePropertyChanged();
            }
        }

        public double PlotTallyButtonTrayVerticalSize
        {
            get
            {
                try
                {
                    return Xamarin.Essentials.Preferences.Get(PERF_PlotTallyButtonTrayVerticalSize, DEFAULT_PLOT_TALLY_BUTTON_TRAY_VERTICAL_SIZE);
                }
                catch { return 0.6; }
            }
            set
            {
                Xamarin.Essentials.Preferences.Set(PERF_PlotTallyButtonTrayVerticalSize, value);
                RaisePropertyChanged();
            }
        }

        public void AddCruiser(string cruiser)
        {
            if (string.IsNullOrWhiteSpace(cruiser)) { return; }
            cruiser = cruiser.Trim();
            cruiser = cruiser.ToUpper();

            var cruisers = Data.Cruisers;
            if (cruisers.Contains(cruiser) == false)
            {
                Data.Cruisers.Add(cruiser);
                RaisePropertyChanged(nameof(Cruisers));
            }
        }

        public void RemoveCruiser(string cruiser)
        {
            var cruisers = Cruisers;

            Data.Cruisers.Remove(cruiser);
            RaisePropertyChanged(nameof(Cruisers));
        }

        public void EnsureLoaded()
        {
            lock (DataSyncLock)
            {
                if (Data == null)
                {
                    Refresh();
                }
            }
        }

        public void Refresh()
        {
            lock (DataSyncLock)
            {
                var propDict = App.Current.Properties;

                if (propDict.ContainsKey("TallySettings"))
                {
                    var tallySettings = JsonConvert.DeserializeObject<TallySettings>((string)propDict["TallySettings"]);
                    Data = tallySettings;
                }
                else
                {
                    Data = new TallySettings();
                }

                RaisePropertyChangedAll();
            }
        }

        public void Save()
        {
            lock (DataSyncLock)
            {
                var tallySettings = Data;
                if (tallySettings != null)
                {
                    var tallySettingsSer = JsonConvert.SerializeObject(tallySettings);

                    var propDict = App.Current.Properties;
                    propDict.SetValue("TallySettings", tallySettingsSer);
                    App.Current.SavePropertiesAsync();
                }
            }
        }

        protected void RaisePropertyChangedAll()
        {
            RaisePropertyChanged(nameof(EnableAskEnterTreeData));
            RaisePropertyChanged(nameof(EnableCruiserPopup));
            RaisePropertyChanged(nameof(Cruisers));
            RaisePropertyChanged(nameof(PlotTallyButtonTrayVerticalSize));
            RaisePropertyChanged(nameof(TallyButtonTrayVerticalSize));
        }
    }
}