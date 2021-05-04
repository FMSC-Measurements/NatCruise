using NatCruise.Cruise.Models;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface IPlotTallyService
    {
        Task<TreeStub_Plot> TallyAsync(TallyPopulation_Plot pop,
            string unitCode, int plot);
    }
}