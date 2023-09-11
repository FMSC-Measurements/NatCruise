using NatCruise.Models;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface IPlotTallyService
    {
        Task<PlotTreeEntry> TallyAsync(TallyPopulation_Plot pop,
            string unitCode, int plot);
    }
}