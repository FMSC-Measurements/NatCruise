using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NatCruise.Wpf.Triggers
{
    public class ClearTextBoxAction : TargetedTriggerAction<TextBox>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override void Invoke(object parameter)
        {
            var target = Target;
            target.Clear();
        }
    }
}
