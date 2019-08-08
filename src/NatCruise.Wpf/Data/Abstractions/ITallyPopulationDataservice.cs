using NatCruise.Wpf.Models;
using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface ITallyPopulationDataservice
    {
        IEnumerable<TallyPopulation> GetTallyPopulations();

        IEnumerable<TallyPopulation> GetTallyPopulations(string stratumCode);

        IEnumerable<TallyPopulation> GetTallyPopulations(string stratumCode, string sampleGroupCode);

        void UpdateTallyPopulation(TallyPopulation tallyPop);
    }
}