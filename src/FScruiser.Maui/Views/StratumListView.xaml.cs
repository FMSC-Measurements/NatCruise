using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;
using NatCruise.Models;

namespace FScruiser.Maui.Views;

public partial class StratumListView : BasePage
{
	public StratumListView()
	{
		InitializeComponent();
	}

	public StratumListView(StratumListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void openStratumMenu(object sender, EventArgs e)
    {
        var vm = (StratumListViewModel)BindingContext;
        var stratum = (Stratum)((Element)sender).BindingContext;

        var actionSheetResult = await DisplayActionSheet((string)null, "Cancel", (string)null, "Fields", "Sample Groups");


        if (vm != null)
        {
            switch (actionSheetResult)
            {
                case "Fields":
                    {
                        vm.ShowFieldSetupCommand.Execute(stratum);
                        return;
                    }
                case "Sample Groups":
                    {
                        vm.ShowSampleGroupsCommand.Execute(stratum);
                        return;
                    }
            }
        }

        //var swipeview = ((Element)sender).GetAncestor<SwipeView>();
        //swipeview.Open(OpenSwipeItem.BottomItems);
    }
}