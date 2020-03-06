using NatCruise.Data;
using NatCruise.Design.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Data
{
    public class DataserviceProvider : IDataserviceProvider
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

        public object GetDataservice(Type type)
        {
            var cruiseFilePath = CruiseFilePath;
            try
            {


                //if(type == typeof(IRecentFilesDataservice))
                //{
                //    return new RecentFilesDataservice();
                //}
                if (type == typeof(ICruiseDataservice))
                {
                    return new CruiseDataservice(cruiseFilePath);
                }
                else if (type == typeof(ISetupInfoDataservice))
                {
                    return new SetupInfoDataservice();
                }
                else if (type == typeof(ICuttingUnitDataservice))
                {
                    return new CuttingUnitDataservice(cruiseFilePath);
                }
                else if (type == typeof(ISampleGroupDataservice))
                {
                    return new SampleGroupDataservice(cruiseFilePath);
                }
                else if (type == typeof(ISpeciesCodeDataservice))
                {
                    return new SpeciesCodeDataservice(cruiseFilePath);
                }
                else if (type == typeof(IStratumDataservice))
                {
                    return new StratumDataservice(cruiseFilePath);
                }
                else if (type == typeof(ISubpopulationDataservice))
                {
                    return new SubpopulationDataservice(cruiseFilePath);
                }
                else if (type == typeof(ITallyPopulationDataservice))
                {
                }
                else if (type == typeof(ITallySettingsDataservice))
                {
                }
                else if (type == typeof(ITreeFieldDataservice))
                {
                }

                return null;
            }
            catch (Exception e)
            {
                throw;
                //return null;
            }
        }

        public T GetDataservice<T>()
        {
            return (T)GetDataservice(typeof(T));
        }

        public void OpenFile(string filePath)
        {
            var fileExtention = Path.GetExtension(filePath).ToLower();
            if (fileExtention == ".cruise")
            {
                var convertedPath = CruiseDAL.Migrator.GetConvertedPath(filePath);
                CruiseDAL.Migrator.MigrateFromV2ToV3(filePath, convertedPath);

                filePath = convertedPath;
            }

            _cruiseFilePath = filePath;
        }

        public Task OpenFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}