using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class SaleDataservice : CruiseDataserviceBase, ISaleDataservice
    {
        public SaleDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public SaleDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public SaleDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public void DeleteCruise(string cruiseID)
        {
            var database = Database;
            var saleNumber = database.ExecuteScalar<string>("SELECT SaleNumber FROM Cruise WHERE CruiseID = @p1;", cruiseID);
            database.Execute("DELETE FROM Cruise WHERE CruiseID = @p1;", cruiseID);
            // clean up sale records if there is no more cruise associated with them
            database.Execute("DELETE FROM Sale WHERE SaleNumber = @p1 AND (SELECT count(*) FROM Cruise WHERE SaleNumber = @p1) = 0; ", saleNumber);

            // clean up tombstone records
            CruiseDAL.V3.Sync.CruiseFileUtilities.ClearTombstoneRecords(database, cruiseID);

            //            database.Execute2(
            //@"DELETE FROM TreeDefaultValue_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM CuttingUnit_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM Stratum_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM CuttingUnit_Stratum_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM Plot_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM Plot_Stratum_Tombstone WHERE CruiseID = @CruiseID;
            //--DELETE FROM PlotLocation_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM SampleGroup_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM TreeFieldSetup_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM LogFieldSetup_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM SubPopulation_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM Tree_Tombstone WHERE CruiseID = @CruiseID;
            //--DELETE FROM TreeMeasurment_Tombstone WHERE CruiseID = @CruiseID;
            //--DELETE FROM TreeLocation_Tombstone WHERE CruiseID = @CruiseID;
            //--DELETE FROM TreeFieldValue_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM Log_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM Stem_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM TallyLedger_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM TreeAuditRule_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM TreeAuditRuleSelector_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM TreeAuditResolution_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM LogGradeAuditRule_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM Reports_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM VolumeEquation_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM StratumTemplate_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM StratumTemplateTreeFieldSetup_Tombstone WHERE CruiseID = @CruiseID;
            //DELETE FROM StratumTemplateLogFieldSetup_Tombstone WHERE CruiseID = @CruiseID;", new { CruiseID = cruiseID });
        }

        public IEnumerable<Models.Cruise> GetCruises()
        {
            return Database.From<Models.Cruise>()
                .LeftJoin("LK_Purpose", "USING (Purpose)")
                .Query().ToArray();
        }

        public IEnumerable<Models.Cruise> GetCruises(string saleID)
        {
            return Database.From<Models.Cruise>()
                .LeftJoin("LK_Purpose", "USING (Purpose)")
                .Where("SaleID = @p1")
                .Query(saleID).ToArray();
        }

        public IEnumerable<Models.Cruise> GetCruisesBySaleNumber(string saleNumber)
        {
            return Database.From<Models.Cruise>()
                .LeftJoin("LK_Purpose", "USING (Purpose)")
                .Where("SaleNumber = @p1")
                .Query(saleNumber).ToArray();
        }

        public Models.Cruise GetCruise()
        {
            return GetCruise(CruiseID);
        }

        public Models.Cruise GetCruise(string cruiseID)
        {
            return Database.From<Models.Cruise>()
                .LeftJoin("LK_Purpose", "USING (Purpose)")
                .Where("CruiseID = @p1")
                .Query(cruiseID).First();
        }

        public Sale GetSale(int saleNumber)
        {
            return Database.From<Sale>()
                .Where("SaleNumber = @p1")
                .Query(saleNumber).FirstOrDefault();
        }

        public Sale GetSale()
        {
            return GetSaleByCruiseID(CruiseID);
        }

        public Sale GetSaleByCruiseID(string cruiseID)
        {
            if (string.IsNullOrEmpty(cruiseID)) { throw new ArgumentException($"'{nameof(cruiseID)}' cannot be null or empty.", nameof(cruiseID)); }

            return Database.From<Sale>()
                .Join("Cruise", "USING (SaleNumber)")
                .Where("Cruise.CruiseID = @p1")
                .Query(cruiseID)
                .FirstOrDefault();
        }

        public Sale GetSaleBySaleNumber(string saleNumber)
        {
            if (string.IsNullOrEmpty(saleNumber)) { throw new ArgumentException($"'{nameof(saleNumber)}' cannot be null or empty.", nameof(saleNumber)); }

            return Database.From<Sale>()
                .Where("SaleNumber = @p1")
                .Query(saleNumber)
                .FirstOrDefault();
        }

        public IEnumerable<Sale> GetSales()
        {
            return Database.From<Sale>()
                .Query().ToArray();
        }

        public void UpdateCruise(Models.Cruise cruise)
        {
            if (cruise is null) { throw new ArgumentNullException(nameof(cruise)); }

            Database.Execute2(
@"UPDATE Cruise SET
    CruiseNumber = @CruiseNumber,
    Purpose = @Purpose,
    Remarks = @Remarks,
    DefaultUOM = @DefaultUOM,
    UseCrossStrataPlotTreeNumbering = @UseCrossStrataPlotTreeNumbering,
    ModifiedBy = @DeviceID
WHERE CruiseID = @CruiseID",
                new
                {
                    cruise.CruiseID,
                    cruise.CruiseNumber,
                    cruise.Purpose,
                    cruise.Remarks,
                    cruise.DefaultUOM,
                    cruise.UseCrossStrataPlotTreeNumbering,
                    DeviceID,
                });
        }

        public void UpdateSale(Sale sale)
        {
            if (sale is null) { throw new ArgumentNullException(nameof(sale)); }

            Database.Execute2(
@"UPDATE Sale SET
    Name = @Name,
    Region = @Region,
    Forest = @Forest,
    District = @District,
    Remarks = @Remarks,
    ModifiedBy = @DeviceID
WHERE SaleID = @SaleID;",
new
{
    sale.Name,
    sale.Region,
    sale.Forest,
    sale.District,
    sale.Remarks,
    sale.SaleID,
    DeviceID,
});
        }

        public void UpdateSaleNumber(Sale sale)
        {
            if (sale is null) { throw new ArgumentNullException(nameof(sale)); }

            Database.Execute2(
@"UPDATE Sale SET
    SaleNumber = @SaleNumber,
    ModifiedBy = @DeviceID
WHERE SaleID = @SaleID;",
new
{
    sale.SaleNumber,
    sale.SaleID,
    DeviceID,
});
        }

        public void UpdateSaleRemarks(long sale_CN, string remarks)
        {
            Database.Execute("UPDATE Sale SET Remarks = @p1, ModifiedBy = @p3 WHERE Sale_CN = @p2;", remarks, sale_CN, DeviceID);
        }
    }
}