using NatCruise.Navigation;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public interface IDesignNavigationService : INatCruiseNavigationService
    {
        Task ShowCruise();

        Task ShowSale();

        

        Task ShowTemplateLandingLayout();

        Task ShowTreeDefaultValues();

        Task ShowSpecies();

        Task ShowDesignTemplates();

        Task ShowTreeFields();

        Task ShowLogFields();

        Task ShowDesignChecks();

        Task ShowCombineFile();
    }
}