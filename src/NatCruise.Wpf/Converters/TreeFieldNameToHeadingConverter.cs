using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class TreeFieldNameToHeadingConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TreeFieldsProperty = DependencyProperty.Register(
            nameof(TreeFields),
            typeof(IEnumerable<TreeField>),
            typeof(TreeFieldNameToHeadingConverter),
            new PropertyMetadata(OnTreeFieldChanged));

        private static void OnTreeFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newVal = e.NewValue as IEnumerable<TreeField>;
            var oldVal = e.OldValue as IEnumerable<TreeField>;
            if(newVal == oldVal) { return; }

            ((TreeFieldNameToHeadingConverter)d).NameMap = newVal?.ToDictionary(x => x.Field);
        }

        public IDictionary<string, TreeField> NameMap { get; set; }

        public IEnumerable<TreeField> TreeFields
        {
            get => (IEnumerable<TreeField>)GetValue(TreeFieldsProperty);
            set
            {
                //if (object.ReferenceEquals(GetValue(TreeFieldsProperty), value)) { return; }

                SetValue(TreeFieldsProperty, value);
                //_nameMap = value?.ToDictionary(x => x.Field);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nameMap = NameMap;
            if (value is string sValue && nameMap != null)
            {
                var treeField = nameMap[sValue];
                return treeField.Heading ?? treeField.DefaultHeading;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}