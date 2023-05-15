using FScruiser.XF.Controls;
using FScruiser.XF.Util;
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

        private void sampleGroupItemMenuButton_clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.BottomItems);
        }
    }
}