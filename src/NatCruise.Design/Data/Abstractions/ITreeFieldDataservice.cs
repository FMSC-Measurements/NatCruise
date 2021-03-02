using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface ITreeFieldDataservice : IDataservice
    {
        IEnumerable<string> GetTreeFields();

        void AddTreeField(TreeField field);

        void DeleteTreeField(string field);

        IEnumerable<TreeField> GetTreeFieldsByStratum(string stratumCode);
    }
}