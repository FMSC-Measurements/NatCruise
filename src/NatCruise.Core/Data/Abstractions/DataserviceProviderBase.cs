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

        public abstract void RegisterDataservices(IContainerRegistry containerRegistry);

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

        public abstract IDataservice GetDataservice(Type type);

        public T GetDataservice<T>() where T : class, IDataservice
        {
            return (T)GetDataservice(typeof(T));
        }

        protected Device InitCurrentDevice(IDeviceInfoService deviceInfoService)
        {
            var deviceID = deviceInfoService.DeviceID;
            var deviceName = deviceInfoService.DeviceName;
            var cruiseID = CruiseID;

            var device = Database.Query<Device>("SELECT * FROM Device WHERE DeviceID = @p1 AND CruiseID = @p2;", deviceID, cruiseID).FirstOrDefault();

            if (device == null)
            {
                device = new Device
                {
                    CruiseID = cruiseID,
                    DeviceID = deviceID,
                    Name = deviceName,
                };

                Database.Insert(device);
            }

            return device;
        }
    }
}