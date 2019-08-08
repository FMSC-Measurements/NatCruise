using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface ISpeciesCodeDataservice
    {
        IEnumerable<string> GetSpeciesCodes();

        IEnumerable<string> GetSpeciesCodes(string productCode);

        void AddSpeciesCode(string speciesCode);

        void DeleteSpeciesCode(string species);
    }
}