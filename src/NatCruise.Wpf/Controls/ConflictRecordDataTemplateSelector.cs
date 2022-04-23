using CruiseDAL.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Wpf.Controls
{
    public class ConflictRecordDataTemplateSelector : DataTemplateSelector
    {
        public ConflictRecordDataTemplateSelector()
        {
        }

        public DataTemplate Plot { get; set; }
        public DataTemplate Tree { get; set; }
        public DataTemplate Log { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            var dataContext = element.DataContext;

            var itemType = item.GetType().Name;
            switch(itemType)
            {
                case "Tree": return Tree;
                case "Plot": return Plot;
                case "Log": return Log;
                default: return null;
            }
        }
    }
}
