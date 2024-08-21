using System.Runtime.CompilerServices;

namespace FScruiser.Maui.Controls;

public partial class ErrorCountTag : Frame
{
	public ErrorCountTag()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty ErrorCountProperty = BindableProperty.Create(
        nameof(ErrorCount),
        typeof(int?),
        typeof(ErrorCountTag),
        0,
        propertyChanged: (s, o, n) => ((ErrorCountTag)s).OnPropertyChanged());

    public int? ErrorCount { get => (int)GetValue(ErrorCountProperty); set => SetValue(ErrorCountProperty, value); }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(ErrorCount))
        {
            PART_errorCountSpan.Text = ErrorCount?.ToString() ?? "";
        }
    }
}