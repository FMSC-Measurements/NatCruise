using NatCruise.Cruise.Models;
using NatCruise.Models;
using Xamarin.Forms;

namespace FScruiser.XF.Controls
{
    public class TallyEntryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TallyEditTemplate { get; set; }
        public DataTemplate TreeItemTemplate { get; set; }
        public DataTemplate BasicTemplate { get; set; }
        public DataTemplate ManualTreeTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is TallyEntry tallyEntry)
            {
                if (tallyEntry.EntryType == TallyLedgerEntryTypeValues.TALLY)
                {
                    return (tallyEntry.TreeID != null) ? TreeItemTemplate : BasicTemplate;
                }
                else if (tallyEntry.EntryType == TallyLedgerEntryTypeValues.MANUAL_TREE)
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