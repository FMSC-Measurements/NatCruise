using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.ViewCells
{
    public class TallyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ClickerTallyTemplate { get; set; }

        public DataTemplate TallyTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return TallyTemplate;

            //var tallyPop = (TallyPopulation)item;

            //return (tallyPop.IsClickerTally) ? ClickerTallyTemplate : TallyTemplate;
        }
    }
}
