using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ILogErrorDataservice : IDataservice
    {
        IEnumerable<LogError> GetLogErrorsByLog(string logID);

        IEnumerable<LogError> GetLogErrorsByTree(string treeID);
    }
}