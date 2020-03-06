using FScruiser.Models;

namespace FScruiser.Data
{
    public interface ISaleDataservice
    {
        Sale GetSale();

        void UpdateSale(Sale sale);
    }
}