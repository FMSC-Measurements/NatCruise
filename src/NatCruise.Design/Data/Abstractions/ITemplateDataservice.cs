using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface ITemplateDataservice : IDataservice
    {
        #region species
        IEnumerable<Species> GetSpecies();

        void AddSpecies(Species sp);

        void UpsertSpecies(Species sp);

        void DeleteSpecies(string speciesCode);

        IEnumerable<string> GetSpeciesCodes();

        void AddSpeciesCode(string speciesCode);
        #endregion

        #region TreeDefaultValues
        IEnumerable<TreeDefaultValue> GetTreeDefaultValues();

        void AddTreeDefaultValue(TreeDefaultValue tdv);

        void UpsertTreeDefaultValue(TreeDefaultValue tdv);

        void DeleteTreeDefaultValue(TreeDefaultValue tdv);
        #endregion

        #region TreeAuditRules
        IEnumerable<TreeAuditRule> GetTreeAuditRules();

        IEnumerable<TreeAuditRule> GetTreeAuditRules(string species, string prod, string livedead);

        void AddTreeAuditRule(TreeAuditRule tar);

        void UpsertTreeAuditRule(TreeAuditRule tar);

        void DeleteTreeAuditRule(TreeAuditRule tar);
        #endregion

        #region TreeAuditRules
        IEnumerable<TreeAuditRuleSelector> GetRuleSelectors();

        IEnumerable<TreeAuditRuleSelector> GetRuleSelectors(string tarID);

        void AddRuleSelector(TreeAuditRuleSelector tars);

        void DeleteRuleSelector(TreeAuditRuleSelector tars);
        #endregion

        #region TreeFields
        IEnumerable<TreeField> GetTreeFields();

        void UpdateTreeField(TreeField treeField);
        #endregion

        #region LogFields
        IEnumerable<LogField> GetLogFields();

        void UpdateLogField(LogField lf);
        #endregion

        #region StratumDefault
        IEnumerable<StratumDefault> GetStratumDefaults();

        IEnumerable<StratumDefault> GetTreeFieldSetupStratumDefaults();

        void AddStratumDefault(StratumDefault std);

        void UpdateStratumDefault(StratumDefault std);
        #endregion

        #region SampleGroupDefaults
        IEnumerable<SampleGroupDefault> GetSampleGroupDefaults();

        void AddSampleGroupDefault(SampleGroupDefault sgd);

        void UpdateSampleGroupDefault(SampleGroupDefault sgd);

        #endregion

        #region TreeFieldSetupDefault
        IEnumerable<TreeFieldSetupDefault> GetTreeFieldSetupDefaults(string stratumDefaultID);

        void AddTreeFieldSetupDefault(TreeFieldSetupDefault tfsd);

        void UpsertTreeFieldSetupDefault(TreeFieldSetupDefault tfsd);

        void DeleteTreeFieldSetupDefault(TreeFieldSetupDefault tfsd);
        #endregion

        #region LogFieldSetupDefault
        IEnumerable<LogFieldSetupDefault> GetLogFieldSetupDefaults();

        IEnumerable<LogFieldSetupDefault> GetLogFieldSetupDefaults(string stratumDefaultID);

        void AddLogFieldSetupDefault(LogFieldSetupDefault lfsd);

        void UpsertLogFieldSetupDefault(LogFieldSetupDefault lfsd);
        #endregion

        #region Reports
        IEnumerable<Reports> GetReports();

        void AddReport(Reports report);

        void UpsertReport(Reports report);
        #endregion

        #region VolumeEquation
        IEnumerable<VolumeEquation> GetVolumeEquations();

        void AddVolumeEquation(VolumeEquation ve);

        void UpsertVolumeEquation(VolumeEquation ve);
        #endregion
    }
}