using NatCruise.Models;

namespace FScruiser.Maui.Controls
{
    public class LogFieldDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? Grade { get; set; }

        public DataTemplate? DefaultNumeric { get; set; }

        public DataTemplate? Default { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var logField = (LogFieldSetup)item;
            var fieldName = logField.Field;
            switch (fieldName)
            {
                case (nameof(Log.Grade)):
                case (nameof(Log.ExportGrade)):
                    {
                        return Grade;
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
                        return DefaultNumeric;
                    }
                default:
                    {
                        return Default;
                    }
            }
        }
    }
}