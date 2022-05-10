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

        public IEnumerable<TreeField> GetNonPlotTreeFields(string unitCode);

        public IEnumerable<TreeField> GetPlotTreeFields(string unitCode);
    }
}