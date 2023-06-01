using FScruiser.XF.Controls;
using FScruiser.XF.Util;
using FScruiser.XF.ViewModels;
using NatCruise.Models;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SampleGroupListView : InitializableContentPage
    {
        public SampleGroupListView()
        {
            InitializeComponent();
        }

        //private void sampleGroupItemMenuButton_clicked(object sender, EventArgs e)
        //{
        //    var swipeview = ((Element)sender).GetAncestor<SwipeView>();
        //    swipeview.Open(OpenSwipeItem.BottomItems);
        //}

        private async void openSampleGroupMenu(object sender, EventArgs e)
        {
            var vm = (SampleGroupListViewModel)BindingContext;
            var sampleGroup = (SampleGroup)((Element)sender).BindingContext;

            var actionSheetResult = await DisplayActionSheet((string)null, "Cancel", (string)null, "Subpopulations");


            if (vm != null)
            {
                switch (actionSheetResult)
                {
                    case "Subpopulations":
                        {
                            vm.ShowSubpopulationsCommand.Execute(sampleGroup);
                            return;
                        }
                }
            }
        }
    }
}