using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IPlotErrorDataservice : IDataservice
    {
        IEnumerable<PlotError> GetPlotErrors(string unit, int plotNumber);
    }
}