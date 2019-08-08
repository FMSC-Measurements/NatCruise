using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface ITallySettingsDataservice
    {
        string GetHotKey(string stratumCode, string sampleGroupCode);

        string GetHotKey(string stratumCode, string sampleGroupCode, string species, string livedead);

        void SetHotKey(string hotkey, string stratumCode, string sampleGroupCode, string species = null, string livedead = null);

        IEnumerable<string> GetUsedHotKeys(string stratumCode);

        string GetDescription(string stratumCode, string sampleGroupCode, string species = null, string livedead = null);

        void SetDescription(string description, string stratumCode, string sampleGroupCode, string species = null, string livedead = null);
    }
}