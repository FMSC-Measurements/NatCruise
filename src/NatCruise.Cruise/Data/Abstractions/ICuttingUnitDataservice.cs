using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;

namespace NatCruise.Cruise.Data
{
    public interface ICuttingUnitDataservice : IDataservice
    {
        CuttingUnit_Ex GetUnit(string code);

        IEnumerable<CuttingUnit_Ex> GetUnits();

        void UpdateCuttingUnit(CuttingUnit_Ex cuttingUnit);

        #region stratra

        string GetCruiseMethod(string stratumCode);

        IEnumerable<string> GetStratumCodesByUnit(string unitCode);

        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode);

        #endregion stratra

        #region sampleGroups

        IEnumerable<string> GetSampleGroupCodes(string stratumCode);

        SampleGroup GetSampleGroup(string stratumCode, string sgCode);

        IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode);

        SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode);

        #endregion sampleGroups

        IEnumerable<SubPopulation> GetSubPopulations(string stratumCode, string sampleGroupCode);

        void LogMessage(string message, string level);
    }
}