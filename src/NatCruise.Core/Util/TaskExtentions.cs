using System;
using System.Threading.Tasks;

namespace NatCruise.Util
{
    // To help resolve SonarQube 'Bug' issues I replaced all uses of HanleFireAndForget with .ConfigureAwait(false)
    // At the time I wasn't actualy using the exception catching features of this class, so it was easier to just not use it
    // Maybe it will be useful at some later time

    //public static class TaskExtentions
    //{
    //    public static Action<Exception> OnException { get; set; }

    //    public static bool RethrowExceptions { get; set; }

    //    public static void FireAndForget(this Task @this, bool continueOnCapturedContext = false, Action<Exception> onException = null)
    //    => HandleFireAndForget(@this, continueOnCapturedContext, onException);

    //    private static async void HandleFireAndForget(Task task, bool continueOnCapturedContext, Action<Exception> onException)
    //    {
    //        try
    //        {
    //            await task.ConfigureAwait(continueOnCapturedContext);
    //        }
    //        catch (Exception ex) when (OnException != null || onException != null)
    //        {
    //            OnException?.Invoke(ex);
    //            onException?.Invoke(ex);

    //            if (RethrowExceptions)
    //                throw;
    //        }
    //    }
    //}
}