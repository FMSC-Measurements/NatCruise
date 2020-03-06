namespace FScruiser.XF.Services
{
    public interface IApplicationSettings
    {
        bool EnableCrashReports { get; set; }

        bool EnableAnalitics { get; set; }

        void Save();
    }

    public class ApplicationSettings : IApplicationSettings
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
            //Application.SavePropertiesAsync();
        }

        //protected Tvalue GetPropValueOrDefault<Tvalue>(string key, Tvalue defaultValue)
        //{
        //    var propDict = Application.Properties;

        //    if (propDict.ContainsKey(key))
        //    {
        //        return (Tvalue)propDict[key];
        //    }
        //    else
        //    {
        //        return defaultValue;
        //    }
        //}

        //protected void SetPropValue<Tvalue>(string key, Tvalue value)
        //{
        //    var propDict = Application.Properties;

        //    if (propDict.ContainsKey(key))
        //    {
        //        propDict[key] = value;
        //    }
        //    else
        //    {
        //        propDict.Add(key, value);
        //    }
        //}
    }
}