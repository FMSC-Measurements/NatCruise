using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class TreeListPage : BasePage
{
	public TreeListPage()
	{
		InitializeComponent();
	}

	public TreeListPage(TreeListViewModel treeListViewModel) : this()
	{
		BindingContext = treeListViewModel;
	}
}