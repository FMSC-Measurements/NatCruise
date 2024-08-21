using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NatCruise.Wpf.Converters.Multi
{
    public class AnyToVisabilityMultiConverter : AnyConverter
    {
        public AnyToVisabilityMultiConverter()
        {
            TrueObject = Visibility.Visible;
            FalseObject = Visibility.Collapsed;
        }
    }
}
