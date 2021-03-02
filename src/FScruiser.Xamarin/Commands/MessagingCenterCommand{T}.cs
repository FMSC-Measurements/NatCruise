using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Commands
{
    public class MessagingCenterCommand<T> : MessagingCenterCommand
    {
        //public MessagingCenterCommand()
        //{
        //}

        public MessagingCenterCommand(string message) : base(message, IsValidParameter)
        {
        }

        public MessagingCenterCommand(string message, Func<object, bool> canExecute) : base(message, (o) => IsValidParameter(o) && canExecute((T)o))
        {
        }

        public override void Execute(object parameter)
        {
            if (IsValidParameter(parameter))
            {
                MessagingCenter.Send<object, T>((object)null, Message, (T)parameter);
            }
        }

        static bool IsValidParameter(object o)
        {
            if (o != null)
            {
                // The parameter isn't null, so we don't have to worry whether null is a valid option
                return o is T;
            }

            var t = typeof(T);

            // The parameter is null. Is T Nullable?
            if (Nullable.GetUnderlyingType(t) != null)
            {
                return true;
            }

            // Not a Nullable, if it's a value type then null is not valid
            return !t.GetTypeInfo().IsValueType;
        }
    }
}
