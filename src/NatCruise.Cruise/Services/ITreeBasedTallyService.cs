using NatCruise.Cruise.Models;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface ITreeBasedTallyService
    {
        Task<TallyEntry> TallyAsync(string unitCode,
            TallyPopulation pop);
    }
}