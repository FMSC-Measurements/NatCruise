using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public interface INatCruiseNavigationService
    {
        Task ShowFeedback();

        Task ShowSettings();

        Task ShowSampleStateManagment();

        //Task ShowImport();

        Task ShowCruiseSelect(string saleNumber);

        Task GoBackAsync();
    }
}