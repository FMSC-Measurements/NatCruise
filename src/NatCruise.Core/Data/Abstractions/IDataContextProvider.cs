using NatCruise.Services;

namespace NatCruise.Data
{
    public interface IDataContextService
    {
        CruiseDAL.CruiseDatastore_V3 Database { get; set; }

        IDeviceInfoService DeviceInfoService { get; }

        ISampleSelectorDataService SampleSelectorDataService { get; }

        string CruiseID { get; set; }

        string DeviceID { get;  }
    }
}