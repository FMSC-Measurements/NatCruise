using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface ISubpopulationDataservice
    {
        IEnumerable<Subpopulation> GetSubpopulations(string stratumCode, string sampleGroupCode);

        bool Exists(string stratumCode, string sampleGroupCode, string species, string livedead);

        bool HasTreeCounts(string stratumCode, string sampleGroupCode, string species, string livedead);

        void AddSubpopulation(Subpopulation subpopulation);

        void DeleteSubpopulation(Subpopulation subpopulation);

        void UpdateSubpopulation(Subpopulation subpopulation);
    }
}