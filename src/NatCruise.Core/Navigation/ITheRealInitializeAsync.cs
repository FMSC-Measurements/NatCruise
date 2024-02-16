using Prism.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public interface ITheRealInitializeAsync
    {
        Task InitializeAsync(IDictionary<string, object> parameters);
    }
}