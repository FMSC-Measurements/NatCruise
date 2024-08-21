using FScruiser.Maui.Controls;
using NatCruise.MVVM.ViewModels;

namespace FScruiser.Maui.Views;

public partial class TallyPopulationDetailsView : BasePage
{
	public TallyPopulationDetailsView()
	{
		InitializeComponent();
	}

	public TallyPopulationDetailsView(TallyPopulationDetailsViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
    }
}