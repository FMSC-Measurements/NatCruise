namespace FScruiser.Maui.TestViews;

public partial class TestControlsViews : ContentPage
{
    private object _selectedRadioButtonValue;

    public object SelectedRadioButtonValue
	{
		get => _selectedRadioButtonValue;
		set
		{
			_selectedRadioButtonValue = value;
			OnPropertyChanged();
		}
	}

    public TestControlsViews()
	{
		InitializeComponent();

		BindingContext = this;
	}

    private void RadioButton_Tapped(object sender, TappedEventArgs e)
    {
		var view = (View)sender;
		var parrent = view.Parent as Layout;
		if (parrent != null)
		{
			var rb = parrent.Children.First(x => x is RadioButton) as RadioButton;
			rb.IsChecked = true;
		}
    }
}