using CruiseDAL;
using NatCruise.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class TreeErrorDataservice : CruiseDataserviceBase, ITreeErrorDataservice
    {
        public TreeErrorDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TreeErrorDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<TreeError> GetTreeErrors(string treeID)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.TreeAuditRuleID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.IsResolved," +
                "te.Resolution, " +
                "te.ResolutionInitials " +
                "FROM TreeError AS te " +
                "WHERE te.TreeID = @p1;",
                new object[] { treeID }).ToArray();
        }

        public TreeError GetTreeError(string treeID, string treeAuditRuleID)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.TreeAuditRuleID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.IsResolved," +
                "te.Resolution, " +
                "te.ResolutionInitials " +
                "FROM TreeError AS te " +
                "WHERE te.TreeID = @p1 " +
                "AND te.TreeAuditRuleID = @p2;",
                new object[] { treeID, treeAuditRuleID }).FirstOrDefault();
        }

        public IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.Resolution " +
                "FROM TreeError AS te " +
                "JOIN Tree AS t USING (TreeID) " +
                "WHERE t.CuttingUnitCode = @p1 AND t.CruiseID = @p2;",
                new object[] { cuttingUnitCode, CruiseID }).ToArray();
        }

        public IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID)
        {
            return Database.Query<TreeError>(
@"SELECT
    te.TreeID,
    te.Field,
    te.Level,
    te.Message,
    te.IsResolved,
    te.Resolution
FROM TreeError AS te
JOIN Tree AS t USING (TreeID)
JOIN Plot AS p USING (CuttingUnitCode, CruiseID, PlotNumber)
WHERE p.PlotID = @p1;",
                new object[] { plotID }).ToArray();
        }

        public void SetTreeAuditResolution(string treeID, string treeAuditRuleID, string resolution, string initials)
        {
            Database.Execute("INSERT OR REPLACE INTO TreeAuditResolution " +
                "(TreeID, CruiseID, TreeAuditRuleID, Resolution, Initials)" +
                "VALUES" +
                "(@p1, @p2, @p3, @p4, @p5);"
                , treeID, CruiseID, treeAuditRuleID, resolution, initials);
        }

        public void ClearTreeAuditResolution(string treeID, string treeAuditRuleID)
        {
            Database.Execute("DELETE FROM TreeAuditResolution WHERE TreeID = @p1 AND TreeAuditRuleID = @p2"
                , treeID, treeAuditRuleID);
        }
    }
}