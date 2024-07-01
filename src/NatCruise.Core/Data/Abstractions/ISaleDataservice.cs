using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ISaleDataservice : IDataservice
    {
        IReadOnlyCollection<Sale> GetSales();

        Sale GetSale();

        Sale GetSale(int saleNumber);

        Sale GetSaleByCruiseID(string cruiseID);

        Sale GetSaleBySaleNumber(string saleNumber);

        void UpdateSale(Sale sale);

        void UpdateSaleNumber(Sale sale);

        void DeleteCruise(string cruiseID);

        IReadOnlyCollection<Models.Cruise> GetCruises();

        IReadOnlyCollection<Models.Cruise> GetCruisesBySaleNumber(string saleNumber);

        IReadOnlyCollection<Models.Cruise> GetCruises(string saleID);

        Models.Cruise GetCruise();

        Models.Cruise GetCruise(string cruiseID);

        void UpdateCruise(Models.Cruise cruise);
    }
}