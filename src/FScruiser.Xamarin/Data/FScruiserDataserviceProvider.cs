using CruiseDAL;
using NatCruise.Data;
using NatCruise.Services;
using Prism.Ioc;

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