using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;

namespace NatCruise.Cruise.Data
{
    public interface ITallyPopulationDataservice : IDataservice
    {
        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);
    }
}