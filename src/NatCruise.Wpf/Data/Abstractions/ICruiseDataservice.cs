using NatCruise.Wpf.Models;

namespace NatCruise.Wpf.Data
{
    public interface ICruiseDataservice
    {
        Sale GetSale();

        void UpdateSale(Sale sale);
    }
}