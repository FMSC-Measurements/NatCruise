using System.Collections.Generic;

namespace FScruiser.Util
{
    public static class IDictionaryExtentions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this.TryGetValue(key, out TValue value))
            { return value; }
            else { return default(TValue); }
        }

        public static Tvalue GetValueOrDefault<Tvalue>(this IDictionary<string, object> @this, string key, Tvalue defaultValue)
        {
            if(@this.ContainsKey(key))
            {
                return (Tvalue)@this[key];
            }
            else
            {
                return defaultValue;
            }
        }

        public static void SetValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if(@this.ContainsKey(key))
            {
                @this[key] = value;
            }
            else
            {
                @this.Add(key, value);
            }
        }

        public static void ClearValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            @this.Remove(key);
        }
    }
}