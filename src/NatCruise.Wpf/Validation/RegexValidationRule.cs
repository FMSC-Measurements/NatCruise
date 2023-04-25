using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace NatCruise.Wpf.Validation
{
    public class RegexValidationRule : ValidationRule
    {
        public RegexValidationRule()
        {
            base.ValidationStep = ValidationStep.RawProposedValue;
        }

        public string Match { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var sValue = (string)value;

            if (Regex.IsMatch(sValue, Match, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "Invalid Value");
            }
        }
    }
}