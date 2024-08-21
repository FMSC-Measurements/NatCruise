using System.Runtime.CompilerServices;

namespace FScruiser.Maui.Controls;

public partial class WarningCountTag : Frame
{
	public WarningCountTag()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty WarningCountProperty = BindableProperty.Create(
        nameof(WarningCount),
        typeof(int?),
        typeof(WarningCountTag),
        0,
        propertyChanged: (s, o, n) => ((WarningCountTag)s).OnPropertyChanged());

    public int? WarningCount { get => (int)GetValue(WarningCountProperty); set => SetValue(WarningCountProperty, value); }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(WarningCount))
        {
            PART_warningCountSpan.Text = WarningCount?.ToString() ?? "";
        }
    }
}