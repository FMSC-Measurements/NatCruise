using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data.Abstractions
{
    public interface ISaleDataservice : IDataservice
    {
        IEnumerable<Sale> GetSales();

        Sale GetSale();

        Sale GetSale(int saleNumber);

        Sale GetSale(string cruiseID);

        void UpdateSale(Sale sale);

        void DeleteCruise(string cruiseID);

        IEnumerable<Cruise> GetCruises();

        IEnumerable<Cruise> GetCruises(int saleNumber);

        IEnumerable<Cruise> GetCruises(string saleID);

        Cruise GetCruise();

        Cruise GetCruise(string cruiseID);

        void UpdateCruise(Cruise cruise);
    }
}