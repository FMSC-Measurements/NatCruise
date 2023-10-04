using CruiseDAL;
using NatCruise.Services;
using NatCruise.Wpf.Data;
using Prism.Ioc;
using System;

namespace NatCruise.Data
{
    public class WpfDataserviceProvider : DataserviceProviderBase
    {
        public WpfDataserviceProvider(CruiseDatastore_V3 database, IDeviceInfoService deviceInfoService) : base(database, deviceInfoService)
        {
        }

        public WpfDataserviceProvider(string databasePath, IDeviceInfoService deviceInfoService) : base(databasePath, deviceInfoService)
        {
        }

        public override IDataservice GetDataservice(Type type)
        {
            var ds = base.GetDataservice(type);
            if (ds != null)
            {
                return ds;
            }

            var cruiseID = CruiseID;
            var database = Database;
            var deviceID = DeviceID;

            if (type == typeof(ITemplateDataservice))
            {
                return new TemplateDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(IDesignCheckDataservice))
            {
                return new DesignCheckDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ICruisersDataservice))
            {
                return new CruisersDataservice(database, cruiseID, deviceID);
            }

            return null;
        }

        public static new void RegisterDataservices(IContainerRegistry containerRegistry)
        {
            DataserviceProviderBase.RegisterDataservices(containerRegistry);

            containerRegistry.Register<ITemplateDataservice>(x => GetDataservice<ITemplateDataservice>(x));
            containerRegistry.Register<IDesignCheckDataservice>(x => GetDataservice<IDesignCheckDataservice>(x));
            containerRegistry.Register<ICruisersDataservice>(x => GetDataservice<ICruisersDataservice>(x));
        }
    }
}