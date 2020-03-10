using NatCruise.Data;
using NatCruise.Design.Models;

namespace NatCruise.Design.Data
{
    public interface ICruiseDataservice : IDataservice
    {
        Sale GetSale();

        void UpdateSale(Sale sale);
    }
}