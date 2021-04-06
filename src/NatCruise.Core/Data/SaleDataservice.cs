using CruiseDAL;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public class SaleDataservice : CruiseDataserviceBase, ISaleDataservice
    {
        public SaleDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public SaleDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public void DeleteCruise(string cruiseID)
        {
            var database = Database;
            var saleID = database.ExecuteScalar<string>("SELECT SaleID FROM Cruise WHERE CruiseID = @p1;", cruiseID);
            database.Execute("DELETE FROM Cruise WHERE CruiseID = @p1;", cruiseID);
            database.Execute("DELETE FROM Sale WHERE SaleID = @p1 AND (SELECT count(*) FROM Cruise WHERE SaleID = @p1) = 0; ", saleID);
        }

        public IEnumerable<Cruise> GetCruises()
        {
            return Database.From<Cruise>()
                .Join("Sale AS s", "USING (SaleID)")
                .Query().ToArray();
        }

        public IEnumerable<Cruise> GetCruises(int saleNumber)
        {
            return Database.From<Cruise>()
                .Join("Sale AS s", "USING (SaleID)")
                .Where("s.SaleNumber = @p1")
                .Query(saleNumber).ToArray();
        }

        public IEnumerable<Cruise> GetCruises(string saleID)
        {
            return Database.From<Cruise>()
                .Join("Sale AS s", "USING (SaleID)")
                .Where("SaleID = @p1")
                .Query(saleID).ToArray();
        }

        public Cruise GetCruise()
        {
            return GetCruise(CruiseID);
        }

        public Cruise GetCruise(string cruiseID)
        {
            return Database.From<Cruise>()
                .Join("Sale AS s", "USING (SaleID)")
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
            return GetSale(CruiseID);
        }

        public Sale GetSale(string cruiseID)
        {
            if (string.IsNullOrEmpty(cruiseID)) { throw new ArgumentException($"'{nameof(cruiseID)}' cannot be null or empty.", nameof(cruiseID)); }

            return Database.From<Sale>()
                .Join("Cruise", "USING (SaleID)")
                .Where("Cruise.CruiseID = @p1")
                .Query(cruiseID)
                .FirstOrDefault();
        }

        public IEnumerable<Sale> GetSales()
        {
            return Database.From<Sale>()
                .Query().ToArray();
        }

        public void UpdateCruise(Cruise cruise)
        {
            if (cruise is null) { throw new ArgumentNullException(nameof(cruise)); }

            Database.Execute2(
@"UPDATE Cruise SET 
    Purpose = @Purpose,
    Remarks = @Remarks,
    ModifiedBy = @DeviceID
WHERE CruiseID = @CruiseID",
                new
                {
                    cruise.CruiseID,
                    cruise.Purpose,
                    cruise.Remarks,
                    DeviceID,
                });
        }

        public void UpdateSale(Sale sale)
        {
            if (sale is null) { throw new ArgumentNullException(nameof(sale)); }

            Database.Execute2(
@"UPDATE Sale SET
    Name = @Name,
    SaleNumber = @SaleNumber,
    Region = @Region,
    Forest = @Forest,
    District = @District,
    Remarks = @Remarks,
    ModifiedBy = @DeviceID
WHERE SaleID = @SaleID;",
new
{
    sale.Name,
    sale.SaleNumber,
    sale.Region,
    sale.Forest,
    sale.District,
    sale.Remarks,
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
