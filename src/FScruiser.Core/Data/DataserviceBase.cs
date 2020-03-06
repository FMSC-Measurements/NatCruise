using CruiseDAL;
using CruiseDAL.Schema;
using System;
using System.Linq;

namespace FScruiser.Data
{
    public abstract class DataserviceBase
    {
        protected readonly string PLOT_METHODS = String.Join(", ", CruiseMethods.PLOT_METHODS
            .Append(CruiseMethods.THREEPPNT)
            .Append(CruiseMethods.FIXCNT)
            .Select(x => "'" + x + "'").ToArray());

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

        private void OnDatabaseChanged()
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