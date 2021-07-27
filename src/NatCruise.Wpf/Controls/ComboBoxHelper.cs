using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Wpf.Controls
{
    public class ComboBoxHelper
    {
        public static int GetMaxLength(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxLengthProperty);
        }

        public static void SetMaxLength(DependencyObject obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached(
            "MaxLength",
            typeof(int),
            typeof(ComboBoxHelper),
            new UIPropertyMetadata(OnMaxLenghtChanged));

        private static void OnMaxLenghtChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var comboBox = obj as ComboBox;
            if (comboBox == null) return;

            comboBox.Loaded +=
                (s, e) =>
                {
                    var textBox = comboBox.FindChild<TextBox>("PART_EditableTextBox");
                    if (textBox == null) return;

                    textBox.SetValue(TextBox.MaxLengthProperty, args.NewValue);
                };
        }
    }
}
