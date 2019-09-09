using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Util
{
    public static class IEnumerableExtentions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> @this)
        {
            if(@this is ObservableCollection<T> c) { return c; }

            return new ObservableCollection<T>(@this);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this)
        {
            if(@this is HashSet<T> c) { return c; }

            return new HashSet<T>(@this);
        }
    }
}
