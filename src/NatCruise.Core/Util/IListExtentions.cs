using System;
using System.Collections.Generic;

namespace NatCruise.Util
{
    public static class IListExtentions
    {
        public static TSource Search<TSource>(this IList<TSource> source, Func<TSource, bool> predicate, int start)
        {
            if (predicate == null) { throw new ArgumentNullException(nameof(predicate)); }
            var count = source.Count;
            if (start >= count || start < 0) { throw new ArgumentOutOfRangeException(nameof(start)); }

            for (var i = start; i < count; i++)
            {
                TSource item = source[i];
                if (predicate(item))
                {
                    return item;
                }
            }
            return default(TSource);
        }

        public static TSource ReverseSearch<TSource>(this IList<TSource> source, Func<TSource, bool> predicate, int start)
        {
            if (predicate == null) { throw new ArgumentNullException(nameof(predicate)); }
            var count = source.Count;
            if (start >= count || start < 0) { throw new ArgumentOutOfRangeException(nameof(start)); }

            for (var i = start; i >= 0; i--)
            {
                TSource item = source[i];
                if (predicate(item))
                {
                    return item;
                }
            }
            return default(TSource);
        }
    }
}