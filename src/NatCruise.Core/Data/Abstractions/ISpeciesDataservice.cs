using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ISpeciesDataservice : IDataservice
    {
        IEnumerable<Species> GetSpecies();

        void AddSpecies(Species sp);

        void UpsertSpecies(Species sp);

        void DeleteSpecies(string speciesCode);

        IEnumerable<SpeciesProduct> GetSpeciesProducts();

        IEnumerable<SpeciesProduct> GetSpeciesProducts(string speciesCode);

        SpeciesProduct AddSpeciesProduct(string species, string product, string contractSpecies);

        void AddSpeciesProduct(SpeciesProduct spProd);

        //void DeleteSpeciesProduct(string speciesCode, string product);
        void DeleteSpeciesProduct(SpeciesProduct spProd);

        IEnumerable<string> GetSpeciesCodes();

        IEnumerable<string> GetSpeciesCodes(string stratumCode, string sampleGroupCode);

        void AddSpeciesCode(string speciesCode);
    }
}
