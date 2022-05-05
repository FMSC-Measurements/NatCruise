using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITreeFieldValueDataservice : IDataservice
    {
        IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID);

        void UpdateTreeFieldValue(TreeFieldValue treeFieldValue);
    }
}