using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class PlotEditView : BasePage
{
	public PlotEditView()
	{
		InitializeComponent();
	}

	public PlotEditView(PlotEditViewModel viewModel) : this()
	{
		BindingContext = viewModel;
	}
}