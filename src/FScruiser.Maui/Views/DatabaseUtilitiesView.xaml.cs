using DevExpress.Maui.Core.Internal;
using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DatabaseUtilitiesView : BasePage
{
    public DatabaseUtilitiesView()
    {
        InitializeComponent();
    }

    public DatabaseUtilitiesView(DatabaseUtilitiesViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}