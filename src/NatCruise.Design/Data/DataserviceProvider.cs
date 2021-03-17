using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Data;
using System;

namespace NatCruise.Data
{
    public class DataserviceProvider : DataserviceProviderBase
    {
        public DataserviceProvider(CruiseDatastore_V3 database, IDeviceInfoService deviceInfoService) : base(database, deviceInfoService)
        {
        }

        public DataserviceProvider(string databasePath, IDeviceInfoService deviceInfoService) : base(databasePath, deviceInfoService)
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
                if (type == typeof(ICruiseDataservice))
                {
                    return new CruiseDataservice(database, cruiseID, deviceID);
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

                return null;
            }
            catch (Exception e)
            {
                throw;
                //return null;
            }
        }
    }
}