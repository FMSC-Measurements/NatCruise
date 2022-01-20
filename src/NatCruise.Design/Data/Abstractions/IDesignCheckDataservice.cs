using NatCruise.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Data
{
    public interface IDesignCheckDataservice : IDataservice
    {
        IEnumerable<DesignCheck> GetDesignChecks();
    }
}
