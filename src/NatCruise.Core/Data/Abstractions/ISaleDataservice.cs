using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data.Abstractions
{
    public interface ISaleDataservice : IDataservice
    {
        IEnumerable<Sale> GetSales();

        Sale GetSale(int saleNumber);

        Sale GetSale(string cruiseID);

        void UpdateSale(Sale sale);

        IEnumerable<SaleCruises> GetSaleCruises();

        void DeleteCruise(string cruiseID);

        IEnumerable<Cruise> GetCruises();

        IEnumerable<Cruise> GetCruises(int saleNumber);

        IEnumerable<Cruise> GetCruises(string saleID);

        Cruise GetCruise(string cruiseID);

        void UpdateCruise(Cruise cruise);
    }
}
