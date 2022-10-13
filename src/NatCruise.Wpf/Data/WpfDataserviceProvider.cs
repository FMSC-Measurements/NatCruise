using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Design.Data;
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


            try
            {
                if (type == typeof(ISpeciesCodeDataservice))
                {
                    return new SpeciesCodeDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ITallySettingsDataservice))
                {
                }
                else if (type == typeof(ITemplateDataservice))
                {
                    return new TemplateDataservice(database, cruiseID, deviceID);
                }
                
                else if (type == typeof(IDesignCheckDataservice))
                {
                    return new DesignCheckDataservice(database, cruiseID, deviceID);
                }
                

                return null;
            }
            catch (Exception e)
            {
                throw;
                //return null;
            }
        }

        public static void RegisterDataservices(IContainerRegistry containerRegistry)
        {
            DataserviceProviderBase.RegisterDataservices(containerRegistry);

            containerRegistry.Register<ISpeciesCodeDataservice>(x => GetDataservice<ISpeciesCodeDataservice>(x));
            containerRegistry.Register<ITemplateDataservice>(x => GetDataservice<ITemplateDataservice>(x));
            containerRegistry.Register<IDesignCheckDataservice>(x => GetDataservice<IDesignCheckDataservice>(x));
        }
    }
}