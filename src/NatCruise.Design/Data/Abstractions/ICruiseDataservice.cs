using NatCruise.Design.Models;

namespace NatCruise.Design.Data
{
    public interface ICruiseDataservice
    {
        Sale GetSale();

        void UpdateSale(Sale sale);
    }
}