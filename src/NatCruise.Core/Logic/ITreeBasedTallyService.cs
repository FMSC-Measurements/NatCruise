using NatCruise.Models;
using System.Threading.Tasks;

namespace NatCruise.Logic
{
    public interface ITreeBasedTallyService
    {
        Task<TallyEntry> TallyAsync(string unitCode,
            TallyPopulation pop);
    }
}