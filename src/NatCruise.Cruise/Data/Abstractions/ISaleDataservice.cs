using NatCruise.Cruise.Models;
using NatCruise.Data;

namespace NatCruise.Cruise.Data
{
    public interface ISaleDataservice : IDataservice
    {
        Sale GetSale();

        void UpdateSale(Sale sale);
    }
}