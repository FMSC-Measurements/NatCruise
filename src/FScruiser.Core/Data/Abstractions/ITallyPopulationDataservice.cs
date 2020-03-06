using FScruiser.Models;
using System.Collections.Generic;

namespace FScruiser.Data
{
    public interface ITallyPopulationDataservice
    {
        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);
    }
}