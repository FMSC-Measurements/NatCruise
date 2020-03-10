using CruiseDAL;
using CruiseDAL.Schema;
using System;
using System.Linq;

namespace NatCruise.Data
{
    public abstract class DataserviceBase : IDataservice
    {
        protected readonly string PLOT_METHODS = String.Join(", ", CruiseMethods.PLOT_METHODS
            .Append(CruiseMethods.THREEPPNT)
            .Append(CruiseMethods.FIXCNT)
            .Select(x => "'" + x + "'").ToArray());

        protected string DeviceID { get; set; }
        //private string DeviceName { get; set; }

        private CruiseDatastore_V3 _database;

        public CruiseDatastore_V3 Database
        {
            get { return _database; }
            set
            {
                _database = value;
                OnDatabaseChanged();
            }
        }

        protected virtual void OnDatabaseChanged()
        {
            var database = Database;
            if (database == null) { return; }

            //DatabaseUpdater.Update(database);
        }

        public DataserviceBase(string path)
        {
            var database = new CruiseDatastore_V3(path ?? throw new ArgumentNullException(nameof(path)));

            Database = database;
        }

        public DataserviceBase(CruiseDatastore_V3 database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }
    }
}