using CruiseDAL;
using System;

namespace NatCruise.Data
{
    public enum CruiseLogLevel
    { Debug, Info, Error }

    public class CruiseLogDataservice : CruiseDataserviceBase, ICruiseLogDataservice
    {
        public CruiseLogDataservice(IDataContextService dataContext) : base(dataContext)
        {
            CallingProgram = CruiseDatastore_V3.GetCallingProgram();
        }

        public CruiseLogDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
            CallingProgram = CruiseDatastore_V3.GetCallingProgram();
        }

        public CruiseLogDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
            CallingProgram = CruiseDatastore_V3.GetCallingProgram();
        }

        public string CallingProgram { get; }

        public static string CruiseLogLevelToString(CruiseLogLevel level) => level switch
        {
            CruiseLogLevel.Debug => "D",
            CruiseLogLevel.Info => "I",
            CruiseLogLevel.Error => "E",
            _ => throw new ArgumentOutOfRangeException(nameof(level), "Unexpected level value"),
        };

        public void Log(string message, CruiseLogLevel level = CruiseLogLevel.Debug, string unitID = null, string stratumID = null,
            string sgID = null,
            string plotID = null,
            string treeID = null,
            string logID = null,
            string tallyID = null,
            string fieldName = null,
            string tableName = null)
        {
            var data = new
            {
                CruiseID,
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
                CreatedBy = DeviceID,
            };

            Database.Execute2(
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
    }
}