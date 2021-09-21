using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;

namespace NatCruise.Cruise.Data
{
    public interface ITreeFieldDataservice : IDataservice
    {
        public IEnumerable<TreeField> GetNonPlotTreeFields(string unitCode);

        public IEnumerable<TreeField> GetPlotTreeFields(string unitCode);
    }
}