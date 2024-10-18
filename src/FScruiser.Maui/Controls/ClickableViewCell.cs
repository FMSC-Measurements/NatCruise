using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FScruiser.Maui.Controls
{
    public class ClickableViewCell : ViewCell
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ClickableViewCell), default(ICommand),
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                var cell = (ClickableViewCell)bindable;
                var oldcommand = (ICommand)oldvalue;
                if (oldcommand != null)
                    oldcommand.CanExecuteChanged -= cell.OnCommandCanExecuteChanged;
            }, propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var cell = (ClickableViewCell)bindable;
                var newcommand = (ICommand)newvalue;
                if (newcommand != null)
                {
                    cell.IsEnabled = newcommand.CanExecute(cell.CommandParameter);
                    newcommand.CanExecuteChanged += cell.OnCommandCanExecuteChanged;
                }
            });

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ClickableViewCell), default(object),
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var cell = (ClickableViewCell)bindable;
                if (cell.Command != null)
                {
                    cell.IsEnabled = cell.Command.CanExecute(newvalue);
                }
            });

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void OnTapped()
        {
            base.OnTapped();

            if (!IsEnabled)
            {
                return;
            }

            Command?.Execute(CommandParameter);
        }

        void OnCommandCanExecuteChanged(object? sender, EventArgs eventArgs)
        {
            IsEnabled = Command.CanExecute(CommandParameter);
        }
    }
}
