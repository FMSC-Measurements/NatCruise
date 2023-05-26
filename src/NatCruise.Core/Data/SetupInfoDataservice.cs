﻿using CruiseDAL;
using NatCruise.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class SetupInfoDataservice : ISetupInfoDataservice
    {
        CruiseDatastore_V3 Database { get; }

        public SetupInfoDataservice()
        {
            Database = new CruiseDatastore_V3();
        }

        public IEnumerable<Forest> GetForests(string regionCode)
        {
            return Database.From<Forest>().Where("Region = @p1").Query(regionCode).ToArray();
        }

        public IEnumerable<Purpose> GetPurposes()
        {
            return Database.From<Purpose>().Query().ToArray();
        }

        public IEnumerable<Region> GetRegions()
        {
            return Database.From<Region>().Query().ToArray();
        }

        public IEnumerable<UOM> GetUOMCodes()
        {
            return Database.From<UOM>().Query().ToArray();
        }

        public IEnumerable<District> GetDistricts(string region, string forest)
        {
            return Database.From<District>().Where("Region = @p1 AND Forest = @p2").Query(region, forest).ToArray();
        }

        public IEnumerable<FIASpecies> GetFIASpecies()
        {
//            return Database.Query<FIASpecies>(
//@"SELECT * FROM
//(
//    SELECT * FROM LK_FIA
//    UNION
//    SELECT DISTINCT FIACode, '' AS CommonName
//)
//ORDER BY FIACode;").ToArray();

            return Database.From<FIASpecies>().Query().ToArray();
        }

        public IEnumerable<Product> GetProducts()
        {
            return Database.From<Product>().Query().ToArray();
        }

        public IEnumerable<CruiseMethod> GetCruiseMethods()
        {
            return Database.From<CruiseMethod>().Query().ToArray();
        }

        public IEnumerable<LoggingMethod> GetLoggingMethods()
        {
            return Database.From<LoggingMethod>().Query().ToArray();
        }
    }
}