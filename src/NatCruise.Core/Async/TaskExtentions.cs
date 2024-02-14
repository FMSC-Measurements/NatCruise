using NatCruise.Services;
using System;
using System.Threading.Tasks;

namespace NatCruise.Async
{
    //TODO update TaskExtentions to use Microsoft.Extensions.Logging services
    public static class TaskExtentions
    {
        public static ILoggingService LoggingService { get; set; }

        public static Action<Exception> OnException { get; set; }

        public static bool RethrowExceptions { get; set; }

        public static void FireAndForget(this Task @this, string failMessage = null)
        {
            @this.ContinueWith(HandelTaskException, failMessage, TaskContinuationOptions.OnlyOnFaulted);
        }

        private static void HandelTaskException(Task t, object state)
        {
            var exception = t.Exception;
            if (exception is null) { return; }
            var message = state as string;

            OnException?.Invoke(exception);
            foreach (var ex in exception.InnerExceptions)
            {
                LoggingService.LogException("TaskException", message, ex);
            }

            if (RethrowExceptions)
            { throw exception; }
        }
    }
}