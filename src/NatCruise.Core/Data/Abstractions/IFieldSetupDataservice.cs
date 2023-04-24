using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IFieldSetupDataservice : IDataservice
    {
        #region TreeFieldSetups

        IEnumerable<TreeFieldSetup> GetTreeFieldSetups(string stratumCode);

        IEnumerable<TreeFieldSetup> GetTreeFieldSetups(string stratumCode, string sampleGroupCode);

        void UpsertTreeFieldSetup(TreeFieldSetup tfs);

        void DeleteTreeFieldSetup(TreeFieldSetup tfs);

        //void SetTreeFieldsFromStratumDefault(string stratumCode, StratumDefault sd);

        void SetTreeFieldsFromStratumTemplate(string stratumCode, string stratumTemplateName);

        #endregion TreeFieldSetups

        #region LogFieldSetup

        IEnumerable<LogFieldSetup> GetLogFieldSetupsByTreeID(string tree_guid);

        IEnumerable<LogFieldSetup> GetLogFieldSetups(string stratumCode);

        void UpsertLogFieldSetup(LogFieldSetup lfs);

        void DeleteLogFieldSetup(LogFieldSetup lfs);

        void SetLogFieldsFromStratumTemplate(string stratumCode, string stratumTemplateName);

        IEnumerable<LogFieldSetup> GetLogFieldSetupsByCruise();

        #endregion LogFieldSetup
    }
}