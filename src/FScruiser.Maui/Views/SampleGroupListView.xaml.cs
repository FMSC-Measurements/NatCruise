using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;
using NatCruise.Models;

namespace FScruiser.Maui.Views;

public partial class SampleGroupListView : BasePage
{
	public SampleGroupListView()
	{
		InitializeComponent();
	}

	public SampleGroupListView(SampleGroupListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void openSampleGroupMenu(object sender, EventArgs e)
    {
        var vm = (SampleGroupListViewModel)BindingContext;
        var sampleGroup = (SampleGroup)((Element)sender).BindingContext;

        var actionSheetResult = await DisplayActionSheet((string)null, "Cancel", (string)null, "Subpopulations");

        if (vm != null)
        {
            switch (actionSheetResult)
            {
                case "Subpopulations":
                    {
                        vm.ShowSubpopulationsCommand.Execute(sampleGroup);
                        return;
                    }
            }
        }
    }
}