using CruiseDAL;

namespace NatCruise.Data
{
    public class CruiseDataserviceBase : DataserviceBase
    {
        public string CruiseID { get; }

        public CruiseDataserviceBase(CruiseDatastore_V3 database, string cruiseID) : base(database)
        {
            CruiseID = cruiseID;
        }

        public CruiseDataserviceBase(string path, string cruiseID) : base(path)
        {
            CruiseID = cruiseID;
        }
    }
}