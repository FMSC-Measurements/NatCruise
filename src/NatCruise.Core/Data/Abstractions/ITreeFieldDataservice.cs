using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ITreeFieldDataservice : IDataservice
    {
        IEnumerable<string> GetTreeFieldsNames();

        IEnumerable<TreeField> GetTreeFields();

        void AddTreeField(TreeField field);

        void DeleteTreeField(string field);

        IEnumerable<TreeField> GetTreeFieldsByStratum(string stratumCode);

        IEnumerable<TreeField> GetNonPlotTreeFields(string unitCode);

        IEnumerable<TreeField> GetPlotTreeFields(string unitCode);

        IEnumerable<TreeField> GetTreeFieldsUsedInCruise();
    }
}