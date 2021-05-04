using System;

namespace NatCruise.Wpf.Util
{
    public static class StringExtentions
    {
        public static string Prepend(this String str, string value)
        {
            if (str != null && value != null)
            {
                return value + str;
            }
            else { return str; }
        }
    }
}