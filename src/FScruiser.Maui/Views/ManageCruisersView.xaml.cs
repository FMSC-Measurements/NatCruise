using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class ManageCruisersView : BasePage
{
	public ManageCruisersView()
	{
		InitializeComponent();
	}

	public ManageCruisersView(ManageCruisersViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}