using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.Util
{
    public static class TaskExtentions
    {
        public static Action<Exception> OnException { get; set; }

        public static bool RethrowExceptions { get; set; }

        public static void FireAndForget(this Task @this, bool continueOnCapturedContext = false, Action<Exception> onException = null)
        => HandleFireAndForget(@this, continueOnCapturedContext, onException);

        private static async void HandleFireAndForget(Task task, bool continueOnCapturedContext, Action<Exception> onException)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex) when (OnException != null || onException != null)
            {
                OnException?.Invoke(ex);
                onException?.Invoke(ex);

                if (RethrowExceptions)
                    throw;
            }
        }
    }
}
