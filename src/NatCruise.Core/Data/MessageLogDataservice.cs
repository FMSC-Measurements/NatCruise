using CruiseDAL;

namespace NatCruise.Data
{
    public class MessageLogDataservice : DataserviceBase, IMessageLogDataservice
    {
        public MessageLogDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public MessageLogDataservice(string path, string deviceID) : base(path, deviceID)
        {
        }

        public MessageLogDataservice(CruiseDatastore_V3 database, string deviceID) : base(database, deviceID)
        {
        }

        public void LogMessage(string message, string level)
        {
            Database.LogMessage(message, level);
        }
    }
}