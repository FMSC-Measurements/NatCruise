using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public interface IFieldDataListViewModel
    {
        //HACK when switching between tabs I was refreshing the whole tab, this was to solve issues where changes in one tab could affect other tabs
        // such as deleting a plot could delete the trees within. But this causes the column order and sorting to get reset every time the user
        // switches tabs. A partial solution was to just reload the data when switching tabs. But a better solution is needed.
        // ideally we have a way for the tabs to communicate with each other, so that they can be notified when data changes in another tab
        // and know how to refresh the data approreately.
        void RefreshData();
    }
}
