using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITreeErrorDataservice : IDataservice
    {
        IEnumerable<TreeError> GetTreeErrors(string treeID);

        TreeError GetTreeError(string treeID, string treeAuditRuleID);

        IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode);

        IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID);

        void SetTreeAuditResolution(string treeID, string treeAuditRuleID, string resolution, string initials);

        void ClearTreeAuditResolution(string treeID, string treeAuditRuleID);
    }
}