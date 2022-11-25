using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;

namespace NatCruise.Wpf.Controls
{
    public class GridHelper
    {
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached("ColumnDefinitions",
                                                typeof(string),
                                                typeof(Grid),
                                                new PropertyMetadata(default(string), OnColumnDefinitionsChanged));

        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            

            if (d is System.Windows.Controls.Grid source)
            {
                if (e.OldValue != null && e.OldValue.Equals(e.NewValue)) { return; }
                if (source.ColumnDefinitions?.Count > 0) { source.ColumnDefinitions.Clear(); }

                var value = (string)e.NewValue;
                var a = value.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var text in a)
                {
                    source.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition
                    {
                        Width = CreateGridLength(text),
                    });
                }
            }
        }

        public static string GetColumnDefinitions(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnDefinitionsProperty);
        }

        public static void SetColumnDefinitions(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnDefinitionsProperty, value);
        }

        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.RegisterAttached("RowDefinitions",
                                                typeof(string),
                                                typeof(Grid),
                                                new PropertyMetadata(default(string), OnRowDefinitionsChanged));

        private static void OnRowDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.Grid source)
            {
                if (e.OldValue != null && e.OldValue.Equals(e.NewValue)) { return; }
                if (source.RowDefinitions?.Count > 0) { source.RowDefinitions.Clear(); }

                var value = (string)e.NewValue;
                var a = value.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var (item, i) in a.Select((x, i) => (x, i)))
                {
                    source.RowDefinitions.Add(new System.Windows.Controls.RowDefinition
                    {
                        Height = CreateGridLength(item),
                    });
                }
            }
        }

        public static string GetRowDefinitions(DependencyObject obj)
        {
            return (string)obj.GetValue(RowDefinitionsProperty);
        }

        public static void SetRowDefinitions(DependencyObject obj, string value)
        {
            obj.SetValue(RowDefinitionsProperty, value);
        }

        public static GridLength CreateGridLength(string text)
        {
            text = text.Trim();
            if (text.Length == 0) throw new XamlParseException("Invalid grid length value");
            if (text.EndsWith("*"))
            {
                var starFactor = text.Length > 1 ? double.Parse(text.Substring(0, text.Length - 1)) : 1;
                return new GridLength(starFactor, GridUnitType.Star);
            }
            if(text.Equals("auto", StringComparison.OrdinalIgnoreCase))
            { return new GridLength(0D, GridUnitType.Auto); }
            else
            {
                return new GridLength(double.Parse(text), GridUnitType.Pixel);
            }
        }
    }
}
