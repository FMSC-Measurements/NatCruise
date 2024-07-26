using FScruiser.Maui.Controls;
using FScruiser.Maui.Util;
using NatCruise.MVVM.ViewModels;
using NatCruise.Util;

namespace FScruiser.Maui.Views;

public partial class SubpopulationListView : BasePage
{
	public SubpopulationListView()
	{
		InitializeComponent();
	}

	public SubpopulationListView(SubpopulationListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void subpopulationItemMenu_clicked(object sender, EventArgs e)
    {
        var swipeview = ((Element)sender).GetAncestor<SwipeView>();
        swipeview.Open(OpenSwipeItem.LeftItems);
    }

    private void Page_BindingContextChanged(object sender, EventArgs e)
    {
        var vm = BindingContext as SubpopulationListViewModel;
        if (vm == null) return;

        vm.SubpopulationAdded += HandleSubpopulationAdded;
    }

    private void HandleSubpopulationAdded(object sender, EventArgs e)
    {
        _addSubpopSpeciesPicker.SelectedItem = null;
    }

    private async void _addSubpopSpeciesPicker_AuxiliaryActionClicked(object sender, EventArgs e)
    {
        var newSpecies = await DisplayPromptAsync("New Species Code", (string)null,
            maxLength: SubpopulationListViewModel.SPECIES_CODE_MAX_LENGTH);
        if (newSpecies.IsNullOrEmpty()) return;

        _addSubpopSpeciesPicker.Text = newSpecies;
    }
}