using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public class LogFieldValueDataservice : CruiseDataserviceBase, ILogFieldValueDataservice
    {
        public LogFieldValueDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public LogFieldValueDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public LogFieldValueDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<LogFieldValue> GetLogFieldValues(string logID)
        {
            return Database.Query<LogFieldValue>(
@"SELECT
    l.LogID,
    lf.Field,
    (CASE lf.Field
        WHEN 'Grade' THEN l.Grade
        WHEN 'ExportGrade' THEN l.ExportGrade
        ELSE NULL END) AS ValueText,
    (CASE lf.Field
          WHEN 'BoardFootRemoved' THEN l.BoardFootRemoved
          WHEN 'BarkThickness' THEN l.BarkThickness
          WHEN 'CubicFootRemoved' THEN l.CubicFootRemoved
          WHEN 'DIBClass' THEN l.DIBClass
          WHEN 'GrossBoardFoot' THEN l.GrossBoardFoot
          WHEN 'GrossCubicFoot' THEN l.GrossCubicFoot
          WHEN 'LargeEndDiameter' THEN l.LargeEndDiameter
          WHEN 'NetBoardFoot' THEN l.NetBoardFoot
          WHEN 'NetCubicFoot' THEN l.NetCubicFoot
          WHEN 'PercentRecoverable' THEN l.PercentRecoverable
          WHEN 'SeenDefect' THEN l.SeenDefect
          WHEN 'SmallEndDiameter' THEN l.SmallEndDiameter
          ELSE NULL END) AS ValueReal,
    (CASE lf.Field 
        WHEN 'LogNumber' THEN l.LogNumber
        WHEN 'Length' THEN l.Length
        ELSE NULL END) AS ValueInt,
    ifnull(lfh.Heading, lf.DefaultHeading) AS Heading

FROM Log AS l
JOIN Tree AS t USING (TreeID)
JOIN LogFieldSetup AS lfs USING (StratumCode, CruiseID)
JOIN LogField AS lf USING (Field)
LEFT JOIN LogFieldHeading AS lfh USING (Field, CruiseID)

WHERE l.LogID = @p1
ORDER BY lfS.FieldOrder;",  logID );
        }

        public void UpdateLogFieldValue(LogFieldValue logFieldValue)
        {
            Database.Execute2(
$@"UPDATE Log
SET {logFieldValue.Field} = @Value
WHERE LogID = @LogID;", logFieldValue);
        }
    }
}
