using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface IFieldSetupDataservice : IDataservice
    {
        IEnumerable<TreeFieldSetup> GetTreeFieldSetups(string stratumCode);
        IEnumerable<TreeFieldSetup> GetTreeFieldSetups(string stratumCode, string sampleGroupCode);
        void UpsertTreeFieldSetup(TreeFieldSetup tfs);
        void DeleteTreeFieldSetup(TreeFieldSetup tfs);

        //void SetTreeFieldsFromStratumDefault(string stratumCode, StratumDefault sd);

        void SetTreeFieldsFromStratumTemplate(string stratumCode, string stratumTemplateName);
    }
}