using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITreeAuditRuleDataservice : IDataservice
    {
        #region TreeAuditRules

        IEnumerable<TreeAuditRule> GetTreeAuditRules();

        IEnumerable<TreeAuditRule> GetTreeAuditRules(string species, string prod, string livedead);

        void AddTreeAuditRule(TreeAuditRule tar);

        void UpsertTreeAuditRule(TreeAuditRule tar);

        void DeleteTreeAuditRule(TreeAuditRule tar);

        #endregion TreeAuditRules

        #region TreeAuditRules

        IEnumerable<TreeAuditRuleSelector> GetRuleSelectors();

        IEnumerable<TreeAuditRuleSelector> GetRuleSelectors(string tarID);

        void AddRuleSelector(TreeAuditRuleSelector tars);

        void DeleteRuleSelector(TreeAuditRuleSelector tars);

        #endregion TreeAuditRules
    }
}