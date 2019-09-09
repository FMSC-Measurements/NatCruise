using Prism.Services.Dialogs;
using System.Runtime.CompilerServices;

namespace NatCruise.Wpf.Navigation
{
    public class NotificationDialogParamiters : DialogParameters
    {
        public string Title
        {
            get => GetValueInternal<string>(nameof(Title));
            set => SetValue(value);
        }

        public string Message
        {
            get => GetValueInternal<string>(nameof(Message));
            set => SetValue(value);
        }

        public string OK
        {
            get => GetValueInternal<string>(nameof(OK));
            set => SetValue(value);
        }

        public string Cancel
        {
            get => GetValueInternal<string>(nameof(Cancel));
            set => SetValue(value);
        }

        protected TResult GetValueInternal<TResult>([CallerMemberName] string name = null)
        {
            return base.GetValue<TResult>(name);
        }

        protected void SetValue(object value, [CallerMemberName] string name = null)
        {
            base.Add(name, value);
        }
    }
}