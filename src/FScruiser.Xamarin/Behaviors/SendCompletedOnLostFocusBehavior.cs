using Xamarin.Forms;

namespace FScruiser.XF.Behaviors
{
    public class SendCompletedOnLostFocusBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);

            entry.Unfocused += Entry_Unfocused;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            entry.SendCompleted();
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);

            entry.Unfocused -= Entry_Unfocused;
        }
    }
}