using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class StratumFieldSetupView : BaseTabbedPage
{
	public StratumFieldSetupView()
	{
		InitializeComponent();
	}

	public StratumFieldSetupView(StratumFieldSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}