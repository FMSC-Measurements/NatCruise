using System;
using System.Collections.Generic;
using System.Text;
using Prism.Logging;

namespace FScruiser.XF.Services
{
    public class AppCenterLoggerService : Prism.Logging.ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            System.Diagnostics.Debug.WriteLine($"{category.ToString().ToUpper()}::::{message}");

            //switch (category)
            //{
            //    case Category.Exception:
            //    case Category.Warn:
            //    case Category.Info:
            //    case Category.Debug:
            //        {
            var categoryStr = category.ToString();
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent($"Prisim_Log_{categoryStr}",
                new Dictionary<string, string> { { "message", message }, { "priority", priority.ToString() } });
            //            break;
            //        }
            //}


        }
    }
}
