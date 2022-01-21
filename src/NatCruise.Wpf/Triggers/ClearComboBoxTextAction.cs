using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

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