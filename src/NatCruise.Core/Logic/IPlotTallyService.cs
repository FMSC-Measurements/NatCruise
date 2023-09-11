using NatCruise.Models;
using System.Threading.Tasks;

namespace NatCruise.Logic
{
    public interface IPlotTallyService
    {
        Task<PlotTreeEntry> TallyAsync(TallyPopulation_Plot pop,
            string unitCode, int plot);
    }
}