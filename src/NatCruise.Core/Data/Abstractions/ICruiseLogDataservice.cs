namespace NatCruise.Data
{
    public interface ICruiseLogDataservice : IDataservice
    {
        void Log(string message,
            CruiseLogLevel level = CruiseLogLevel.Debug,
            string unitID = null,
            string stratumID = null,
            string sgID = null,
            string plotID = null,
            string treeID = null,
            string logID = null,
            string tallyID = null,
            string fieldName = null,
            string tableName = null);
    }
}