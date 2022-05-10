using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Cruise.Services;
using NatCruise.Data;
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
            var ds = base.GetDataservice(type);
            if (ds != null)
            {
                return ds;
            }

            var cruiseID = CruiseID;
            var database = Database;
            var deviceID = DeviceID;

            if (typeof(ISaleDataservice).IsAssignableFrom(type))
            { return new SaleDataservice(database, cruiseID, deviceID); }

            if (cruiseID == null)
            { throw new InvalidOperationException("DataserviceProvider: no cruise selected"); }

            if (typeof(IPlotDataservice).IsAssignableFrom(type))
            { return new PlotDataservice(database, cruiseID, deviceID); }

            if (typeof(IPlotTreeDataservice).IsAssignableFrom(type))
            { return new PlotTreeDataservice(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(ISampleSelectorDataService).IsAssignableFrom(type))
            { return SampleSelectorDataService; }

            if (typeof(IFixCNTDataservice).IsAssignableFrom(type))
            { return new FixCNTDataservice(database, cruiseID, deviceID); }

            if (typeof(ITallyDataservice).IsAssignableFrom(type))
            { return new TallyDataservice(database, cruiseID, deviceID, GetDataservice<ISampleInfoDataservice>()); }

            if (typeof(ISampleInfoDataservice).IsAssignableFrom(type))
            { return new SamplerInfoDataservice(database, cruiseID, deviceID); }

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
            base.RegisterDataservices(containerRegistry);

            containerRegistry.Register<IPlotDataservice>(x => GetDataservice<IPlotDataservice>());
            containerRegistry.Register<IPlotTreeDataservice>(e => GetDataservice<IPlotTreeDataservice>());
            containerRegistry.Register<ISampleSelectorDataService>(x => GetDataservice<ISampleSelectorDataService>());
            containerRegistry.Register<IFixCNTDataservice>(x => GetDataservice<IFixCNTDataservice>());
            containerRegistry.Register<ITallyDataservice>(x => GetDataservice<ITallyDataservice>());
            containerRegistry.Register<ITallyPopulationDataservice>(x => GetDataservice<ITallyPopulationDataservice>());
            containerRegistry.Register<ISampleInfoDataservice>(x => GetDataservice<ISampleInfoDataservice>());
        }
    }
}