using CruiseDAL;
using FScruiser.Data;
using FScruiser.Services;
using Prism.Autofac;
using System;
using Xamarin.Forms;

namespace FScruiser.XF.Services
{
    public interface IDataserviceProvider
    {
        string CruisePath { get; set; }

        TResult Get<TResult>() where TResult : class;

        object Get(Type type);

        //void Register<T>(T instance);
    }

    public class DataserviceProvider : IDataserviceProvider
    {
        private string _cruisePath;
        ICruisersDataservice _cruisersDataservice;

        

        public DataserviceProvider(PrismApplication application)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }

        protected PrismApplication Application { get; }
        protected Prism.Ioc.IContainerProvider Container => Application.Container;
        protected IDeviceInfoService DeviceInfoService => (IDeviceInfoService)Container.Resolve(typeof(IDeviceInfoService));

        public ICuttingUnitDatastore CuttingUnitDatastore { get; set; }
        public ISampleSelectorDataService SampleSelectorDataService { get; set; }

        public ICruisersDataservice CruisersDataService { get; set; }

        public CruiseDatastore_V3 CruiseDatastore { get; set; }

        public string CruisePath
        {
            get => _cruisePath;
            set
            {
                if (value != null)
                {
                    var datastore = new CruiseDatastore_V3(value);
                    CruiseDatastore = datastore;
                    SampleSelectorDataService = new SampleSelectorRepository((ISampleInfoDataservice) Get(typeof(ISampleInfoDataservice), datastore));
                }
                _cruisePath = value;
            }
        }

        public TResult Get<TResult>() where TResult : class
        {
            return (TResult)Get(typeof(TResult));
        }

        public object Get(Type type)
        {
            return Get(type, CruiseDatastore);
        }

        private object Get(Type type, CruiseDatastore_V3 cruiseDatastore)
        {
            // non cruise file dependant dataservices
            if (type.IsAssignableFrom(typeof(ICruisersDataservice)))
            { return _cruisersDataservice ?? (_cruisersDataservice = new CruisersDataservice(Application)); }

            // all dataservices below should return null if cruiseDatastore is null
            // note: I am skeptical about wheather this method should return null at all
            //      I think it should throw if the dataservice type can't be found
            //      I almost think it should throw if the datastore is null, but I also think it should be
            //      on the classes that relie on the dataservice provider to determin if the dataservice they requested
            //      is esential and throw if null, or allow for null to be returned and check for it. 

            if(cruiseDatastore == null) { return null; }

            if (type.IsAssignableFrom(typeof(ICuttingUnitDatastore)))
            { return new CuttingUnitDatastore(cruiseDatastore); }

            if (type.IsAssignableFrom(typeof(ISampleSelectorDataService)))
            { return SampleSelectorDataService; }

            if (type.IsAssignableFrom(typeof(ISaleDataservice)))
            { return new SaleDataservice(cruiseDatastore); }

            if (type.IsAssignableFrom(typeof(IFixCNTDataservice)))
            { return new FixCNTDataservice(cruiseDatastore); }

            if (type.IsAssignableFrom(typeof(ITallyDataservice))
                || type.IsAssignableFrom(typeof(ISampleInfoDataservice)))
            { return new TallyDataservice(cruiseDatastore, DeviceInfoService); }

            if (type.IsAssignableFrom(typeof(ITallyPopulationDataservice)))
            { return new TallyPopulationDataservice(cruiseDatastore); }

            if (type.IsAssignableFrom(typeof(ISampleInfoDataservice)))
            { return new SamplerInfoDataservice(cruiseDatastore, DeviceInfoService); }

            else
            { return null; }
        }

        

        //public void Register<T>(T instance)
        //{
        //    var type = typeof(T);

        //    if (type.IsAssignableFrom(typeof(ICuttingUnitDatastore)))
        //    { CuttingUnitDatastore = (ICuttingUnitDatastore)instance; }

        //    if (type.IsAssignableFrom(typeof(ISampleSelectorDataService)))
        //    { SampleSelectorDataService = (ISampleSelectorDataService)instance; }
        //}
    }
}