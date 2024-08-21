namespace FScruiser.Maui.Behaviors;

public class ScrollOnLoadBehavior : Behavior<ItemsView>
{
    protected override void OnAttachedTo(ItemsView listView)
    {
        base.OnAttachedTo(listView);

        listView.PropertyChanged += ListView_PropertyChanged;
    }

    private void ListView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender == null) { return; }
        var listView = (ItemsView)sender;
        if (e.PropertyName == nameof(ItemsView.ItemsSource))
        {
            ScrollBottom(listView);
        }
    }

    private void ScrollBottom(ItemsView listView)
    {
        var items = listView.ItemsSource;
        if (items == null) { return; }

        var itemCount = listView.ItemsSource?.OfType<object>()?.Count() ?? 0;
        if (itemCount > 2)
        {
            listView.ScrollTo(itemCount - 1, position: ScrollToPosition.End, animate: false);
        }
    }

    protected override void OnDetachingFrom(ItemsView listView)
    {
        base.OnDetachingFrom(listView);

        listView.PropertyChanged -= ListView_PropertyChanged;
    }
}