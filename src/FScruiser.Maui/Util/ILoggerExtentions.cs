using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace FScruiser.Maui.Util
{
    public static class ILoggerExtentions
    {
        public static void LogMethodCall<T>(this ILogger<T> @this, [CallerMemberName] string callingMethod = null)
        {
            @this.LogTrace("Called {CallingMethod}", callingMethod);
        }
    }
}