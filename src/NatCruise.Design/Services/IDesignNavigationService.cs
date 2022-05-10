using NatCruise.Navigation;
using System.Threading.Tasks;

namespace NatCruise.Design.Services
{
    public interface IDesignNavigationService : INatCruiseNavigationService
    {
        Task ShowCruise();

        Task ShowSale();

        Task ShowStrata();

        Task ShowCuttingUnitList();

        Task ShowCruiseLandingLayout();

        Task ShowTemplateLandingLayout();

        Task ShowAuditRules();

        Task ShowTreeDefaultValues();

        Task ShowSpecies();

        Task ShowDesignTemplates();

        Task ShowTreeFields();

        Task ShowLogFields();

        Task ShowDesignChecks();

        Task ShowCombineFile();
    }
}