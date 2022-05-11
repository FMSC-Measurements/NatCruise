using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ISubpopulationDataservice : IDataservice
    {
        IEnumerable<Subpopulation> GetSubpopulations(string stratumCode, string sampleGroupCode);

        bool Exists(string stratumCode, string sampleGroupCode, string species, string livedead);

        bool HasTreeCounts(string stratumCode, string sampleGroupCode, string species, string livedead);

        void AddSubpopulation(Subpopulation subpopulation);

        void DeleteSubpopulation(Subpopulation subpopulation);

        void UpdateSubpopulation(Subpopulation subpopulation);

        void UpsertFixCNTTallyPopulation(Subpopulation subpopulation);
    }
}