using FScruiser.Maui.Controls;
using FScruiser.Maui.MVVM;
using FScruiser.Maui.Util;
using FScruiser.Maui.ViewModels;
using Microsoft.Extensions.Logging;
using NatCruise.Navigation;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CruiseSelectView : BasePage
{
    public CruiseSelectView()
    {
        InitializeComponent();
    }

    public CruiseSelectView(CruiseSelectViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}