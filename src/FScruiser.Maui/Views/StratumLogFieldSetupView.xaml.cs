using FScruiser.Maui.Controls;
using NatCruise.MVVM.ViewModels;

namespace FScruiser.Maui.Views;

public partial class StratumLogFieldSetupView : BasePage
{
	public StratumLogFieldSetupView()
	{
		InitializeComponent();
	}

	public StratumLogFieldSetupView(StratumLogFieldSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}