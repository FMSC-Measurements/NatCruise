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
        private string _cruiseFilePath;

        public string CruiseFilePath
        {
            get => _cruiseFilePath;
            set
            {
                _cruiseFilePath = value;
            }
        }

        public abstract IDataservice GetDataservice(Type type);

        public T GetDataservice<T>() where T : IDataservice
        {
            return (T)GetDataservice(typeof(T));
        }

        public virtual void OpenFile(string filePath)
        {
            var fileExtention = Path.GetExtension(filePath).ToLower();
            if (fileExtention == ".cruise")
            {
                var convertedPath = CruiseDAL.Migrator.GetConvertedPath(filePath);
                CruiseDAL.Migrator.MigrateFromV2ToV3(filePath, convertedPath);

                filePath = convertedPath;
            }

            if(System.IO.File.Exists(filePath) == false)
            {
                throw new FileNotFoundException("File Not Found", filePath);
            }

            CruiseFilePath = filePath;
        }

        public Task OpenFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
