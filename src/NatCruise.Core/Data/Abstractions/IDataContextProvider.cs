using NatCruise.Services;
using System;

namespace NatCruise.Data
{
    public interface IDataContextService : IDisposable
    {
        CruiseDAL.CruiseDatastore_V3 Database { get; set; }

        string DatabasePath { get; }

        bool IsReady { get; }

        Exception InitError { get; }

        IDeviceInfoService DeviceInfoService { get; }

        ISampleSelectorDataService SampleSelectorDataService { get; }

        event EventHandler CruiseChanged;

        string CruiseID { get; set; }

        string DeviceID { get;  }

        bool OpenOrCreateDatabase(string path);
    }
}