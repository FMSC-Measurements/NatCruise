using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data.Abstractions
{
    public interface ISaleDataservice : IDataservice
    {
        IEnumerable<Sale> GetSales();

        Sale GetSale();

        Sale GetSale(int saleNumber);

        Sale GetSaleByCruiseID(string cruiseID);

        Sale GetSaleBySaleNumber(string saleNumber);

        void UpdateSale(Sale sale);

        void DeleteCruise(string cruiseID);

        IEnumerable<Cruise> GetCruises();

        IEnumerable<Cruise> GetCruisesBySaleNumber(string saleNumber);

        IEnumerable<Cruise> GetCruises(string saleID);

        Cruise GetCruise();

        Cruise GetCruise(string cruiseID);

        void UpdateCruise(Cruise cruise);
    }
}