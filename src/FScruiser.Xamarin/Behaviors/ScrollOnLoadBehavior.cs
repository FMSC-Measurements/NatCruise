using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Behaviors
{
    public class ScrollOnLoadBehavior : Behavior<ListView>
    {
        protected override void OnAttachedTo(ListView listView)
        {
            base.OnAttachedTo(listView);

            listView.PropertyChanged += ListView_PropertyChanged;
        }

        private void ListView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var listView = (ListView)sender;
            if(e.PropertyName == nameof(ListView.ItemsSource))
            {
                ScrollBottom(listView);
            }
        }

        private void ScrollBottom(ListView listView)
        {
            var items = listView.ItemsSource;
            if(items == null) { return; }

            var lastItem = listView.ItemsSource.OfType<object>().LastOrDefault();
            if(lastItem == null) { return; }
            
            listView.ScrollTo(lastItem, ScrollToPosition.End, false);
        }

        protected override void OnDetachingFrom(ListView listView)
        {
            base.OnDetachingFrom(listView);

            listView.PropertyChanged -= ListView_PropertyChanged;
        }
    }
}
