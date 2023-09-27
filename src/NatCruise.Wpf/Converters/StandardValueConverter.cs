using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NatCruise.Wpf.Converters
{
    public class StandardValueConverter : MarkupExtension, IValueConverter
    {
        private Type _targetTypeInternal;

        public StandardValueConverter()
        {
        }

        public Type TargetType { get; set; }

        public bool Nullable { get; set; }

        public object FallBackValue { get; set; }

        protected Type TargetTypeInternal
        {
            get
            {
                return _targetTypeInternal ??= (Nullable && TargetType != null) ? typeof(Nullable<>).MakeGenericType(TargetType)
                    : TargetType;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            targetType = TargetTypeInternal ?? targetType;

            if (targetType == typeof(object)) return value;
            var converter = TypeDescriptor.GetConverter(targetType);
            return converter.ConvertFrom(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}