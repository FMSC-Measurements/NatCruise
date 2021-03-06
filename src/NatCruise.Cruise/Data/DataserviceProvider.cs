﻿using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using Prism.Ioc;
using System;

namespace NatCruise.Cruise.Data
{
    public class DataserviceProvider : DataserviceProviderBase
    {
        public DataserviceProvider(string databasePath, IDeviceInfoService deviceInfo) : base(databasePath, deviceInfo)
        {
        }

        public DataserviceProvider(CruiseDatastore_V3 datastore, IDeviceInfoService deviceInfo) : base(datastore, deviceInfo)
        {
        }

        public ISampleSelectorDataService SampleSelectorDataService { get; set; }

        protected override void OnCruiseIDChanged(string value)
        {
            base.OnCruiseIDChanged(value);
            if (value != null)
            {
                SampleSelectorDataService = new SampleSelectorRepository(GetDataservice<ISampleInfoDataservice>());
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
            var deviceID = DeviceID;

            // all dataservices below should return null if cruiseDatastore is null
            // note: I am skeptical about wheather this method should return null at all
            //      I think it should throw if the dataservice type can't be found
            //      I almost think it should throw if the datastore is null, but I also think it should be
            //      on the classes that relie on the dataservice provider to determin if the dataservice they requested
            //      is esential and throw if null, or allow for null to be returned and check for it.

            if (typeof(ICuttingUnitDatastore).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(IPlotDatastore).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(ITreeDatastore).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(ILogDatastore).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(ICuttingUnitDatastore).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(ISampleSelectorDataService).IsAssignableFrom(type))
            { return SampleSelectorDataService; }

            if (typeof(ISaleDataservice).IsAssignableFrom(type))
            { return new SaleDataservice(database, cruiseID, deviceID); }

            if (typeof(IFixCNTDataservice).IsAssignableFrom(type))
            { return new FixCNTDataservice(database, cruiseID, deviceID); }

            if (typeof(ITallyDataservice).IsAssignableFrom(type))
            { return new TallyDataservice(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if(typeof(ISampleInfoDataservice).IsAssignableFrom(type))
            {
                return new SamplerInfoDataservice(database, cruiseID, deviceID);
            }

            if (typeof(ITallyPopulationDataservice).IsAssignableFrom(type))
            { return new TallyPopulationDataservice(database, cruiseID, deviceID); }

            if (typeof(ISampleInfoDataservice).IsAssignableFrom(type))
            { return new SamplerInfoDataservice(database, cruiseID, deviceID); }
            else
            {
                throw new InvalidOperationException("no dataservice found for type " + type.FullName);

                //return null;
            }
        }

        public override void RegisterDataservices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ICuttingUnitDatastore>(x => GetDataservice<ICuttingUnitDatastore>());
            containerRegistry.Register<IPlotDatastore>(x => GetDataservice<IPlotDatastore>());
            containerRegistry.Register<ITreeDatastore>(x => GetDataservice<ITreeDatastore>());
            containerRegistry.Register<ILogDatastore>(x => GetDataservice<ILogDatastore>());
            containerRegistry.Register<ISampleSelectorDataService>(x => GetDataservice<ISampleSelectorDataService>());
            containerRegistry.Register<ISaleDataservice>(x => GetDataservice<ISaleDataservice>());
            containerRegistry.Register<IFixCNTDataservice>(x => GetDataservice<IFixCNTDataservice>());
            containerRegistry.Register<ITallyDataservice>(x => GetDataservice<ITallyDataservice>());
            containerRegistry.Register<ITallyPopulationDataservice>(x => GetDataservice<ITallyPopulationDataservice>());
            containerRegistry.Register<ISampleInfoDataservice>(x => GetDataservice<ISampleInfoDataservice>());
        }
    }
}