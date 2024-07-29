using FScruiser.Maui.Controls;
using FScruiser.Maui.Util;
using NatCruise.MVVM.ViewModels;

namespace FScruiser.Maui.Views;

public partial class TreeAuditRuleEditView : BasePage
{
	public TreeAuditRuleEditView()
	{
		InitializeComponent();
	}

    public TreeAuditRuleEditView(TreeAuditRuleEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }


    private void openTARSItemMenu(object sender, EventArgs e)
    {
        var swipeview = ((Element)sender).GetAncestor<SwipeView>();
        swipeview.Open(OpenSwipeItem.LeftItems);
    }
}