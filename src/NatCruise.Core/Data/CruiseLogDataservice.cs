using CruiseDAL;
using System;

namespace NatCruise.Data
{
    public enum CruiseLogLevel
    { Debug, Info, Error }

    public class CruiseLogDataservice : CruiseDataserviceBase, ICruiseLogDataservice
    {
        private static string _callingProgram;

        public CruiseLogDataservice(IDataContextService dataContext) : base(dataContext)
        {
            
        }

        public CruiseLogDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public CruiseLogDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public static string CallingProgram  => _callingProgram ??= CruiseDatastore_V3.GetCallingProgram();



        public void Log(string message, CruiseLogLevel level = CruiseLogLevel.Debug, string unitID = null, string stratumID = null,
            string sgID = null,
            string plotID = null,
            string treeID = null,
            string logID = null,
            string tallyID = null,
            string fieldName = null,
            string tableName = null)
        {
            Log(this, message, level, unitID, stratumID, sgID, plotID, treeID, logID, tallyID, fieldName, tableName);
        }

        public static void Log(CruiseDataserviceBase dataService, string message, CruiseLogLevel level = CruiseLogLevel.Debug, string unitID = null, string stratumID = null,
            string sgID = null,
            string plotID = null,
            string treeID = null,
            string logID = null,
            string tallyID = null,
            string fieldName = null,
            string tableName = null)
        {
            var cruiseID = dataService.CruiseID;
            var deviceID = dataService.DeviceID;
            var database = dataService.Database;

            var data = new
            {
                cruiseID,
                Message = message,
                Level = CruiseLogLevelToString(level),
                Program = CallingProgram,

                CuttingUnitID = unitID,
                StratumID = stratumID,
                SampleGroupID = sgID,
                PlotID = plotID,
                TreeID = treeID,
                LogID = logID,
                TallyLedgerID = tallyID,

                Field = fieldName,
                TableName = tableName,
                CreatedBy = deviceID,
            };

            database.Execute2(
@"INSERT INTO CruiseLog (
    CruiseID,
    Message,
    Level,
    Program,

    CuttingUnitID,
    StratumID,
    SampleGroupID,
    PlotID,
    TreeID,
    LogID,
    TallyLedgerID,

    Field,
    TableName,
    CreatedBy
) VALUES (
    @CruiseID,
    @Message,
    @Level,
    @Program,

    @CuttingUnitID,
    @StratumID,
    @SampleGroupID,
    @PlotID,
    @TreeID,
    @LogID,
    @TallyLedgerID,

    @Field,
    @TableName,
    @CreatedBy
);", data);
        }

        public static string CruiseLogLevelToString(CruiseLogLevel level) => level switch
        {
            CruiseLogLevel.Debug => "D",
            CruiseLogLevel.Info => "I",
            CruiseLogLevel.Error => "E",
            _ => throw new ArgumentOutOfRangeException(nameof(level), "Unexpected level value"),
        };
    }
}