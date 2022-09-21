using Prism.Common;
using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public interface ITheRealInitializeAsync
    {
        Task InitializeAsync(IParameters parameters);
    }
}