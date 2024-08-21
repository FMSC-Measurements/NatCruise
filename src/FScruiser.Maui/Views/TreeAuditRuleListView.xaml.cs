using FScruiser.Maui.Controls;
using FScruiser.Maui.Util;
using NatCruise.MVVM.ViewModels;

namespace FScruiser.Maui.Views;

public partial class TreeAuditRuleListView : BasePage
{
	public TreeAuditRuleListView()
	{
		InitializeComponent();
	}

	public TreeAuditRuleListView(TreeAuditRuleListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void openTARItemMenu(object sender, EventArgs e)
    {
        var swipeview = ((Element)sender).GetAncestor<SwipeView>();
        swipeview.Open(OpenSwipeItem.LeftItems);
    }
}