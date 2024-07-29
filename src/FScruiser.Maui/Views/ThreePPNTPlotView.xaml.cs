using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class ThreePPNTPlotView : BasePage
{
	public ThreePPNTPlotView()
	{
		InitializeComponent();
	}

	public ThreePPNTPlotView(ThreePPNTPlotViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}