using Prism.Common;
using System.Collections.Generic;

namespace NatCruise.Navigation
{
    public interface ITheRealInitialize
    {
        void Initialize(IDictionary<string, object> parameters);
    }
}