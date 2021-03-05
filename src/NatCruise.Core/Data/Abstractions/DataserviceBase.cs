using CruiseDAL;
using CruiseDAL.Schema;
using System;
using System.Linq;

namespace NatCruise.Data
{
    public abstract class DataserviceBase : IDataservice
    {
        protected readonly string PLOT_METHODS = String.Join(", ", CruiseMethods.PLOT_METHODS
            .Append(CruiseMethods.THREEPPNT)
            .Append(CruiseMethods.FIXCNT)
            .Select(x => "'" + x + "'").ToArray());

        public string DeviceID { get; protected set; }

        public CruiseDatastore_V3 Database { get; }

        public DataserviceBase(string path, string deviceID)
        {
            if (string.IsNullOrWhiteSpace(deviceID)) { throw new ArgumentException($"'{nameof(deviceID)}' cannot be null or whitespace", nameof(deviceID)); }
            DeviceID = deviceID;

            var database = new CruiseDatastore_V3(path ?? throw new ArgumentNullException(nameof(path)));
            Database = database;
        }

        public DataserviceBase(CruiseDatastore_V3 database, string deviceID)
        {
            if (string.IsNullOrWhiteSpace(deviceID)) { throw new ArgumentException($"'{nameof(deviceID)}' cannot be null or whitespace", nameof(deviceID)); }
            DeviceID = deviceID;

            Database = database ?? throw new ArgumentNullException(nameof(database));
        }
    }
}