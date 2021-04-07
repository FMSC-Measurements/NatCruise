using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NatCruise.Wpf.Triggers
{
    public class ClearComboBoxAction : TargetedTriggerAction<ComboBox>
    {
        protected override void Invoke(object parameter)
        {
            var target = Target;
            Target.SelectedIndex = -1;
        }
    }
}