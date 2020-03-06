using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public partial class CuttingUnitDatastore
    {
        public IEnumerable<Log> GetLogs(string treeID)
        {
            return Database.Query<Log>(
                "SELECT " +
                "l.*, " +
                "(SELECT count(*) FROM LogGradeError AS lge WHERE lge.LogID = l.LogID AND IsResolved=0) AS ErrorCount " +
                "FROM Log_v3 AS l " +
                "WHERE l.TreeID = @p1;", treeID).ToArray();
        }

        public Log GetLog(string logID)
        {
            return Database.Query<Log>(
                "SELECT " +
                "l.*, " +
                "(SELECT count(*) FROM LogGradeError AS lge WHERE lge.LogID = l.LogID AND IsResolved=0) AS ErrorCount " +
                "FROM Log_v3 AS l " +
                "WHERE l.LogID = @p1;", logID).FirstOrDefault();
        }

        public Log GetLog(string treeID, int logNumber)
        {
            return Database.Query<Log>(
                "SELECT " +
                "l.*, " +
                "(SELECT count(*) FROM LogGradeError AS lge WHERE lge.LogID = l.LogID AND IsResolved=0) AS ErrorCount " +
                "FROM Log_v3 AS l " +
                "WHERE l.TreeID = @p1 AND LogNumber = @p2;", treeID, logNumber).FirstOrDefault();
        }

        public void InsertLog(Log log)
        {
            var logID = Guid.NewGuid().ToString();

            var logNumber = Database.ExecuteScalar<int>("SELECT ifnull(max(LogNumber), 0) + 1 FROM Log_V3 WHERE TreeID = @p1", log.TreeID);

            Database.Execute2(
                "INSERT INTO Log_V3 ( " +
                    "LogID , " +
                    "TreeID, " +
                    "LogNumber, " +

                    "Grade, " +
                    "SeenDefect, " +
                    "PercentRecoverable, " +
                    "Length, " +
                    "ExportGrade, " +

                    "SmallEndDiameter, " +
                    "LargeEndDiameter, " +
                    "GrossBoardFoot, " +
                    "NetBoardFoot, " +
                    "GrossCubicFoot, " +

                    "NetCubicFoot, " +
                    "BoardFootRemoved, " +
                    "CubicFootRemoved, " +
                    "DIBClass, " +
                    "BarkThickness, " +

                    "CreatedBy " +
                ") VALUES ( " +
                    "@LogID, " +
                    "@TreeID, " +
                    "@LogNumber, " +

                    "@Grade, " +
                    "@SeenDefect, " +
                    "@PercentRecoverable, " +
                    "@Length," +
                    "@ExportGrade, " +

                    "@SmallEndDiameter, " +
                    "@LargeEndDiameter, " +
                    "@GrossBoardFoot, " +
                    "@NetBoardFoot, " +
                    "@GrossCubicFoot, " +

                    "@NetCubicFoot, " +
                    "@BoardFootRemoved, " +
                    "@CubicFootRemoved, " +
                    "@DIBClass, " +
                    "@BarkThickness, " +

                    "@CreatedBy" +
                ");",
                new
                {
                    LogID = logID,
                    log.TreeID,
                    LogNumber = logNumber,

                    log.Grade,
                    log.SeenDefect,
                    log.PercentRecoverable,
                    log.Length,
                    log.ExportGrade,

                    log.SmallEndDiameter,
                    log.LargeEndDiameter,
                    log.GrossBoardFoot,
                    log.NetBoardFoot,
                    log.GrossCubicFoot,

                    log.NetCubicFoot,
                    log.BoardFootRemoved,
                    log.CubicFootRemoved,
                    log.DIBClass,
                    log.BarkThickness,

                    CreatedBy = UserName,
                });

            log.LogNumber = logNumber;
            log.LogID = logID;
        }

        public void UpdateLog(Log log)
        {
            Database.Execute("UPDATE Log_V3 SET " +
                "LogNumber = @p1, " +
                "Grade = @p2, " +
                "SeenDefect = @p3, " +
                "PercentRecoverable = @p4, " +
                "Length = @p5, " +
                "ExportGrade = @p6, " +
                "SmallEndDiameter = @p7, " +
                "LargeEndDiameter = @p8, " +
                "GrossBoardFoot = @p9, " +
                "NetBoardFoot = @p10, " +
                "GrossCubicFoot = @p11, " +
                "NetCubicFoot = @p12, " +
                "BoardFootRemoved = @p13, " +
                "CubicFootRemoved = @p14, " +
                "DIBClass = @p15, " +
                "BarkThickness = @p16, " +
                "ModifiedBy = @p17 " +
                "WHERE LogID = @p18;",
                log.LogNumber,
                log.Grade,
                log.SeenDefect,
                log.PercentRecoverable,
                log.Length,
                log.ExportGrade,
                log.SmallEndDiameter,
                log.LargeEndDiameter,
                log.GrossBoardFoot,
                log.NetBoardFoot,
                log.GrossCubicFoot,
                log.NetCubicFoot,
                log.BoardFootRemoved,
                log.CubicFootRemoved,
                log.DIBClass,
                log.BarkThickness,
                UserName,
                log.LogID);
        }

        public void DeleteLog(string logID)
        {
            Database.Execute("DELETE FROM Log_V3 WHERE LogID = @p1;", logID);
        }

        private static readonly LogFieldSetup[] DEFAULT_LOG_FIELDS = new LogFieldSetup[]{
            new LogFieldSetup(){
                Field = nameof(Log.LogNumber), Heading = "LogNum"},
            new LogFieldSetup(){
                Field = nameof(Log.Grade), Heading = "Grade"},
            new LogFieldSetup() {
                Field = nameof(Log.SeenDefect), Heading = "PctSeenDef"}
        };

        public IEnumerable<LogFieldSetup> GetLogFields(string treeID)
        {
            var fields = Database.From<LogFieldSetup>()
                .Where("StratumCode = (SELECT StratumCode FROM Tree_V3 WHERE TreeID = @p1)")
                .OrderBy("FieldOrder")
                .Query(treeID).ToArray();

            if (fields.Length == 0)
            {
                return DEFAULT_LOG_FIELDS;
            }
            else
            {
                return fields;
            }
        }

        public IEnumerable<LogError> GetLogErrorsByLog(string logID)
        {
            return Database.Query<LogError>(
                "SELECT " +
                "lge.LogID, " +
                "l.LogNumber, " +
                "lge.Message " +
                "FROM LogGradeError AS lge " +
                "JOIN Log_V3 AS l USING (LogID) " +
                "WHERE lge.LogID = @p1;",
                new object[] { logID })
                .ToArray();
        }

        public IEnumerable<LogError> GetLogErrorsByTree(string treeID)
        {
            return Database.Query<LogError>(
                "SELECT " +
                "LogID, " +
                "l.LogNumber, " +
                "Message " +
                "FROM LogGradeError " +
                "JOIN Log_V3 AS l USING (LogID) " +
                "WHERE l.TreeID  = @p1;",
                new object[] { treeID })
                .ToArray();
        }
    }
}
