using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITallyPopulationDataservice : IDataservice
    {
        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        TallyPopulation_Plot GetPlotTallyPopulation(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead);

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);

        IEnumerable<TallyPopulation> GetTallyPopulations(string cuttingUnitCode = null, string stratumCode = null, string sampleGroupCode = null);

        void UpdateTallyPopulation(TallyPopulation tallyPop);
    }
}