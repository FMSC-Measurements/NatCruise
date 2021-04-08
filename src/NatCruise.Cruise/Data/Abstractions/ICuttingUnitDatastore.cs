﻿using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;

namespace NatCruise.Cruise.Services
{
    public interface ICuttingUnitDatastore : IPlotDatastore, ITreeDatastore, ILogDatastore, IDataservice
    {
        CuttingUnit_Ex GetUnit(string code);

        IEnumerable<CuttingUnit_Ex> GetUnits();

        void UpdateCuttingUnit(CuttingUnit_Ex cuttingUnit);

        #region stratra

        string GetCruiseMethod(string stratumCode);

        IEnumerable<string> GetStratumCodesByUnit(string unitCode);

        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode);

        #endregion stratra

        #region sampleGroups

        IEnumerable<string> GetSampleGroupCodes(string stratumCode);

        SampleGroup GetSampleGroup(string stratumCode, string sgCode);

        //IEnumerable<SampleGroupProxy> GetSampleGroupProxiesByUnit(string unitCode);

        IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode);

        SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode);

        #endregion sampleGroups

        IEnumerable<SubPopulation> GetSubPopulations(string stratumCode, string sampleGroupCode);

        //IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        //TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        #region validation

        IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode);

        IEnumerable<PlotError> GetPlotErrorsByUnit(string cuttingUnitCode);

        //IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead);

        #endregion validation

        #region tree

        TreeStub GetTreeStub(string tree_GUID);

        IEnumerable<Tree> GetTreesByUnitCode(string unitCode);

        IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode);

        #endregion tree

        #region Tally Entries

        //TallyEntry GetTallyEntry(string tallyLedgerID);
        //IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        //IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber);

        //TallyEntry InsertTallyAction(TallyAction entry);

        //Task<TallyEntry> InsertTallyActionAsync(TallyAction tallyAction);

        //void InsertTallyLedger(TallyLedger tallyLedger);

        //void DeleteTallyEntry(string tallyLedgerID);

        #endregion Tally Entries

        void LogMessage(string message, string level);
    }
}