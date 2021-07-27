using CruiseDAL;
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

            if (typeof(ISaleDataservice).IsAssignableFrom(type))
            { return new SaleDataservice(database, cruiseID, deviceID); }

            if (cruiseID == null)
            { throw new InvalidOperationException("DataserviceProvider: no cruise selected"); }

            if (typeof(ICuttingUnitDataservice).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(database, cruiseID, deviceID); }

            if (typeof(IPlotDataservice).IsAssignableFrom(type))
            { return new PlotDataservice(database, cruiseID, deviceID); }

            if (typeof(ITreeDataservice).IsAssignableFrom(type))
            { return new TreeDataservice(database, cruiseID, deviceID); }

            if (typeof(ILogDataservice).IsAssignableFrom(type))
            { return new LogDataservice(database, cruiseID, deviceID); }

            if (typeof(IPlotTallyDataservice).IsAssignableFrom(type))
            { return new PlotTallyDataservice(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(ISampleSelectorDataService).IsAssignableFrom(type))
            { return SampleSelectorDataService; }

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
            containerRegistry.Register<ICuttingUnitDataservice>(x => GetDataservice<ICuttingUnitDataservice>());
            containerRegistry.Register<IPlotDataservice>(x => GetDataservice<IPlotDataservice>());
            containerRegistry.Register<ITreeDataservice>(x => GetDataservice<ITreeDataservice>());
            containerRegistry.Register<ILogDataservice>(x => GetDataservice<ILogDataservice>());
            containerRegistry.Register<ISampleSelectorDataService>(x => GetDataservice<ISampleSelectorDataService>());
            containerRegistry.Register<ISaleDataservice>(x => GetDataservice<ISaleDataservice>());
            containerRegistry.Register<IFixCNTDataservice>(x => GetDataservice<IFixCNTDataservice>());
            containerRegistry.Register<ITallyDataservice>(x => GetDataservice<ITallyDataservice>());
            containerRegistry.Register<IPlotTallyDataservice>(e => GetDataservice<IPlotTallyDataservice>());
            containerRegistry.Register<ITallyPopulationDataservice>(x => GetDataservice<ITallyPopulationDataservice>());
            containerRegistry.Register<ISampleInfoDataservice>(x => GetDataservice<ISampleInfoDataservice>());
        }
    }
}