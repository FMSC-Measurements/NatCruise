using NatCruise.Models;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Wpf.Controls
{
    public class TreeFieldValueDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RealTemplate { get; set; }
        public DataTemplate IntTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tfv = (TreeFieldValue)item;

            switch (tfv.DBType)
            {
                case "REAL":
                    {
                        return RealTemplate;
                    }
                case "INT":
                case "INTEGER":
                    {
                        return IntTemplate;
                    }
                case "TEXT":
                    {
                        return TextTemplate;
                    }
                case "BOOL":
                case "BOOLEAN":
                    {
                        return BoolTemplate;
                    }
            }

            return base.SelectTemplate(item, container);
        }
    }
}