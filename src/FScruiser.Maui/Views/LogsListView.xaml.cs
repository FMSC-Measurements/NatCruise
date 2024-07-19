using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class LogsListView : BasePage
{
	public LogsListView()
	{
		InitializeComponent();
	}

	public LogsListView(LogsListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}