using NatCruise.Services;

namespace NatCruise.Data
{
    public interface IDataContextService
    {
        CruiseDAL.CruiseDatastore_V3 Database { get; }

        IDeviceInfoService DeviceInfoService { get; }

        ISampleSelectorDataService SampleSelectorDataService { get; }

        string CruiseID { get; }

        string DeviceID { get; }
    }
}