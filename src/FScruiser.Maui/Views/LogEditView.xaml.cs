using FScruiser.Maui.Controls;
using NatCruise.MVVM.ViewModels;

namespace FScruiser.Maui.Views;

public partial class LogEditView : BasePage
{
	public LogEditView()
	{
		InitializeComponent();
	}

	public LogEditView(LogEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}