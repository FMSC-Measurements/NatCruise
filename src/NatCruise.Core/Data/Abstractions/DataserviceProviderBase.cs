using CruiseDAL;
using System;
using System.IO;

namespace NatCruise.Data
{
    public abstract class DataserviceProviderBase : IDataserviceProvider
    {
        private string _cruiseID;

        public string DatabasePath => Database.Path;

        public CruiseDatastore_V3 Database { get; protected set; }

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

        public DataserviceProviderBase(CruiseDatastore_V3 database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public DataserviceProviderBase(string databasePath)
        {
            if (System.IO.File.Exists(databasePath) == false)
            {
                throw new FileNotFoundException("Cruise Database File Not Found", databasePath);
            }
            Database = new CruiseDatastore_V3(databasePath);
        }

        public CruiseDatastore_V3 GetDatabase()
        {
            return Database ?? new CruiseDatastore_V3(DatabasePath);
        }

        public abstract IDataservice GetDataservice(Type type);

        public T GetDataservice<T>() where T : class, IDataservice
        {
            return (T)GetDataservice(typeof(T));
        }
    }
}