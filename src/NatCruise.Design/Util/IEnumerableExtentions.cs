using System.Collections.Generic;

namespace NatCruise.Design.Util
{
    public static class IEnumerableExtentions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this)
        {
            if (@this is HashSet<T> c) { return c; }

            return new HashSet<T>(@this);
        }
    }
}