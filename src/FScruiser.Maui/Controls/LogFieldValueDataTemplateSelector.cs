using DevExpress.Maui.DataGrid;
using NatCruise.Models;

namespace FScruiser.Maui.Controls
{
    public class LogFieldValueDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? Grade { get; set; }

        public DataTemplate? Real { get; set; }

        public DataTemplate? Integer { get; set; }

        public DataTemplate? Default { get; set; }

        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {

            var logField = (item is CellData cellData) ? (LogFieldValue)cellData.Item : (LogFieldValue)item;
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

                case (nameof(Log.NetBoardFoot)):
                case (nameof(Log.NetCubicFoot)):
                case (nameof(Log.PercentRecoverable)):
                case (nameof(Log.SeenDefect)):
                case (nameof(Log.SmallEndDiameter)):
                    {
                        return Real;
                    }
                case (nameof(Log.Length)):
                    {
                        return Integer;
                    }
                default:
                    {
                        return Default;
                    }
            }
        }
    }
}