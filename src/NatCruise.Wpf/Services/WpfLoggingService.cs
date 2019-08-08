using NatCruise.Wpf.Util;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NatCruise.Wpf.Services
{
    public class WpfLoggingService : LoggingService
    {
        public override void LogException(string catigory, string message, Exception ex, IDictionary<string, string> data = null)
        {
            Debug.WriteLine($"Error:::{catigory}::::{message}::::{ex.Message}::::");
            Debug.WriteLine(ex.StackTrace);

            if (data == null) { data = new Dictionary<string, string>(); }

            data.SetValue("error_catigory", catigory);
            data.SetValue("error_message", message);

            Crashes.TrackError(ex, data);
        }

        public override void LogEvent(string name, IDictionary<string, string> data = null)
        {
            Debug.WriteLine($"Event:::{name}::::");

            foreach(var item in data)
            {
                Debug.WriteLine($"Eventdata:::{item.Key}::::{item.Value.ToString()}::::");
            }

            Analytics.TrackEvent(name, data);
        }
    }
}