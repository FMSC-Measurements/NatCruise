using CruiseDAL;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using System;

namespace NatCruise.Cruise.Data
{
    public class DataserviceProvider : DataserviceProviderBase
    {
        public DataserviceProvider(IDeviceInfoService deviceInfo, string databasePath) : base(databasePath)
        {
            DeviceInfoService = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
        }

        public DataserviceProvider(IDeviceInfoService deviceInfo, CruiseDatastore_V3 datastore) : base(datastore)
        {
            DeviceInfoService = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
        }

        protected IDeviceInfoService DeviceInfoService { get; }

        public ISampleSelectorDataService SampleSelectorDataService { get; set; }

        protected override void OnCruiseIDChanged(string value)
        {
            base.OnCruiseIDChanged(value);
            if (value != null)
            {
                SampleSelectorDataService = new SampleSelectorRepository((ISampleInfoDataservice)GetDataservice(typeof(ISampleInfoDataservice)));
            }
            else
            {
                SampleSelectorDataService = null;
            }
        }

        public override IDataservice GetDataservice(Type type)
        {
            var cruiseID = CruiseID;
            var database = Database;

            // all dataservices below should return null if cruiseDatastore is null
            // note: I am skeptical about wheather this method should return null at all
            //      I think it should throw if the dataservice type can't be found
            //      I almost think it should throw if the datastore is null, but I also think it should be
            //      on the classes that relie on the dataservice provider to determin if the dataservice they requested
            //      is esential and throw if null, or allow for null to be returned and check for it.

            if (typeof(ICuttingUnitDatastore).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(database, cruiseID); }

            if (typeof(ISampleSelectorDataService).IsAssignableFrom(type))
            { return SampleSelectorDataService; }

            if (typeof(ISaleDataservice).IsAssignableFrom(type))
            { return new SaleDataservice(database, cruiseID); }

            if (typeof(IFixCNTDataservice).IsAssignableFrom(type))
            { return new FixCNTDataservice(database, cruiseID); }

            if (typeof(ITallyDataservice).IsAssignableFrom(type)
                || typeof(ISampleInfoDataservice).IsAssignableFrom(type))
            { return new TallyDataservice(database, cruiseID, DeviceInfoService); }

            if (typeof(ITallyPopulationDataservice).IsAssignableFrom(type))
            { return new TallyPopulationDataservice(database, cruiseID); }

            if (typeof(ISampleInfoDataservice).IsAssignableFrom(type))
            { return new SamplerInfoDataservice(database, cruiseID, DeviceInfoService); }
            else
            {
                throw new InvalidOperationException("no dataservice found for type " + type.FullName);

                //return null;
            }
        }
    }
}