using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NatCruise.Util
{
    public static class IDictionaryExtentions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this.TryGetValue(key, out TValue value))
            { return value; }
            else { return default; }
        }

        public static Tvalue GetValue<Tvalue>(this IDictionary<string, object> @this, string key)
        {
            var value = @this[key];
            return ConvertValue<Tvalue>(value);
        }

        public static Tvalue GetValueOrDefault<Tvalue>(this IDictionary<string, object> @this, string key, Tvalue defaultValue = default)
        {
            if (@this.ContainsKey(key))
            {
                var value = @this[key];
                return ConvertValue<Tvalue>(value, defaultValue);
            }
            else
            {
                return defaultValue;
            }
        }

        public static void SetValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (@this.ContainsKey(key))
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

        

        private static Tvalue ConvertValue<Tvalue>(object inValue, Tvalue defaultValue = default)
        {
            if (inValue == null) return defaultValue;

            if(inValue is Tvalue) { return (Tvalue)inValue; }
            
            var targetType = typeof(Tvalue);
            if(targetType.IsEnum)
            {
                var inValueAsString = (inValue is string str) ? str : inValue.ToString();
                if (Enum.IsDefined(targetType, inValueAsString))
                {
                    return (Tvalue)Enum.Parse(targetType, inValueAsString);
                }
                else if(int.TryParse(inValueAsString, out int numVal))
                {
                    return (Tvalue)Enum.ToObject(targetType, numVal);
                }
            }

            return (Tvalue)Convert.ChangeType(inValue, targetType);
        }

    }
}