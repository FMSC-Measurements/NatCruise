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
        private string _databasePath;

        public string DatabasePath
        {
            get => _databasePath;
            set
            {
                _databasePath = value;
            }
        }

        public Cruise Cruise { get; set; }
        public string CruiseID => Cruise?.CruiseID;

        public DataserviceProviderBase(string databasePath)
        {
            OpenDatabase(databasePath);
        }

        public abstract IDataservice GetDataservice(Type type);

        public T GetDataservice<T>() where T : IDataservice
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
