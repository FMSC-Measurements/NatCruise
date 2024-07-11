namespace FScruiser.Maui.Behaviors
{
    public class SendCompletedOnLostFocusBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);

            entry.Unfocused += Entry_Unfocused;
        }

        private void Entry_Unfocused(object? sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            entry?.SendCompleted();
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);

            entry.Unfocused -= Entry_Unfocused;
        }
    }
}