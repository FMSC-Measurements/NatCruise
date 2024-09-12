using FScruiser.Maui.Constants;

namespace FScruiser.Maui.Controls;

public partial class RadioButtonIndicator : ContentView
{
	public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(
        nameof(IsChecked),
        typeof(bool),
        typeof(RadioButtonIndicator),
        defaultValue: false,
        propertyChanged: (sender, oldVal, newVal) => ((RadioButtonIndicator)sender).UpdateIndicator()
        );

    public bool IsChecked { get => (bool)GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }

    public static readonly BindableProperty ForgroundColorProperty = BindableProperty.Create(
        nameof(ForgroundColor),
        typeof(Color),
        typeof(RadioButtonIndicator),
        defaultValue: Colors.Black,
        propertyChanged: (sender, oldVal, newVal) => ((RadioButtonIndicator)sender).UpdateIndicator()
        );

    public Color ForgroundColor { get => (Color)GetValue(ForgroundColorProperty); set => SetValue(ForgroundColorProperty, value); }

    public RadioButtonIndicator()
	{
		InitializeComponent();

	}

    protected void UpdateIndicator()
    {
        if (IsChecked)
        {
            PART_Indicator.Source = new FontImageSource
            {
                FontFamily = Fonts.FAsolid,
                Glyph = FAIcons.CircleDot,
                Color = ForgroundColor
            };
        }
        else
        {
            PART_Indicator.Source = new FontImageSource
            {
                FontFamily = Fonts.FAsolid,
                Glyph = FAIcons.Circle,
                Color = ForgroundColor
            };
        }
    }
}