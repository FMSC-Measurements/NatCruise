using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITreeDefaultValueDataservice : IDataservice
    {
        IEnumerable<TreeDefaultValue> GetTreeDefaultValues();

        void AddTreeDefaultValue(TreeDefaultValue tdv);

        void UpsertTreeDefaultValue(TreeDefaultValue tdv);

        void DeleteTreeDefaultValue(TreeDefaultValue tdv);
    }
}