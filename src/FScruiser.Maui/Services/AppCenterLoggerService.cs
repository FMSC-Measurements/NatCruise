using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NatCruise.Services;
using NatCruise.Util;
using System.Diagnostics;

//using Prism.Logging;

namespace FScruiser.Maui.Services;

public class AppCenterLoggerService : LoggingService //, Prism.Logging.ILoggerFacade
{
    //void Prism.Logging.ILoggerFacade.Log(string message, Category category, Priority priority)
    //{
    //    var categoryStr = category.ToString();
    //    var data = new Dictionary<string, string>
    //    {
    //        { "message", message },
    //        { "prisim_priority", priority.ToString() }
    //    };

    //    LogEvent($"Prisim_Log_{categoryStr}", data: data);

    //}

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

        foreach (var item in data)
        {
            Debug.WriteLine($"Eventdata:::{item.Key}::::{item.Value.ToString()}::::");
        }

        Analytics.TrackEvent(name, data);
    }
}