using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ImportView : BasePage
{
    public ImportView()
    {
        InitializeComponent();
    }

    public ImportView(ImportViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}