using NatCruise.Data;
using NatCruise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public interface ITallyDataservice : IDataservice
    {
        TallyEntry GetTallyEntry(string tallyLedgerID);

        IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber);

        //IEnumerable<TallyEntry> GetTallyEntriesByUnitCodeIncludeUntallied(string unitCode);

        Task<TallyEntry> InsertTallyActionAsync(TallyAction tallyAction);

        TallyEntry InsertTallyAction(TallyAction atn);

        void DeleteTallyEntry(string tallyLedgerID);

        void RefreshErrorsAndWarnings(TallyEntry tallyEntry);
    }
}