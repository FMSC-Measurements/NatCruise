using FScruiser.Maui.Controls;
using NatCruise.MVVM.ViewModels;

namespace FScruiser.Maui.Views;

public partial class StratumTreeFieldSetupView : BasePage
{
	public StratumTreeFieldSetupView()
	{
		InitializeComponent();
	}

	public StratumTreeFieldSetupView(StratumTreeFieldSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}