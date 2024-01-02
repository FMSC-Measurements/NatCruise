using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public class LogFieldDataservice : CruiseDataserviceBase, ILogFieldDataservice
    {
        public LogFieldDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public LogFieldDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public LogFieldDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        //        public IEnumerable<LogField> GetLogFieldsUsedInCruise()
        //        {
        //            return Database.Query<LogField>(
        //@"SELECT
        //    lf.Field,
        //    lfh.Heading,
        //    lf.DefaultHeading,
        //    DbType
        //FROM LogField AS lf
        //LEFT JOIN LogFieldHeading AS lfh ON lf.Field = lfh.Field AND lfh.CruiseID = @p1
        //WHERE lf.Field in (SELECT DISTINCT Field FROM LogFieldSetup WHERE CruiseID = @p1)
        //ORDER BY ifnull(lfh.Heading, lf.DefaultHeading);", CruiseID).ToArray();
        //        }

        public IEnumerable<LogField> GetLogFields()
        {
            return Database.Query<LogField>(
@"SELECT
    lf.Field,
    lfh.Heading,
    lf.DefaultHeading,
    DbType
FROM LogField AS lf
LEFT JOIN LogFieldHeading AS lfh ON lf.Field = lfh.Field AND lfh.CruiseID = @p1
ORDER BY ifnull(lfh.Heading, lf.DefaultHeading);", CruiseID).ToArray();
        }

        public void UpdateLogField(LogField lf)
        {
            if (string.IsNullOrEmpty(lf.Heading))
            {
                Database.Execute2("DELETE FROM LogFieldHeading WHERE CruiseID = @CruiseID AND Field = @Field;",
                    new { CruiseID, lf.Field });
            }
            else
            {
                Database.Execute2(
@"INSERT INTO LogFieldHeading (
    CruiseID,
    Field,
    Heading
) VALUES (
    @CruiseID,
    @Field,
    @Heading
)
ON CONFLICT (CruiseID, Field) DO
UPDATE SET Heading = @Heading
WHERE CruiseID = @CruiseID AND Field = @Field;",
                new
                {
                    CruiseID,
                    lf.Field,
                    lf.Heading,
                });
            }
        }
    }
}