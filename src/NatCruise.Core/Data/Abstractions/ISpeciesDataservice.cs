﻿using NatCruise.Models;
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

        //void DeleteSpeciesProduct(string speciesCode, string product);
        void DeleteSpeciesProduct(SpeciesProduct spProd);

        IEnumerable<string> GetSpeciesCodes();

        void AddSpeciesCode(string speciesCode);
    }
}
