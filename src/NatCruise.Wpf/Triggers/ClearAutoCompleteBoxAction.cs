using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NatCruise.Wpf.Triggers
{
    public class ClearAutoCompleteBoxAction : TargetedTriggerAction<AutoCompleteBox>
    {
        protected override void Invoke(object parameter)
        {
            var target = Target;
            target.ClearValue(AutoCompleteBox.TextProperty);
        }
    }
}
