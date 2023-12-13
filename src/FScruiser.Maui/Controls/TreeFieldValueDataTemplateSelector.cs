using Microsoft.Maui.Controls.Compatibility;
using NatCruise.Models;

namespace FScruiser.Maui.Controls;

public class TreeFieldValueDataTemplateSelector : DataTemplateSelector
{
    public static readonly BindableProperty UseAltInitialsTemplateProperty = BindableProperty.CreateAttached(
        "UseAltInitialsTemplate",
        typeof(bool),
        typeof(Layout<View>),
        false);

    public static bool GetUseAltInitialsTemplate(BindableObject target) => (bool)target.GetValue(UseAltInitialsTemplateProperty);

    public static void SetUseAltInitialsTemplate(BindableObject target, bool value) => target.SetValue(UseAltInitialsTemplateProperty, value);

    public DataTemplate? RealTemplate { get; set; }
    public DataTemplate? IntTemplate { get; set; }
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? BoolTemplate { get; set; }

    public DataTemplate? RemarksTemplate { get; set; }

    public DataTemplate? InitialsTemplate { get; set; }

    public DataTemplate? InitialsAltTemplate { get; set; }

    public DataTemplate? LiveDeadTemplate { get; set; }

    public DataTemplate? GradeTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        var useAltInitialsTemplate = (bool)container.GetValue(UseAltInitialsTemplateProperty);

        var tfv = (TreeFieldValue)item;

        // if useAltInitialsTemplate is false just let fall back to TextTemplate
        if (tfv.Field.Equals(nameof(TreeEx.Initials), StringComparison.OrdinalIgnoreCase) && useAltInitialsTemplate) return InitialsAltTemplate;

        if (tfv.Field.Equals(nameof(TreeEx.Remarks), StringComparison.OrdinalIgnoreCase)) return RemarksTemplate;
        if (tfv.Field.Equals(nameof(TreeEx.LiveDead), StringComparison.OrdinalIgnoreCase)) return LiveDeadTemplate;
        if (tfv.Field.Equals(nameof(TreeEx.Grade), StringComparison.OrdinalIgnoreCase)) return GradeTemplate;

        switch (tfv.DBType)
        {
            case "REAL":
                {
                    return RealTemplate;
                }
            case "INT":
            case "INTEGER":
                {
                    return IntTemplate;
                }
            case "TEXT":
                {
                    return TextTemplate;
                }
            case "BOOL":
            case "BOOLEAN":
                {
                    return BoolTemplate;
                }
        }

        return null;
    }
}