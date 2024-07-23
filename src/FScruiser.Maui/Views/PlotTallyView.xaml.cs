using FScruiser.Maui.Controls;
using FScruiser.Maui.Util;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class PlotTallyView : BasePage
{
	public PlotTallyView()
	{
		InitializeComponent();
	}

	public PlotTallyView(PlotTallyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void _stratumFilterButton_Clicked(object sender, EventArgs e)
    {
        if (sender != null && sender is View view
            && view != null && view.BindingContext is string stratumFilter
            && BindingContext is PlotTallyViewModel viewModel)
        {
            viewModel.StratumFilter = stratumFilter;
        }
    }

    private void _treeCellTappedGesture_Tapped(object sender, TappedEventArgs e)
    {
        _treeEditPanel.IsVisible = true;
    }

    private void _hideTreeEditPanelButton_Clicked(object sender, EventArgs e)
    {
        _treeEditPanel.IsVisible = false;
    }

    private void openDeleteButton_Clicked(object sender, EventArgs e)
    {
        var swipeview = ((Element)sender).GetAncestor<SwipeView>();
        swipeview.Open(OpenSwipeItem.LeftItems);
    }

    private void _treeEditPanel_BindingContextChanged(object sender, EventArgs e)
    {
        var bindingContext = _treeEditPanel.BindingContext;
        if (bindingContext == null && _treeEditPanel.IsVisible)
        {
            _treeEditPanel.IsVisible = false;
        }
    }

    
}