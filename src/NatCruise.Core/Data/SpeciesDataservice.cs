using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class SpeciesDataservice : CruiseDataserviceBase, ISpeciesDataservice
    {
        public SpeciesDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public SpeciesDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public void AddSpecies(Species sp)
        {
            Database.Execute2(
@"INSERT INTO Species (
    CruiseID,
    SpeciesCode,
    ContractSpecies,
    FIACode
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @ContractSpecies,
    @FIACode
)",
    new
    {
        CruiseID,
        sp.SpeciesCode,
        sp.ContractSpecies,
        sp.FIACode,
    });
        }

        public IEnumerable<Species> GetSpecies()
        {
            return Database.From<Species>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public void UpsertSpecies(Species sp)
        {
            Database.Execute2(
@"INSERT INTO Species (
    CruiseID,
    SpeciesCode,
    ContractSpecies,
    FIACode
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @ContractSpecies,
    @FIACode
)
ON CONFLICT (CruiseID, SpeciesCode) DO
UPDATE SET
    ContractSpecies = @ContractSpecies,
    FIACode = @FIACode
WHERE CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode;",
    new
    {
        CruiseID,
        sp.SpeciesCode,
        sp.ContractSpecies,
        sp.FIACode,
    });
        }

        public void DeleteSpecies(string speciesCode)
        {
            Database.Execute("DELETE FROM Species WHERE SpeciesCode = @p1 AND CruiseID = @p2;", speciesCode, CruiseID);
        }

        public IEnumerable<SpeciesProduct> GetSpeciesProducts()
        {
            return Database.From<SpeciesProduct>().Where("CruiseID = @p1").Query(CruiseID).ToArray();
        }

        public IEnumerable<SpeciesProduct> GetSpeciesProducts(string speciesCode)
        {
            return Database.From<SpeciesProduct>().Where("CruiseID = @p1 AND SpeciesCode = @p2").Query(CruiseID, speciesCode).ToArray();
        }

        public SpeciesProduct AddSpeciesProduct(string species, string product, string contractSpecies)
        {
            var spProd = new SpeciesProduct
            {
                SpeciesCode = species,
                PrimaryProduct = product,
                ContractSpecies = contractSpecies
            };

            AddSpeciesProduct(spProd);
            return spProd;
        }

        //public void DeleteSpeciesProduct(string speciesCode, string product)
        //{
        //    Database.Execute("DELETE FROM Species_Product WHERE CruiseID = @p1 AND SpeciesCode = @p2 AND ifnull(PrimaryProduct, '') = ifnull(@p3, '');",
        //        CruiseID, speciesCode, product);
        //}

        public void AddSpeciesProduct(SpeciesProduct spProd)
        {
            Database.Execute2(
@"INSERT INTO Species_Product (
    CruiseID,
    SpeciesCode,
    PrimaryProduct,
    ContractSpecies
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @PrimaryProduct,
    @ContractSpecies
);",
            new
            {
                CruiseID,
                spProd.SpeciesCode,
                spProd.PrimaryProduct,
                spProd.ContractSpecies,
            });
        }

        public void UpdateSpeciesProduct(SpeciesProduct spProd)
        {
            Database.Execute2(
@"Update Species_Product SET
    ContractSpecies = @ContractSpecies
WHERE CruiseID = @CuiseID
    AND SpeciesCode = @SpeciesCode
    AND ifnull(PrimaryProduct,'') = ifnull(@PrimaryProduct, '');",
            new
            {
                CruiseID,
                spProd.SpeciesCode,
                spProd.PrimaryProduct,
                spProd.ContractSpecies,
            });
        }

        public void DeleteSpeciesProduct(SpeciesProduct spProd)
        {
            Database.Execute2(
@"DELETE FROM Species_Product
WHERE CruiseID = @CruiseID
    AND SpeciesCode = @SpeciesCode
    AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '');",
                new
                {
                    CruiseID,
                    SpeciesCode = spProd.SpeciesCode,
                    PrimaryProduct = spProd.PrimaryProduct,
                });
        }

        public void AddSpeciesCode(string speciesCode)
        {
            Database.Execute(
@"INSERT INTO Species (
    CruiseID,
    SpeciesCode
) VALUES (
    @p1,
    @p2
);", CruiseID, speciesCode);
        }

        public IEnumerable<string> GetSpeciesCodes()
        {
            return Database.QueryScalar<string>("SELECT SpeciesCode FROM Species WHERE CruiseID = @p1;", CruiseID).ToArray();
        }

        public IEnumerable<string> GetSpeciesCodes(string stratumCode, string sampleGroupCode)
        {
            if (stratumCode is null) { throw new ArgumentNullException(nameof(stratumCode)); }

            return Database.QueryScalar<string>("SELECT SpeciesCode FROM Subpopulation WHERE CruiseID = @p1 AND StratumCode = @p2 AND (@p2 IS NULL OR SampleGroupCode = @p3);",
                CruiseID, stratumCode, sampleGroupCode).ToArray();
        }
    }
}