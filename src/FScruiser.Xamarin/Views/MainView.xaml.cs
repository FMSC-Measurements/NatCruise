using FScruiser.XF.ViewModels;
using Prism.Navigation;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : FlyoutPage, IMasterDetailPageOptions
    {
        public bool IsPresentedAfterNavigation => Device.Idiom != TargetIdiom.Phone;

        public MainView()
        {
            InitializeComponent();
        }

        private void _cuttingUnitPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var vm = BindingContext as MainViewModel;
            //var selectedIndex = _cuttingUnitPicker.SelectedIndex;
            //var selectedItem = _cuttingUnitPicker.ItemsSource[selectedIndex] as CuttingUnit_Ex;
            //vm.SelectedCuttingUnit = selectedItem;
        }
    }
}