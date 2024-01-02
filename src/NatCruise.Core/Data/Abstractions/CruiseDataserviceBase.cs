using CruiseDAL;

namespace NatCruise.Data
{
    public class CruiseDataserviceBase : DataserviceBase
    {
        public CruiseDataserviceBase(IDataContextService dataContext) : base(dataContext)
        {
        }

        public CruiseDataserviceBase(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, deviceID)
        {
            CruiseID = cruiseID;
        }

        public CruiseDataserviceBase(string path, string cruiseID, string deviceID) : base(path, deviceID)
        {
            CruiseID = cruiseID;
        }
    }
}