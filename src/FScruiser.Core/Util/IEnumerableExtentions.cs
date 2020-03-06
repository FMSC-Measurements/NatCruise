using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FScruiser.Util
{
    public static class IEnumerableExtentions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> @this)
        {
            return @this ?? Enumerable.Empty<T>();
        }

        public static IEnumerable OrEmpty(this IEnumerable @this)
        {
            return @this ?? Enumerable.Empty<object>();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || e.Count() == 0;
        }

        public static int MaxOrDefault<T>(this IEnumerable<T> @this, Func<T, int> selector, int dVal = default(int))
        {
            if (@this.IsNullOrEmpty())
            { return dVal; }
            else
            { return @this.Max(selector); }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> @this)
        {
            return new ObservableCollection<T>(@this);
        }
    }
}