using CruiseDAL;
using CruiseDAL.V3.Models;
using NatCruise.Core.Services;
using Prism.Ioc;
using System;
using System.IO;
using System.Linq;

namespace NatCruise.Data
{
    public abstract class DataserviceProviderBase : IDataserviceProvider
    {
        private string _cruiseID;
        private CruiseDatastore_V3 _database;

        public string DatabasePath => Database.Path;

        public CruiseDatastore_V3 Database
        {
            get => _database;
            set
            {
                CruiseID = null;
                _database = value;
            }
        }

        public IDeviceInfoService DeviceInfoService { get; }
        public string DeviceID { get; }

        public string CruiseID
        {
            get => _cruiseID;
            set
            {
                _cruiseID = value;
                OnCruiseIDChanged(value);
            }
        }

        protected virtual void OnCruiseIDChanged(string value)
        {
            if (value != null)
            {
                InitCurrentDevice(DeviceInfoService);
            }
        }

        public static void RegisterDataservices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISaleDataservice>(x => GetDataservice<ISaleDataservice>(x));
            containerRegistry.Register<ICuttingUnitDataservice>(x => GetDataservice<ICuttingUnitDataservice>(x));
            containerRegistry.Register<IStratumDataservice>(x => GetDataservice<IStratumDataservice>(x));
            containerRegistry.Register<ISampleGroupDataservice>(x => GetDataservice<ISampleGroupDataservice>(x));
            containerRegistry.Register<ISubpopulationDataservice>(x => GetDataservice<ISubpopulationDataservice>(x));
            containerRegistry.Register<ITreeDataservice>(x => GetDataservice<ITreeDataservice>(x));
            containerRegistry.Register<ITreeErrorDataservice>(x => GetDataservice<ITreeErrorDataservice>(x));
            containerRegistry.Register<ITreeFieldDataservice>(x => GetDataservice<ITreeFieldDataservice>(x));
            containerRegistry.Register<ITreeFieldValueDataservice>(x => GetDataservice<ITreeFieldValueDataservice>(x));
            containerRegistry.Register<ILogDataservice>(x => GetDataservice<ILogDataservice>(x));
            containerRegistry.Register<ILogErrorDataservice>(x => GetDataservice<ILogErrorDataservice>(x));
            containerRegistry.Register<IFieldSetupDataservice>(x => GetDataservice<IFieldSetupDataservice>(x));
            containerRegistry.Register<IMessageLogDataservice>(x => GetDataservice<IMessageLogDataservice>(x));
            containerRegistry.Register<ITallyPopulationDataservice>(x => GetDataservice<ITallyPopulationDataservice>(x));
            containerRegistry.Register<IPlotDataservice>(x => GetDataservice<IPlotDataservice>(x));
            containerRegistry.Register<IPlotStratumDataservice>(x => GetDataservice<IPlotStratumDataservice>(x));
            containerRegistry.Register<IPlotErrorDataservice>(x => GetDataservice<IPlotErrorDataservice>(x));
            containerRegistry.Register<IPlotTreeDataservice>(x => GetDataservice<IPlotTreeDataservice>(x));
            containerRegistry.Register<ITallyPopulationDataservice>(x => GetDataservice<ITallyPopulationDataservice>(x));
            containerRegistry.Register<ILogFieldDataservice>(x => GetDataservice<ILogFieldDataservice>(x));
            containerRegistry.Register<ITallyLedgerDataservice>(x => GetDataservice<ITallyLedgerDataservice>(x));
        }

