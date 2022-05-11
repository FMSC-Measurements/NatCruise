using CruiseDAL;
using NatCruise.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class DeviceDataservice : CruiseDataserviceBase, IDeviceDataservice
    {
        public DeviceDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public DeviceDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<Device> GetDevices()
        {
            return Database.Query<Device>(
@"WITH ssModifiedDate AS (
SELECT max(ss.ModifiedDate) AS LastModified, DeviceID, CruiseID
FROM SamplerState AS ss
    WHERE CruiseID = @p1
GROUP BY ss.DeviceID, CruiseID )

SELECT d.*, ss.LastModified FROM DEVICE AS d
LEFT JOIN ssModifiedDate AS ss USING (DeviceID, CruiseID)
WHERE d.CruiseID = @p1;", CruiseID).ToArray();
        }

        public IEnumerable<Device> GetOtherDevices()
        {
            return Database.Query<Device>(
@"WITH ssModifiedDate AS (
SELECT max(ss.ModifiedDate) AS LastModified, DeviceID, CruiseID
FROM SamplerState AS ss
WHERE CruiseID =  @p2
GROUP BY ss.DeviceID )

SELECT d.*, ss.LastModified FROM DEVICE AS d
LEFT JOIN ssModifiedDate AS ss USING (DeviceID)
WHERE d.DeviceID != @p1 AND CruiseID = @p2;", DeviceID, CruiseID).ToArray();
        }
    }
}