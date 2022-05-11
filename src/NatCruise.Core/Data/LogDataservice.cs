using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class LogDataservice : CruiseDataserviceBase, ILogDataservice
    {
        public LogDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public LogDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<Log> GetLogs(string treeID)
        {
            return Database.Query<Log>(
                "SELECT " +
                "l.*, " +
                "(SELECT count(*) FROM LogGradeError AS lge WHERE lge.LogID = l.LogID AND IsResolved=0) AS ErrorCount " +
                "FROM Log AS l " +
                "WHERE l.TreeID = @p1;", treeID).ToArray();
        }

        public Log GetLog(string logID)
        {
            return Database.Query<Log>(
                "SELECT " +
                "l.*, " +
                "(SELECT count(*) FROM LogGradeError AS lge WHERE lge.LogID = l.LogID AND IsResolved=0) AS ErrorCount " +
                "FROM Log AS l " +
                "WHERE l.LogID = @p1;", logID).FirstOrDefault();
        }

        public Log GetLog(string treeID, int logNumber)
        {
            return Database.Query<Log>(
                "SELECT " +
                "l.*, " +
                "(SELECT count(*) FROM LogGradeError AS lge WHERE lge.LogID = l.LogID AND IsResolved=0) AS ErrorCount " +
                "FROM Log AS l " +
                "WHERE l.TreeID = @p1 AND LogNumber = @p2;", treeID, logNumber).FirstOrDefault();
        }

        public void InsertLog(Log log)
        {
            if (log is null) { throw new ArgumentNullException(nameof(log)); }

            var logID = Guid.NewGuid().ToString();

            var logNumber = Database.ExecuteScalar<int>("SELECT ifnull(max(LogNumber), 0) + 1 FROM Log WHERE TreeID = @p1", log.TreeID);

            Database.Execute2(
                "INSERT INTO Log ( " +
                    "CruiseID," +
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
                    "@CruiseID," +
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
                    CruiseID,
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

                    CreatedBy = DeviceID,
                });

            log.LogNumber = logNumber;
            log.LogID = logID;
        }

        public void UpdateLog(Log log)
        {
            if (log is null) { throw new ArgumentNullException(nameof(log)); }

            Database.Execute("UPDATE Log SET " +
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
                DeviceID,
                log.LogID);
        }

        public void DeleteLog(string logID)
        {
            Database.Execute("DELETE FROM Log WHERE LogID = @p1;", logID);
        }

        
    }
}