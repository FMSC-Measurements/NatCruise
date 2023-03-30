using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ILogFieldDataservice : IDataservice
    {
        IEnumerable<LogField> GetLogFields();

        void UpdateLogField(LogField lf);
    }
}