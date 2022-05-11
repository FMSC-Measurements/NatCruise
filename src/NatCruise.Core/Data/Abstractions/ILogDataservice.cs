using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ILogDataservice : IDataservice
    {
        IEnumerable<Log> GetLogs(string tree_guid);

        Log GetLog(string log_guid);

        Log GetLog(string tree_guid, int logNumber);

        void InsertLog(Log log);

        void UpdateLog(Log log);

        void DeleteLog(string log_guid);
    }
}