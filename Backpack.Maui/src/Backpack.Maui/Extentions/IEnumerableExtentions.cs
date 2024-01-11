using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack.Maui.Extensions
{
    internal static class IEnumerableExtentions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? @this)
        {
            return @this ?? Enumerable.Empty<T>();
        }

        public static IEnumerable OrEmpty(this IEnumerable? @this)
        {
            return @this ?? Enumerable.Empty<object>();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? e)
        {
            if (e is string s) return String.IsNullOrEmpty(s);

            return e == null || !e.Any();
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            var i = 0;
            foreach (T element in enumerable)
            {
                if (Equals(element, item))
                    return i;

                i++;
            }

            return -1;
        }
    }
}
