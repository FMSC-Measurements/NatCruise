using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NatCruise.Util
{
    public enum ToDictionaryConflictOption
    { ThrowOnConflict, Ignore, Replace };

    public static class IEnumerableExtentions
    {
        private class IdentityFunction<TElement>
        {
            public static Func<TElement, TElement> Instance
            {
                get { return x => x; }
            }
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> @this)
        {
            return @this ?? Enumerable.Empty<T>();
        }

        public static IEnumerable OrEmpty(this IEnumerable @this)
        {
            return @this ?? Enumerable.Empty<object>();
        }

        public static bool AnyAndNotNull<T>(this IEnumerable<T> @this)
        {
            return @this != null && @this.Any();
        }

        public static bool AnyAndNotNull<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            return @this != null && @this.Any(predicate);
        }

        public static int MaxOrDefault<T>(this IEnumerable<T> @this, Func<T, int> selector, int dVal = default(int))
        {
            if (@this.AnyAndNotNull())
            { return @this.Max(selector); }
            else
            { return dVal; }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || e.Any() == false;
        }

        public static Dictionary<TKey, IEnumerable<TValue>> ToCollectionDictionary<TKey, TValue>(this IEnumerable<TValue> @this, Func<TValue, TKey> keySelector)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }
            if (keySelector is null) { throw new ArgumentNullException(nameof(keySelector)); }

            var dict = new Dictionary<TKey, IEnumerable<TValue>>();

            foreach (var i in @this)
            {
                var key = keySelector(i);

                if (dict.ContainsKey(key) == false)
                {
                    var list = new List<TValue>();
                    list.Add(i);
                    dict.Add(key, list);
                }
                else
                {
                    var list = (List<TValue>)dict[key];
                    list.Add(i);
                }
            }
            return dict;
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, ToDictionaryConflictOption conflictOption)
        {
            return ToDictionary(source, keySelector, IdentityFunction<TSource>.Instance, comparer, conflictOption);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, ToDictionaryConflictOption conflictOption)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            Dictionary<TKey, TElement> d = new Dictionary<TKey, TElement>(comparer);
            foreach (TSource element in source)
            {
                var key = keySelector(element);
                if (conflictOption != ToDictionaryConflictOption.ThrowOnConflict || d.ContainsKey(key))
                {
                    if (conflictOption == ToDictionaryConflictOption.Replace) { continue; }
                    { d[key] = elementSelector(element); }
                }
                else
                {
                    d.Add(key, elementSelector(element));
                }
            }
            return d;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> @this)
        {
            if (@this is ObservableCollection<T> c) { return c; }

            return new ObservableCollection<T>(@this);
        }

        // ToHashSet requires in netcore 2.0, net472 or netstd21
        //#if !NETCOREAPP3_1

        //        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this)
        //        {
        //            if (@this is HashSet<T> c) { return c; }

        //            return new HashSet<T>(@this);
        //        }

        //#endif
    }
}