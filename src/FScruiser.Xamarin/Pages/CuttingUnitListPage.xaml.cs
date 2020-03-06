using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.XF.Pages
{
    public partial class CuttingUnitListPage : ContentPage
    {
        protected CuttingUnitListViewModel ViewModel => (CuttingUnitListViewModel)BindingContext;

        public CuttingUnitListPage()
        {
            try
            {
                InitializeComponent();
                Prism.Mvvm.ViewModelLocator.SetAutowireViewModel(this, true);

                UnitListView.ItemSelected += UnitListView_ItemSelected;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void UnitListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            if (sender == null) { throw new ArgumentNullException(nameof(sender)); }
            if (eventArgs == null) { throw new ArgumentNullException(nameof(eventArgs)); }
            if (eventArgs.SelectedItem == null) { return; } //selected item may be null, do nothing if it is

            var unit = (CuttingUnit)eventArgs.SelectedItem;

            ViewModel.SelectUnit(unit);
        }
    }
}