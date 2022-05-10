using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Data.Abstractions;
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
            var cruiseID = CruiseID;
            var database = Database;
            var deviceID = DeviceID;


            try
            {
                //if(type == typeof(IRecentFilesDataservice))
                //{
                //    return new RecentFilesDataservice();
                //}
                if (type == typeof(ISaleDataservice))
                {
                    return new SaleDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ICuttingUnitDataservice))
                {
                    return new CuttingUnitDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ISampleGroupDataservice))
                {
                    return new SampleGroupDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ISpeciesCodeDataservice))
                {
                    return new SpeciesCodeDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(IStratumDataservice))
                {
                    return new StratumDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ISubpopulationDataservice))
                {
                    return new SubpopulationDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ITallyPopulationDataservice))
                {
                }
                else if (type == typeof(ITallySettingsDataservice))
                {
                }
                else if (type == typeof(ITreeFieldDataservice))
                {
                }
                else if (type == typeof(ITemplateDataservice))
                {
                    return new TemplateDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(IFieldSetupDataservice))
                {
                    return new FieldSetupDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(IDesignCheckDataservice))
                {
                    return new DesignCheckDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(IMessageLogDataservice))
                {
                    return new MessageLogDataservice(database, deviceID);
                }
                else if (type == typeof(ITreeDataservice))
                {
                    return new TreeDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ITreeErrorDataservice))
                {
                    return new TreeErrorDataservice(database, cruiseID, deviceID);
                }
                else if (type == typeof(ITreeFieldValueDataservice))
                {
                    return new TreeFieldValueDataservice(database, cruiseID, deviceID);
                }

                return null;
            }
            catch (Exception e)
            {
                throw;
                //return null;
            }
        }

        public override void RegisterDataservices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISaleDataservice>(x => GetDataservice<ISaleDataservice>());
            containerRegistry.Register<ICuttingUnitDataservice>(x => GetDataservice<ICuttingUnitDataservice>());
            containerRegistry.Register<ISampleGroupDataservice>(x => GetDataservice<ISampleGroupDataservice>());
            containerRegistry.Register<ISpeciesCodeDataservice>(x => GetDataservice<ISpeciesCodeDataservice>());
            containerRegistry.Register<IStratumDataservice>(x => GetDataservice<IStratumDataservice>());
            containerRegistry.Register<ISubpopulationDataservice>(x => GetDataservice<ISubpopulationDataservice>());
            containerRegistry.Register<ITemplateDataservice>(x => GetDataservice<ITemplateDataservice>());
            containerRegistry.Register<IFieldSetupDataservice>(x => GetDataservice<IFieldSetupDataservice>());
            containerRegistry.Register<IDesignCheckDataservice>(x => GetDataservice<IDesignCheckDataservice>());
            containerRegistry.Register<IMessageLogDataservice>(x => GetDataservice<IMessageLogDataservice>());
            //containerRegistry.Register<ISetupInfoDataservice>(x => GetDataservice<ISetupInfoDataservice>());
            containerRegistry.Register<ITreeDataservice>(x => GetDataservice<ITreeDataservice>());
            containerRegistry.Register<ITreeErrorDataservice>(x => GetDataservice<ITreeErrorDataservice>());
            containerRegistry.Register<ITreeFieldDataservice>(x => GetDataservice<ITreeFieldDataservice>());
            containerRegistry.Register<ITreeFieldValueDataservice>(x => GetDataservice<ITreeFieldValueDataservice>()); 

        }
    }
}