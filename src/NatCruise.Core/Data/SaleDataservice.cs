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
        public SaleDataservice(string path, string cruiseID) : base(path, cruiseID)
        {
        }

        public SaleDataservice(CruiseDatastore_V3 database, string cruiseID) : base(database, cruiseID)
        {
        }

        public IEnumerable<SaleCruises> GetSaleCruises()
        {
            var sales = GetSales();
            var saleCruises = sales.Select(x => new SaleCruises() { Sale = x }).ToArray();

            foreach(var sc in saleCruises)
            {
                var saleID = sc.Sale.SaleID;
                sc.Cruises = GetCruises(saleID);
            }

            return saleCruises;
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

        public Sale GetSale(string cruiseID)
        {
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

            Database.Execute2("UPDATE Cruise SET " +
                "Purpose = @Purpose, Remarks = @Remarks, ModifiedBy = @UserName " +
                "WHERE Cruise_CN = @Cruise_CN", cruise);
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
    Remarks = @Remarks
WHERE SaleID = @SaleID;", sale);
        }

        public void UpdateSaleRemarks(long sale_CN, string remarks)
        {
            Database.Execute("UPDATE Sale SET Remarks = @p1 WHERE Sale_CN = @p2;", remarks, sale_CN);
        }
    }
}
