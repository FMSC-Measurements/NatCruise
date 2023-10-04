using NatCruise.Data;
using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IFixCNTDataservice : IDataservice
    {
        bool GetOneTreePerTallyOption();

        IEnumerable<FixCntTallyPopulation> GetFixCNTTallyPopulations(string stratumCode);

        void IncrementFixCNTTreeCount(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value);

        void DecrementFixCNTTreeCount(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value);

        void AddFixCNTTree(string unitCode, int plotNumber, string stratumCode,
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