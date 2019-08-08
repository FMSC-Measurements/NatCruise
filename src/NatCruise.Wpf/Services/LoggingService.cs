using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public abstract class LoggingService : ILoggingService
    {
        public abstract void LogEvent(string name, IDictionary<string, string> data = null);

        public abstract void LogException(string catigory, string message, Exception ex, IDictionary<string, string> data = null);
    }
}
