using NatCruise.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class TallyEntryCanUntallyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Binding.DoNothing;

            var te = (TallyEntry)value;
            if (te.IsDeleted) return false;

            var entryType = te.EntryType;
            return entryType switch
            {
                TallyLedgerEntryTypeValues.TALLY => true,
                TallyLedgerEntryTypeValues.MANUAL_TREE => false,
                TallyLedgerEntryTypeValues.TREECOUNT_EDIT => false,
                _ => false,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}