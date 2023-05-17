using FScruiser.XF.Controls;
using FScruiser.XF.Util;
using FScruiser.XF.ViewModels;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StratumListView : InitializableContentPage
    {
        public StratumListView()
        {
            InitializeComponent();
        }


        private async void openStratumMenu(object sender, EventArgs e)
        {
            var vm = (StratumListViewModel)BindingContext;
            var stratum = (Stratum)((Element)sender).BindingContext;

            var actionSheetResult = await DisplayActionSheet((string)null, "Cancel", (string)null, "Fields", "Sample Groups");

            
            if (vm != null)
            {
                switch (actionSheetResult)
                {
                    case "Fields":
                        {
                            vm.ShowFieldSetupCommand.Execute(stratum);
                            return;
                        }
                    case "Sample Groups":
                        {
                            vm.ShowSampleGroupsCommand.Execute(stratum);
                            return;
                        }
                }
            }

            //var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            //swipeview.Open(OpenSwipeItem.BottomItems);
        }
    }
}