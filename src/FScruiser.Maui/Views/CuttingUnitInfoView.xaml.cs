using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class CuttingUnitInfoView : BasePage
{
	protected CuttingUnitInfoView()
	{
		InitializeComponent();
	}

	public CuttingUnitInfoView(CuttingUnitInfoViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}