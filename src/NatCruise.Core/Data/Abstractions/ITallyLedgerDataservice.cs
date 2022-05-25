using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITallyLedgerDataservice : IDataservice
    {
        void InsertTallyLedger(TallyLedger tallyLedger);
        IEnumerable<TallyLedger> GetTallyLedgers(string cuttingUnitCode = null, string stratumCode = null, string sampleGroupCode = null, string speciesCode = null, string liveDead = null);
    }
}