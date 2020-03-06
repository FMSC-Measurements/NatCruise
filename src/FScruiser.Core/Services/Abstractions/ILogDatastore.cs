using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ILogDatastore
    {
        IEnumerable<Log> GetLogs(string tree_guid);

        Log GetLog(string log_guid);

        Log GetLog(string tree_guid, int logNumber);

        void InsertLog(Log log);

        void UpdateLog(Log log);

        void DeleteLog(string log_guid);

        IEnumerable<LogFieldSetup> GetLogFields(string tree_guid);

        IEnumerable<LogError> GetLogErrorsByLog(string logID);

        IEnumerable<LogError> GetLogErrorsByTree(string treeID);

    }
}
