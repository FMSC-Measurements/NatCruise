using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class FixCntTallyView : BasePage
{
	protected FixCntTallyView()
	{
		InitializeComponent();
	}

	public FixCntTallyView(FixCNTTallyViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}