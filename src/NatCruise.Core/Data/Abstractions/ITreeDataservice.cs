using NatCruise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public interface ITreeDataservice : IDataservice
    {
        string InsertManualTree(string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = null,
            int treeCount = 1, int kpi = 0, bool stm = false);

        TreeEx GetTree(string treeID);

        IEnumerable<TreeEx> GetTreesByUnitCode(string unitCode);

        IEnumerable<TreeEx> GetPlotTreesByUnitCode(string unitCode);

        void UpdateTree(Tree tree);

        void UpdateTree(TreeEx tree);

        Task UpdateTreeAsync(TreeEx tree);

        void UpdateTreeRemarks(string treeID, string remarks);

        void DeleteTree(string treeID);

        int? GetTreeNumber(string treeID);

        bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null, string stratumCode = null);

        void UpdateTreeInitials(string treeID, string value);

        int GetTreeCount(string treeID);

        void UpdateTreeCount(string treeID, int count);
    }
}