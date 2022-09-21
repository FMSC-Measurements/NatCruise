using FScruiser.XF.Controls;
using FScruiser.XF.ViewModels;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.XF.Views
{
    public partial class CuttingUnitListView : InitializableContentPage
    {
        protected CuttingUnitListViewModel ViewModel => (CuttingUnitListViewModel)BindingContext;

        public CuttingUnitListView()
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