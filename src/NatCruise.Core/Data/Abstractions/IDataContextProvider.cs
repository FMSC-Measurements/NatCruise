using NatCruise.Services;
using System;

namespace NatCruise.Data
{
    public interface IDataContextService
    {
        CruiseDAL.CruiseDatastore_V3 Database { get; set; }

        IDeviceInfoService DeviceInfoService { get; }

        ISampleSelectorDataService SampleSelectorDataService { get; }

        event EventHandler CruiseChanged;

        string CruiseID { get; set; }

        string DeviceID { get;  }
    }
}