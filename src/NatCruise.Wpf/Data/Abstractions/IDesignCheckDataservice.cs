using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IDesignCheckDataservice : IDataservice
    {
        IEnumerable<DesignCheck> GetDesignChecks();
    }
}