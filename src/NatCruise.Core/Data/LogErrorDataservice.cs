using CruiseDAL;
using NatCruise.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class LogErrorDataservice : CruiseDataserviceBase, ILogErrorDataservice
    {
        public LogErrorDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public LogErrorDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<LogError> GetLogErrorsByLog(string logID)
        {
            return Database.Query<LogError>(
@"SELECT
    lge.LogID,
    l.LogNumber,
    lge.Message
FROM LogGradeError AS lge
JOIN Log AS l USING (LogID)
WHERE lge.LogID = @p1;",
                new object[] { logID })
                .ToArray();
        }

        public IEnumerable<LogError> GetLogErrorsByTree(string treeID)
        {
            return Database.Query<LogError>(
@"SELECT
    LogID,
    l.LogNumber,
    Message
FROM LogGradeError
JOIN Log AS l USING (LogID)
WHERE l.TreeID  = @p1;",
                new object[] { treeID })
                .ToArray();
        }
    }
}