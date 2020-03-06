using System;
using System.Collections.Generic;

namespace NatCruise.Services
{
    public interface ILoggingService
    {
        void LogException(string catigory, string message, Exception ex, IDictionary<string, string> data = null);

        void LogEvent(string name, IDictionary<string, string> data = null);
    }
}