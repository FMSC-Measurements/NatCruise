using NatCruise.Cruise.Models;
using NatCruise.Models;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface ITreeBasedTallyService
    {
        Task<TallyEntry> TallyAsync(string unitCode,
            TallyPopulation pop);
    }
}