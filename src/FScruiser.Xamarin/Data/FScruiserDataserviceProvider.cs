using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Services;
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

        public ISampleSelectorDataService SampleSelectorDataService { get; set; }

        protected override void OnCruiseIDChanged(string value)
        {
            base.OnCruiseIDChanged(value);
            if (value != null)
            {
                SampleSelectorDataService = new SampleSelectorRepository(
                    GetDataservice<ISamplerStateDataservice>(),
                    GetDataservice<ISampleGroupDataservice>());
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

            if (typeof(ISampleSelectorDataService).IsAssignableFrom(type))
            { return SampleSelectorDataService; }

            if (typeof(IFixCNTDataservice).IsAssignableFrom(type))
            { return new FixCNTDataservice(database, cruiseID, deviceID); }

            if (typeof(ITallyDataservice).IsAssignableFrom(type))
            { return new TallyDataservice(database, cruiseID, deviceID, GetDataservice<ISamplerStateDataservice>()); }

            else
            {
                throw new InvalidOperationException("no dataservice found for type " + type.FullName);

                //return null;
            }
        }

        public static void RegisterDataservices(IContainerRegistry containerRegistry)
        {
            DataserviceProviderBase.RegisterDataservices(containerRegistry);

            containerRegistry.Register<ISampleSelectorDataService>(x => GetDataservice<ISampleSelectorDataService>(x));
            containerRegistry.Register<IFixCNTDataservice>(x => GetDataservice<IFixCNTDataservice>(x));
            containerRegistry.Register<ITallyDataservice>(x => GetDataservice<ITallyDataservice>(x));
        }



    }
}