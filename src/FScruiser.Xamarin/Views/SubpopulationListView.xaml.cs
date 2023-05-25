using FScruiser.XF.Controls;
using FScruiser.XF.Util;
using NatCruise.MVVM.ViewModels;
using NatCruise.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubpopulationListView : InitializableContentPage
    {
        public SubpopulationListView()
        {
            InitializeComponent();
        }

        private void subpopulationItemMenu_clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.LeftItems);
        }

        private void Page_BindingContextChanged(object sender, EventArgs e)
        {
            var vm = BindingContext as SubpopulationListViewModel;
            if (vm == null) return;

            vm.SubpopulationAdded += HandleSubpopulationAdded;
        }

        private void HandleSubpopulationAdded(object sender, EventArgs e)
        {
            _addSubpopSpeciesPicker.SelectedItem = null;
        }

        private async void _addSubpopSpeciesPicker_AuxiliaryActionClicked(object sender, EventArgs e)
        {
            var newSpecies = await DisplayPromptAsync("New Species Code", (string)null,
                maxLength: SubpopulationListViewModel.SPECIES_CODE_MAX_LENGTH);
            if (newSpecies.IsNullOrEmpty()) return;

            _addSubpopSpeciesPicker.Text = newSpecies;
        }
    }
}