using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Data
{
    public interface ITallyDataservice : IDataservice
    {
        TallyEntry GetTallyEntry(string tallyLedgerID);

        IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber);

        void InsertTallyLedger(TallyLedger tallyLedger);

        Task<TallyEntry> InsertTallyActionAsync(TallyAction tallyAction);

        TallyEntry InsertTallyAction(TallyAction atn);

        void DeleteTallyEntry(string tallyLedgerID);
    }
}