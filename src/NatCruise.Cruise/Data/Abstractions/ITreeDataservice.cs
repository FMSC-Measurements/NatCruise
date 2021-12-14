using NatCruise.Cruise.Models;
using NatCruise.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Data
{
    public interface ITreeDataservice : IDataservice
    {
        string InsertManualTree(string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = "L",
            int treeCount = 1, int kpi = 0, bool stm = false);

        Tree_Ex GetTree(string treeID);

        IEnumerable<Tree_Ex> GetTreesByUnitCode(string unitCode);

        TreeStub GetTreeStub(string treeID);

        IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode);

        IEnumerable<TreeStub> GetPlotTreeStubsByUnitCode(string unitCode);

        IEnumerable<Tree_Ex> GetPlotTreesByUnitCode(string unitCode);

        IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID);

        void UpdateTree(Tree tree);

        void UpdateTree(Tree_Ex tree);

        void UpsertTreeMeasurments(TreeMeasurment mes);

        Task UpdateTreeAsync(Tree_Ex tree);

        void UpdateTreeRemarks(string treeID, string remarks);

        void UpdateTreeFieldValue(TreeFieldValue treeFieldValue);

        void DeleteTree(string treeGuid);

        int? GetTreeNumber(string treeID);

        bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null, string stratumCode = null);

        void UpdateTreeInitials(string treeGuid, string value);

        IEnumerable<TreeError> GetTreeErrors(string treeID);

        TreeError GetTreeError(string treeID, string treeAuditRuleID);

        void SetTreeAuditResolution(string treeID, string treeAuditRuleID, string resolution, string initials);

        void ClearTreeAuditResolution(string treeID, string treeAuditRuleID);
        int GetTreeCount(string treeID);

        void UpdateTreeCount(string treeID, int count);
    }
}