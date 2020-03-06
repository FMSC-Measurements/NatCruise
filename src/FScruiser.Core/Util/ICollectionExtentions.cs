using System.Collections.Generic;

namespace FScruiser.Util
{
    public static class ICollectionExtentions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                collection.Add(item);
            }
        }
    }
}