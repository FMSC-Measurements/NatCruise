using FScruiser.Services;
using FScruiser.Util;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FScruiser.XF.Services
{
    public class TallySettings
    {
        public bool EnableCruiserPopup { get; set; }

        public bool EnableAskEnterTreeData { get; set; }

        public List<string> Cruisers { get; set; } = new List<string>();
    }

    public class TallySettingsDataService : INPC_Base, ITallySettingsDataService
    {
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
        }
    }
}