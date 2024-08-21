using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SaleSelectView : BasePage
{
    protected SaleSelectView()
    {
        InitializeComponent();
    }

    public SaleSelectView(SaleSelectViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}