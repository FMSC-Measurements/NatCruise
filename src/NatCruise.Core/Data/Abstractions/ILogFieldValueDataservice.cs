using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ILogFieldValueDataservice
    {
        IEnumerable<LogFieldValue> GetLogFieldValues(string logID);

        void UpdateLogFieldValue(LogFieldValue logFieldValue);
    }
}