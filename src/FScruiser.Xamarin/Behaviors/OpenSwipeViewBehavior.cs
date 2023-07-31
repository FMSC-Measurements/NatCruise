using FScruiser.XF.Util;
using System;
using Xamarin.Forms;

namespace FScruiser.XF.Behaviors
{
    public class OpenSwipeViewBehavior : Behavior<Button>
    {
        public OpenSwipeItem OpenDirection { get; set; } = OpenSwipeItem.LeftItems;

        protected override void OnAttachedTo(Button button)
        {
            base.OnAttachedTo(button);

            button.Clicked += Button_Clicked;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenDirection);
        }

        protected override void OnDetachingFrom(Button button)
        {
            base.OnDetachingFrom(button);

            button.Clicked -= Button_Clicked;
        }
    }
}
