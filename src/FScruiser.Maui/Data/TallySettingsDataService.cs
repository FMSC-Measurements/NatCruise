using NatCruise.Models;
using NatCruise.Util;
using Newtonsoft.Json;
using Prism.Mvvm;
using System.Collections.Generic;

namespace FScruiser.Maui.Data;

public class TallySettings
{
    public bool EnableCruiserPopup { get; set; }

    public bool EnableAskEnterTreeData { get; set; }

    public List<string> Cruisers { get; set; } = new List<string>();
}

public class TallySettingsDataService : BindableBase, ITallySettingsDataService
{
    private const string PERF_TallyButtonTrayVerticalSize = "TallyButtonTrayVerticalSize";
    private const string PERF_PlotTallyButtonTrayVerticalSize = "PlotTallyButtonTrayVerticalSize";

    private const double DEFAULT_TALLY_BUTTON_TRAY_VERTICAL_SIZE = 0.4;
    private const double DEFAULT_PLOT_TALLY_BUTTON_TRAY_VERTICAL_SIZE = 0.4;

    protected IPreferences Preferences { get; }

    public TallySettingsDataService(IPreferences preferences)
    {
        Preferences = preferences;
    }

    public TallySettingsDataService()
    {
        Preferences =  Microsoft.Maui.Storage.Preferences.Default;
    }

    protected TallySettings? Data { get; set; }
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
                var perf = Preferences.Get(PERF_TallyButtonTrayVerticalSize, DEFAULT_TALLY_BUTTON_TRAY_VERTICAL_SIZE);
                return perf;
            }
            catch { return 0.6; }
        }
        set
        {
            Preferences.Set(PERF_TallyButtonTrayVerticalSize, value);
            RaisePropertyChanged();
        }
    }

    public double PlotTallyButtonTrayVerticalSize
    {
        get
        {
            try
            {
                return Preferences.Get(PERF_PlotTallyButtonTrayVerticalSize, DEFAULT_PLOT_TALLY_BUTTON_TRAY_VERTICAL_SIZE);
            }
            catch { return 0.6; }
        }
        set
        {
            Preferences.Set(PERF_PlotTallyButtonTrayVerticalSize, value);
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
            var tallySetting = Preferences.Get("TallySettings", "");

            if (tallySetting != "")
            {
                var tallySettings = JsonConvert.DeserializeObject<TallySettings>(tallySetting);
                Data = tallySettings ?? new TallySettings();
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

                Preferences.Set("TallySettings", tallySettingsSer);
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