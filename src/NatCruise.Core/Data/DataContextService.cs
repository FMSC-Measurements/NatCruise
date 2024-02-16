using CruiseDAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NatCruise.Models;
using NatCruise.Services;
using System;
using System.IO;
using System.Linq;

namespace NatCruise.Data
{
    public class DataContextService : IDataContextService
    {
        public IDeviceInfoService DeviceInfoService { get; }
        public ILogger Logger { get; }


        private string _cruiseID;
        private CruiseDatastore_V3 _database;
        private bool _isDisposed;

        public DataContextService(string deviceID, string deviceName, IServiceProvider serviceProvider, ILogger<DataContextService> logger)
        {
            ServiceProvider = serviceProvider;
            DeviceID = deviceID;
            DeviceName = deviceName;

            Logger = logger;
        }

        public DataContextService(IDeviceInfoService deviceInfoService, IServiceProvider serviceProvider, ILogger<DataContextService> logger)
            : this(deviceInfoService.DeviceID, deviceInfoService.DeviceName, serviceProvider, logger)
        {
        }

        public string DeviceID { get; }
        public string DeviceName { get; }
        IServiceProvider ServiceProvider { get; }

        public string DatabasePath => Database.Path;

        public event EventHandler CruiseChanged;

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

        public bool IsReady {get; protected set; }

        public Exception InitError {get; protected set; }

        public bool OpenOrCreateDatabase(string path)
        {
            try
            {
                if (File.Exists(path) == false)
                {
                    Logger.LogInformation("Creating Database: {path}", path);
                    var db = new CruiseDatastore_V3(path, true);
                    Database = db;
                }
                else
                {
                    Logger.LogInformation("Opening Database: {path}", path);
                    var db = new CruiseDatastore_V3(path, false);
                    Database = db;
                }

                IsReady = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception While Initializing Data Context");
                InitError = ex;
                IsReady = true;
            }

            return IsReady;
        }

        public bool OpenDatabase(string path)
        {
            try
            {
                Logger.LogInformation("Opening Database: {path}", path);
                var db = new CruiseDatastore_V3(path, false);
                Database = db;

                IsReady = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception While Initializing Data Context");
                InitError = ex;
                IsReady = true;
            }

            return IsReady;
        }

        public bool CreateDatabase(string path)
        {
            try
            {
                Logger.LogInformation("Creating Database: {path}", path);
                var db = new CruiseDatastore_V3(path, true);
                Database = db;

                IsReady = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception While Initializing Data Context");
                InitError = ex;
                IsReady = true;
            }

            return IsReady;
        }



        protected virtual void OnCruiseIDChanged(string value)
        {
            Logger.LogInformation("CruiseID Changed: {CruiseID}", value);

            if (value != null)
            {
                InitCurrentDevice();

                SampleSelectorDataService = new SampleSelectorRepository(
                    ServiceProvider.GetRequiredService<ISamplerStateDataservice>(),
                    ServiceProvider.GetRequiredService<ISampleGroupDataservice>());
            }
            else
            {
                SampleSelectorDataService = null;
            }
            CruiseChanged?.Invoke(this, EventArgs.Empty);
        }

        protected Device InitCurrentDevice()
        {
            var deviceID = DeviceID;
            var deviceName = DeviceName;
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

                Logger.LogInformation("Insert New Device Record {CruiseID}, {DeviceID}, {Name}", device.CruiseID, device.DeviceID, device.Name);
                database.Insert(device);
            }

            return device;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Database?.Dispose();
                    Database = null;
                    IsReady = false;
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}