using NatCruise.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface ITemplateDataservice : IDataservice
    {

        #region TreeDefaultValues

        IEnumerable<TreeDefaultValue> GetTreeDefaultValues();

        void AddTreeDefaultValue(TreeDefaultValue tdv);

        void UpsertTreeDefaultValue(TreeDefaultValue tdv);

        void DeleteTreeDefaultValue(TreeDefaultValue tdv);

        #endregion TreeDefaultValues

        #region StratumTemplate

        IEnumerable<StratumTemplate> GetStratumTemplates();

        void UpsertStratumTemplate(StratumTemplate st);

        void DeleteStratumTemplate(StratumTemplate st);

        #endregion StratumTemplate

        #region StratumTemplateTreeFieldSetup

        IEnumerable<StratumTemplateTreeFieldSetup> GetStratumTemplateTreeFieldSetups(string stratumTemplateName);

        void UpsertStratumTemplateTreeFieldSetup(StratumTemplateTreeFieldSetup stfs);

        void DeleteStratumTemplateTreeFieldSetup(StratumTemplateTreeFieldSetup stfs);

        #endregion StratumTemplateTreeFieldSetup

        #region StratumTemplateLogFieldSetup

        IEnumerable<StratumTemplateLogFieldSetup> GetStratumTemplateLogFieldSetups(string stratumTemplateName);

        void UpsertStratumTemplateLogFieldSetup(StratumTemplateLogFieldSetup stlfs);

        void DeleteStratumTemplateLogFieldSetup(StratumTemplateLogFieldSetup stlfs);

        #endregion StratumTemplateLogFieldSetup

        //#region StratumDefault

        //IEnumerable<StratumDefault> GetStratumDefaults();

        //IEnumerable<StratumDefault> GetTreeFieldSetupStratumDefaults();

        //void AddStratumDefault(StratumDefault std);

        //void UpdateStratumDefault(StratumDefault std);

        //#endregion StratumDefault

        //#region SampleGroupDefaults

        //IEnumerable<SampleGroupDefault> GetSampleGroupDefaults();

        //void AddSampleGroupDefault(SampleGroupDefault sgd);

        //void UpdateSampleGroupDefault(SampleGroupDefault sgd);

        //#endregion SampleGroupDefaults

        //#region TreeFieldSetupDefault

        //IEnumerable<TreeFieldSetupDefault> GetTreeFieldSetupDefaults(string stratumDefaultID);

        //void AddTreeFieldSetupDefault(TreeFieldSetupDefault tfsd);

        //void UpsertTreeFieldSetupDefault(TreeFieldSetupDefault tfsd);

        //void DeleteTreeFieldSetupDefault(TreeFieldSetupDefault tfsd);

        //#endregion TreeFieldSetupDefault

        //#region LogFieldSetupDefault

        //IEnumerable<LogFieldSetupDefault> GetLogFieldSetupDefaults();

        //IEnumerable<LogFieldSetupDefault> GetLogFieldSetupDefaults(string stratumDefaultID);

        //void AddLogFieldSetupDefault(LogFieldSetupDefault lfsd);

        //void UpsertLogFieldSetupDefault(LogFieldSetupDefault lfsd);

        //#endregion LogFieldSetupDefault

        #region Reports

        IEnumerable<Reports> GetReports();

        void AddReport(Reports report);

        void UpsertReport(Reports report);

        #endregion Reports

        #region VolumeEquation

        IEnumerable<VolumeEquation> GetVolumeEquations();

        void AddVolumeEquation(VolumeEquation ve);

        void UpsertVolumeEquation(VolumeEquation ve);

        #endregion VolumeEquation
    }
}