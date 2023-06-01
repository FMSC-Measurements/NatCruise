using FScruiser.XF.Controls;
using FScruiser.XF.Util;
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
    public partial class TreeAuditRuleEditView : InitializableContentPage
    {
        public TreeAuditRuleEditView()
        {
            InitializeComponent();
        }

        private void openTARSItemMenu(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.LeftItems);
        }
    }
}