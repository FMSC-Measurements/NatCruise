using NatCruise.Data;
using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IFixCNTDataservice : IDataservice
    {
        bool GetOneTreePerTallyOption();

        IEnumerable<FixCntTallyPopulation> GetFixCNTTallyPopulations(string stratumCode);

        /// <summary>
        /// Increment the tree count for a tree in the tally
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="plotNumber"></param>
        /// <param name="stratumCode"></param>
        /// <param name="sgCode"></param>
        /// <param name="species"></param>
        /// <param name="liveDead"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns>TreeID value of the existing tree or tree that was created</returns>
        string IncrementFixCNTTreeCount(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value);

        void DecrementFixCNTTreeCount(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value);

        string AddFixCNTTree(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value);

        void RemoveFixCNTTree(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value);

        int GetTreeCount(string unit,
            int plotNumber,
            string stratumCode,
            string sampleGroupCode,
            string species,
            string livedead,
            string field,
            double value);
    }
}