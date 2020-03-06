using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.Commands
{
    public class MessagingCenterCommand : ICommand
    {
        readonly Func<object, bool> _canExecute;

        public string Message { get; set; }

        public event EventHandler CanExecuteChanged;

        //public MessagingCenterCommand()
        //{
        //}

        public MessagingCenterCommand(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public MessagingCenterCommand(string message, Func<object, bool> canExecute)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public void ChangeCanExecute()
        {
            EventHandler changed = CanExecuteChanged;
            changed?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);

            return true;
        }

        public virtual void Execute(object parameter)
        {
            MessagingCenter.Send<object, object>((object)null, Message, parameter);
        }
    }
}