        public DataserviceProviderBase(CruiseDatastore_V3 database, IDeviceInfoService deviceInfoService)
        {
            Database = database; //?? throw new ArgumentNullException(nameof(database));
            DeviceInfoService = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));
            DeviceID = deviceInfoService.DeviceID;
        }

        public DataserviceProviderBase(string databasePath, IDeviceInfoService deviceInfoService)
        {
            if (System.IO.File.Exists(databasePath) == false)
            {
                throw new FileNotFoundException("Cruise Database File Not Found", databasePath);
            }
            Database = new CruiseDatastore_V3(databasePath);
            DeviceInfoService = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));
            DeviceID = deviceInfoService.DeviceID;
        }

        public DataserviceProviderBase(IDeviceInfoService deviceInfoService)
        {
            DeviceInfoService = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));
            DeviceID = deviceInfoService.DeviceID;
        }

        public virtual IDataservice GetDataservice(Type type)
        {
            var cruiseID = CruiseID;
            var database = Database;
            var deviceID = DeviceID;

            if (type == typeof(ISaleDataservice))
            {
                return new SaleDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ICuttingUnitDataservice))
            {
                return new CuttingUnitDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(IStratumDataservice))
            {
                return new StratumDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ISampleGroupDataservice))
            {
                return new SampleGroupDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ISubpopulationDataservice))
            {
                return new SubpopulationDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ITreeDataservice))
            {
                return new TreeDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ITreeErrorDataservice))
            {
                return new TreeErrorDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ITreeFieldDataservice))
            {
                return new TreeFieldDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ITreeFieldValueDataservice))
            {
                return new TreeFieldValueDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ILogDataservice))
            {
                return new LogDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(ILogErrorDataservice))
            {
                return new LogErrorDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(IFieldSetupDataservice))
            {
                return new FieldSetupDataservice(database, cruiseID, deviceID);
            }
            else if (type == typeof(IMessageLogDataservice))
            {
                return new MessageLogDataservice(database, deviceID);
            }
            else if (type == typeof(ITallyPopulationDataservice))
            {
                return new TallyPopulationDataservice(database, cruiseID, deviceID);
            }
            else if(typeof(IPlotDataservice) == type)
            {
                return new PlotDataservice(database, cruiseID, deviceID);
            }
            else if (typeof(IPlotStratumDataservice) == type )
            {
                return new PlotStratumDataservice(database, cruiseID, deviceID);
            }
            else if (typeof(IPlotErrorDataservice) == type)
            {
                return new PlotErrorDataservice(database, cruiseID, deviceID);
            }
            else if(typeof(IPlotTreeDataservice)== type)
            {
                return new PlotTreeDataservice(database, cruiseID, deviceID, GetDataservice<ISamplerStateDataservice>());
            }
            if (typeof(ISamplerStateDataservice).IsAssignableFrom(type))
            {
                return new SamplerStateDataservice(database, cruiseID, deviceID);
            }
            else if(typeof(ITallyPopulationDataservice) == type)
            {
                return new TallyPopulationDataservice(database, cruiseID, deviceID);
            }
            if (type == typeof(ILogFieldDataservice))
            {
                return new LogFieldDataservice(database, cruiseID, deviceID);
            }
            if (type == typeof(ITallyLedgerDataservice))
            {
                return new TallyLedgerDataservice(database, cruiseID, deviceID);
            }

            else
            {
                return null;
            }
        }

        public T GetDataservice<T>() where T : class, IDataservice
        {
            return (T)GetDataservice(typeof(T));
        }

        protected Device InitCurrentDevice(IDeviceInfoService deviceInfoService)
        {
            var deviceID = deviceInfoService.DeviceID;
            var deviceName = deviceInfoService.DeviceName;
            var cruiseID = CruiseID;
            var database = Database;

            var device = database.Query<Device>("SELECT * FROM Device WHERE DeviceID = @p1 AND CruiseID = @p2;", deviceID, cruiseID).FirstOrDefault();

            if (device == null)
            {
                device = new Device
                {
                    CruiseID = cruiseID,
                    DeviceID = deviceID,
                    Name = deviceName,
                };

                database.Insert(device);
            }

            return device;
        }

        public static T GetDataservice<T>(IContainerProvider container) where T : class, IDataservice
        {
            var dsp = container.Resolve<IDataserviceProvider>();
            return dsp.GetDataservice<T>();
        }
    }
}