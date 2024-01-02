using CruiseDAL;
using Microsoft.Extensions.DependencyInjection;
using NatCruise.Models;
using NatCruise.Services;
using System;
using System.Linq;

namespace NatCruise.Data
{
    public class DataContextService : IDataContextService
    {
        public IDeviceInfoService DeviceInfoService { get; }


        private string _cruiseID;
        private CruiseDatastore_V3 _database;

        public DataContextService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            DeviceInfoService = serviceProvider.GetRequiredService<IDeviceInfoService>();

            //DeviceInfoService = deviceInfoService;
        }

        public string DeviceID { get; }

        IServiceProvider ServiceProvider { get; }

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

        public string CruiseID
        {
            get => _cruiseID;
            set
            {
                _cruiseID = value;
                OnCruiseIDChanged(value);
            }
        }

        public ISampleSelectorDataService SampleSelectorDataService { get; protected set; }

        protected virtual void OnCruiseIDChanged(string value)
        {
            if (value != null)
            {
                InitCurrentDevice(DeviceInfoService);
            }

            if (value != null)
            {
                SampleSelectorDataService = new SampleSelectorRepository(
                    ServiceProvider.GetRequiredService<ISamplerStateDataservice>(),
                    ServiceProvider.GetRequiredService<ISampleGroupDataservice>());
            }
            else
            {
                SampleSelectorDataService = null;
            }
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
    }
}