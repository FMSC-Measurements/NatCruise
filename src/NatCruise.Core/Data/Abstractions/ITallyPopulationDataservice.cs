using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITallyPopulationDataservice : IDataservice
    {
        IEnumerable<TallyPopulationEx> GetTallyPopulationsByUnitCode(string unitCode);

        TallyPopulationEx GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);

        IEnumerable<TallyPopulation> GetTallyPopulations(string stratumCode, string sampleGroupCode);

        void UpdateTallyPopulation(TallyPopulation tallyPop);
    }
}