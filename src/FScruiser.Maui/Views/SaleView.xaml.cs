using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class SaleView : BasePage
{
	public SaleView()
	{
		InitializeComponent();
	}

	public SaleView(SaleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}