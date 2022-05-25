using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ISaleDataservice : IDataservice
    {
        IEnumerable<Sale> GetSales();

        Sale GetSale();

        Sale GetSale(int saleNumber);

        Sale GetSaleByCruiseID(string cruiseID);

        Sale GetSaleBySaleNumber(string saleNumber);

        void UpdateSale(Sale sale);

        void UpdateSaleNumber(Sale sale);

        void DeleteCruise(string cruiseID);

        IEnumerable<Models.Cruise> GetCruises();

        IEnumerable<Models.Cruise> GetCruisesBySaleNumber(string saleNumber);

        IEnumerable<Models.Cruise> GetCruises(string saleID);

        Models.Cruise GetCruise();

        Models.Cruise GetCruise(string cruiseID);

        void UpdateCruise(Models.Cruise cruise);
    }
}