using Prism.Navigation;

namespace FScruiser.XF.Util
{
    public static class NavigationParametersExtentions
    {
        public static T GetValueOrDefault<T>(this INavigationParameters @this, string key)
        {
            if (@this.TryGetValue<T>(key, out var value))
            {
                return value;
            }
            else
            {
                return default(T);
            }
        }

        public static T GetValueOrDefault<T>(this INavigationParameters @this, string key, T defaultValue)
        {
            if (@this.TryGetValue<T>(key, out var value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}