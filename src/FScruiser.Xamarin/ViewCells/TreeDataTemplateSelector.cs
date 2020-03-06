using FScruiser.Models;
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

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var tallyEntry = (TallyEntry)item;

            if(tallyEntry.EntryType == TallyLedger.EntryTypeValues.TALLY)
            {
                return (tallyEntry.TreeID != null) ? TreeItemTemplate : BasicTemplate;
            }
            else
            { return TallyEditTemplate; }
            
        }
    }
}
