using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NatCruise.Wpf.Triggers
{
    public class ClearComboBoxTextAction : TargetedTriggerAction<ComboBox>
    {
        protected override void Invoke(object parameter)
        {
            var target = Target;
            target.ClearValue(ComboBox.TextProperty);
        }
    }
}