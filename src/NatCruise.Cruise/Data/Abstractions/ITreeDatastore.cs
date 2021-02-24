using NatCruise.Cruise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface ITreeDatastore
    {
        string CreateMeasureTree(string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = "L",
            int treeCount = 1, int kpi = 0, bool stm = false);

        Tree_Ex GetTree(string treeID);

        IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID);

        void UpdateTree(Tree tree);

        void UpdateTree(Tree_Ex tree);

        void UpsertTreeMeasurments(TreeMeasurment mes);

        Task UpdateTreeAsync(Tree_Ex tree);

        void UpdateTreeRemarks(string treeID, string remarks);

        void UpdateTreeFieldValue(TreeFieldValue treeFieldValue);

        void DeleteTree(string treeGuid);

        #region util

        int? GetTreeNumber(string treeID);

        IEnumerable<TreeError> GetTreeErrors(string treeID);

        TreeError GetTreeError(string treeID, string treeAuditRuleID);

        void SetTreeAuditResolution(string treeID, string treeAuditRuleID, string resolution, string initials);

        void ClearTreeAuditResolution(string treeID, string treeAuditRuleID);

        bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null);

        void UpdateTreeInitials(string treeGuid, string value);

        #endregion util
    }
}