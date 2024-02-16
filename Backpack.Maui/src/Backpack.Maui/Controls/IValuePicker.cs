using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Backpack.Maui.Controls
{
    public interface IValuePicker : IPicker, IView, ITextStyle, ITextAlignment, IItemDelegate<string>
    {
        IList Items { get; }

        string Text { get; }

        string? AuxiliaryActionHeading { get; }

        ICommand AuxiliaryActionCommand { get; }

        event EventHandler AuxiliaryActionClicked;

        void RaiseAuxiliaryActionClicked();

        //string?  GetItemDisplayValue(object item);
    }
}
