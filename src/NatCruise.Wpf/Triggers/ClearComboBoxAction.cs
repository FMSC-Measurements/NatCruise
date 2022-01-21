using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace NatCruise.Wpf.Triggers
{
    public class ClearComboBoxAction : TargetedTriggerAction<ComboBox>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override void Invoke(object parameter)
        {
            var target = Target;
            Target.SelectedIndex = -1;
        }
    }
}