using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Data;
using Prism.Ioc;
using System;

namespace FScruiser.XF.Data
{
    public class FScruiserDataserviceProvider : DataserviceProviderBase
    {
        public FScruiserDataserviceProvider(string databasePath, IDeviceInfoService deviceInfo) : base(databasePath, deviceInfo)
        {
        }

        public FScruiserDataserviceProvider(CruiseDatastore_V3 datastore, IDeviceInfoService deviceInfo) : base(datastore, deviceInfo)
        {
        }

        public static void RegisterDataservices(IContainerRegistry containerRegistry)
        {
            DataserviceProviderBase.RegisterDataservices(containerRegistry);
        }
    }
}