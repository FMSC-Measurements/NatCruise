using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public interface ILoggingService
    {
        void LogException(string catigory, string message, Exception ex, IDictionary<string, string> data = null);

        void LogEvent(string name, IDictionary<string, string> data = null);
    }
}
