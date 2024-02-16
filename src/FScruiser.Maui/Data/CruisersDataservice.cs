using NatCruise.Data;

namespace FScruiser.Maui.Data;

public class CruisersDataservice : ICruisersDataservice
{
    private const string CRUISERS_PROP_KEY = "cruisers";
    private const string PROMPT_CRUISERS_ON_SAMPLE_KEY = "prompt_cruisers_on_sample";
    private string[]? _cruisers;

    public string DeviceID => throw new NotSupportedException();

    public string CruiseID => throw new NotSupportedException();

    public CruisersDataservice(IPreferences preferences)
    {
        Preferences = preferences ?? Microsoft.Maui.Storage.Preferences.Default;
    }

    public IPreferences Preferences { get; }

    public bool PromptCruiserOnSample
    {
        get
        {
            return Preferences.Get(PROMPT_CRUISERS_ON_SAMPLE_KEY, false);
        }
        set
        {
            Preferences.Set(PROMPT_CRUISERS_ON_SAMPLE_KEY, value);
        }
    }

    protected void RefreshCruisers()
    {
        var props = Preferences;

        if (props.ContainsKey(CRUISERS_PROP_KEY))
        {
            var cruisersPropValue = props.Get(CRUISERS_PROP_KEY, "");
            _cruisers = cruisersPropValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            _cruisers = new string[0];
        }
    }

    protected void SaveCruisers()
    {
        var cruisers = _cruisers ?? new string[0];

        Preferences.Set(CRUISERS_PROP_KEY, string.Join(",", cruisers));
    }

    public void AddCruiser(string cruiser)
    {
        RefreshCruisers();
        _cruisers = _cruisers.Append(cruiser).Distinct().ToArray();
        SaveCruisers();
    }

    public IEnumerable<string> GetCruisers()
    {
        RefreshCruisers();
        return _cruisers;
    }

    public void RemoveCruiser(string cruiser)
    {
        RefreshCruisers();
        _cruisers = _cruisers.Where(x => x != cruiser).ToArray();
        SaveCruisers();
    }

    //public void UpdateCruiser(string oldValue, string newValue)
    //{
    //    throw new NotImplementedException();
    //}
}