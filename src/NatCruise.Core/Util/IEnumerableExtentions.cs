using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NatCruise.Util
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

        public static Dictionary<TKey,IEnumerable<TValue>> ToCollectionDictionary<TKey, TValue>(this IEnumerable<TValue> @this, Func<TValue, TKey> keySelector)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }
            if (keySelector is null) { throw new ArgumentNullException(nameof(keySelector)); }

            var dict = new Dictionary<TKey, IEnumerable<TValue>>();

            foreach(var i in @this)
            {
                var key = keySelector(i);

                if(dict.ContainsKey(key) == false)
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