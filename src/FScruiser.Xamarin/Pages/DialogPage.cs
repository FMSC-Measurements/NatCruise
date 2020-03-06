using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Pages
{
    public abstract class DialogPage : ContentPage
    {
        private TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

        public DialogResult DialogResult { get; protected set; } = DialogResult.Cancel;

        public event EventHandler<object> OnClosed;

        public DialogPage()
        {
        }

        protected void RaiseOnClosed(object output)
        {
            OnClosed?.Invoke(this, output);
        }

        protected abstract bool OnClosing(DialogResult result, out object output);

        protected bool Close(DialogResult result)
        {
            DialogResult = result;
            if (OnClosing(result, out var output))
            {
                RaiseOnClosed(output);
                return base.OnBackButtonPressed();
            }
            else
            {
                return false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return Close(DialogResult.Cancel);
        }

        public Task<object> ShowDialog(INavigation navigation)
        {
            var myTcs = new TaskCompletionSource<object>();

            if (System.Threading.Interlocked.CompareExchange(ref _tcs, myTcs, null) != null)//if _tcs == null then _tcs = newTcs; return origianl value of _tcs
            {
                throw new InvalidOperationException($"ShowDialog can only be called once at a time");
            }

            void handelClose(object sender, object output)
            {
                var tcs = System.Threading.Interlocked.Exchange(ref _tcs, null);//_tcs = null; return original value of _tcs

                OnClosed -= handelClose;

                if (this.DialogResult == DialogResult.Cancel)
                {
                    tcs?.SetResult(null);
                }
                else
                {
                    tcs?.SetResult(output);
                }
            }

            OnClosed += handelClose;

            navigation.PushModalAsync(this);

            return myTcs.Task;
        }
    }
}