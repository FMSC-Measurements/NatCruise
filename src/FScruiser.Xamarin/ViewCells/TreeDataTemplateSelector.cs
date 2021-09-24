using NatCruise.Cruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.ViewCells
{
    public class TreeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TallyEditTemplate { get; set; }
        public DataTemplate TreeItemTemplate { get; set; }
        public DataTemplate BasicTemplate { get; set; }
        public DataTemplate ManualTreeTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is TallyEntry tallyEntry)
            {
                if (tallyEntry.EntryType == TallyLedger.EntryTypeValues.TALLY)
                {
                    return (tallyEntry.TreeID != null) ? TreeItemTemplate : BasicTemplate;
                }
                else if(tallyEntry.EntryType == TallyLedger.EntryTypeValues.MANUAL_TREE)
                {
                    return ManualTreeTemplate;
                }
                else
                { return TallyEditTemplate; }
            }
            else
            { return null; }
        }
    }
}
