using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface ITallyPopulationDataservice : IDataservice
    {
        IEnumerable<TallyPopulation> GetTallyPopulations();

        IEnumerable<TallyPopulation> GetTallyPopulations(string stratumCode);

        IEnumerable<TallyPopulation> GetTallyPopulations(string stratumCode, string sampleGroupCode);

        void UpdateTallyPopulation(TallyPopulation tallyPop);
    }
}