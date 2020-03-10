using NatCruise.Data;
using NatCruise.Design.Data;
using System;

namespace NatCruise.Data
{
    public class DataserviceProvider : DataserviceProviderBase
    {
        public override IDataservice GetDataservice(Type type)
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
    }
}