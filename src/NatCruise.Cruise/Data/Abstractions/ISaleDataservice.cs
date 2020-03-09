using NatCruise.Cruise.Models;

namespace NatCruise.Cruise.Data
{
    public interface ISaleDataservice
    {
        Sale GetSale();

        void UpdateSale(Sale sale);
    }
}