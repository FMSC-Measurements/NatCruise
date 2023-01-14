using FScruiser.XF.Controls;
using NatCruise.Models;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace FScruiser.XF.Util
{
    public static class LogEditControlFactory
    {
        public static View MakeEditView(LogFieldSetup field)
        {
            View editView = null;
            switch (field.Field)
            {
                case (nameof(Log.LogNumber)):
                    {
                        return null;
                    }
                case (nameof(Log.Grade)):
                case (nameof(Log.ExportGrade)):
                    {
                        editView = MakeGradePicker(field);
                        break;
                    }
                case (nameof(Log.BoardFootRemoved)):
                case (nameof(Log.BarkThickness)):
                case (nameof(Log.CubicFootRemoved)):
                case (nameof(Log.DIBClass)):
                case (nameof(Log.GrossBoardFoot)):
                case (nameof(Log.GrossCubicFoot)):
                case (nameof(Log.LargeEndDiameter)):
                case (nameof(Log.Length)):
                case (nameof(Log.NetBoardFoot)):
                case (nameof(Log.NetCubicFoot)):
                case (nameof(Log.PercentRecoverable)):
                case (nameof(Log.SeenDefect)):
                case (nameof(Log.SmallEndDiameter)):
                    {
                        editView = new Entry();
                        ((InputView)editView).Keyboard = Keyboard.Numeric;
                        ((Entry)editView).Behaviors.Add(new Xamarin.CommunityToolkit.Behaviors.NumericValidationBehavior
                        {
                            InvalidStyle = new Style<Entry>().Add((Entry.TextColorProperty, Color.Red))
                        });
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Log.{field.Field}");
                        break;
                    }
                default:
                    {
                        editView = new Entry();
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Log.{field.Field}");
                        break;
                    }
            }

            if (editView is Entry entry)
            {
                entry.Effects.Add(new Xamarin.CommunityToolkit.Effects.SelectAllTextEffect());
            }

            return editView;
        }

        public static View MakeGradePicker(LogFieldSetup field)
        {
            var editView = new ValuePicker();
            editView.ValueSource = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
            editView.SetBinding(ValuePicker.SelectedValueProperty, $"Log.{field.Field}");

            return editView;
        }
    }
}