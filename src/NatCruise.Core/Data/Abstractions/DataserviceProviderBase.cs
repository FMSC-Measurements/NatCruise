using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public abstract class DataserviceProviderBase : IDataserviceProvider
    {
        private string _cruiseID;
        private CruiseDatastore_V3 _datastore;
        private string _databasePath;

        protected CruiseDatastore_V3 Datastore
        {
            get => _datastore;
            set => _datastore = value;
        }

        public string DatabasePath
        {
            get => _databasePath;
            set
            {
                _databasePath = value;
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

        protected virtual void OnCruiseIDChanged(string value)
        {
            
        }

        public DataserviceProviderBase(CruiseDatastore_V3 datastore)
        {
            _datastore = datastore ?? throw new ArgumentNullException(nameof(datastore));
        }

        public DataserviceProviderBase(string databasePath)
        {
            OpenDatabase(databasePath);
        }

        public CruiseDatastore_V3 GetDatabase()
        {
            return _datastore ?? new CruiseDatastore_V3(DatabasePath);
        }

        public abstract IDataservice GetDataservice(Type type);

        public T GetDataservice<T>() where T : class, IDataservice
        {
            return (T)GetDataservice(typeof(T));
        }

        public virtual void OpenDatabase(string databasePath)
        {
            if(System.IO.File.Exists(databasePath) == false)
            {
                throw new FileNotFoundException("Cruise Database File Not Found", databasePath);
            }

            DatabasePath = databasePath;
        }
    }
}
