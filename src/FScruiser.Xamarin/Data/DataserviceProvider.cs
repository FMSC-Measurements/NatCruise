using CruiseDAL;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using Prism.Ioc;
using System;
using System.Linq;
using Xamarin.Forms;

namespace FScruiser.XF.Data
{
    public class DataserviceProvider : DataserviceProviderBase
    {
        ICruisersDataservice _cruisersDataservice;

        public DataserviceProvider(IContainerProvider container, string databasePath) : base(databasePath)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        protected Prism.Ioc.IContainerProvider Container { get; }
        protected IDeviceInfoService DeviceInfoService => (IDeviceInfoService)Container.Resolve(typeof(IDeviceInfoService));

        public ISampleSelectorDataService SampleSelectorDataService { get; set; }

        public ICruisersDataservice CruisersDataService { get; set; }

        public CruiseDatastore_V3 CruiseDatastore { get; set; }

        //public string CruisePath
        //{
        //    get => _cruisePath;
        //    set
        //    {
        //        if (value != null)
        //        {
        //            var datastore = new CruiseDatastore_V3(value);
        //            CruiseDatastore = datastore;
        //            SampleSelectorDataService = new SampleSelectorRepository((ISampleInfoDataservice) Get(typeof(ISampleInfoDataservice), datastore));
        //        }
        //        _cruisePath = value;
        //    }
        //}

        public override void OpenDatabase(string filePath)
        {
            base.OpenDatabase(filePath);

            var datastore = new CruiseDatastore_V3(DatabasePath);
            CruiseDatastore = datastore;

            SampleSelectorDataService = new SampleSelectorRepository((ISampleInfoDataservice)GetDataservice(typeof(ISampleInfoDataservice), datastore));
        }

        public override IDataservice GetDataservice(Type type)
        {
            return GetDataservice(type, CruiseDatastore);
        }

        private IDataservice GetDataservice(Type type, CruiseDatastore_V3 cruiseDatastore)
        {
            var cruiseID = CruiseID;



            // all dataservices below should return null if cruiseDatastore is null
            // note: I am skeptical about wheather this method should return null at all
            //      I think it should throw if the dataservice type can't be found
            //      I almost think it should throw if the datastore is null, but I also think it should be
            //      on the classes that relie on the dataservice provider to determin if the dataservice they requested
            //      is esential and throw if null, or allow for null to be returned and check for it. 

            if(cruiseDatastore == null) { return null; }

            if (typeof(ICuttingUnitDatastore).IsAssignableFrom(type))
            { return new CuttingUnitDatastore(cruiseDatastore, cruiseID); }

            if (typeof(ISampleSelectorDataService).IsAssignableFrom(type))
            { return SampleSelectorDataService; }

            if (typeof(ISaleDataservice).IsAssignableFrom(type))
            { return new SaleDataservice(cruiseDatastore, cruiseID); }

            if (typeof(IFixCNTDataservice).IsAssignableFrom(type))
            { return new FixCNTDataservice(cruiseDatastore, cruiseID); }

            if (typeof(ITallyDataservice).IsAssignableFrom(type)
                || typeof(ISampleInfoDataservice).IsAssignableFrom(type))
            { return new TallyDataservice(cruiseDatastore, cruiseID, DeviceInfoService); }

            if (typeof(ITallyPopulationDataservice).IsAssignableFrom(type))
            { return new TallyPopulationDataservice(cruiseDatastore, cruiseID); }

            if (typeof(ISampleInfoDataservice).IsAssignableFrom(type))
            { return new SamplerInfoDataservice(cruiseDatastore, cruiseID, DeviceInfoService); }

            else
            { return null; }
        }
    }
}