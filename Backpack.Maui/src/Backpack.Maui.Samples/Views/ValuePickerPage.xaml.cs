using Backpack.Maui.Samples.ViewModels;

namespace Backpack.Maui.Samples.Views;

public partial class ValuePickerPage : ContentPage
{
	public ValuePickerPage()
	{
		InitializeComponent();

		BindingContext = new ValuePickerSampleViewModel();
	}
}