using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewCells
{
    public abstract class TallyEntryViewCell_Base : ViewCell
    {
        private bool _isSelected;

        protected TallyEntryViewCell_Base()
        {
        }

        #region UntallyCommand

        /// <summary>
        /// Identifies the <see cref="UntallyCommand"/> bindable property.
        /// </summary>
        public static readonly BindableProperty UntallyCommandProperty =
            BindableProperty.Create(nameof(UntallyCommand),
              typeof(Command<string>),
              typeof(TallyEntryViewCell_Base),
              defaultValue: default(Command<string>));

        /// <summary>
        /// Gets or sets the <see cref="UntallyCommand" /> property. This is a bindable property.
        /// </summary>
        public ICommand UntallyCommand
        {
            get { return (Command<string>)GetValue(UntallyCommandProperty); }
            set { SetValue(UntallyCommandProperty, value); }
        }

        #endregion UntallyCommand

        public bool IsSelected
        {
            get { return _isSelected; }
            private set
            {
                if (_isSelected == value) { return; }
                _isSelected = value;
                OnIsSelectedChanged(value);
            }
        }

        protected abstract View DrawrView { get; }

        protected override void OnTapped()
        {
            base.OnTapped();

            IsSelected = !IsSelected;
        }

        protected override void OnPropertyChanging(string propertyName = null)
        {
            if (propertyName == nameof(Parent))
            {
                var parent = RealParent;
                if (parent != null && parent is ListView listView)
                {
                    UnwireListView(listView);
                }
            }

            base.OnPropertyChanging(propertyName);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            var parent = RealParent;
            if (parent != null && parent is ListView listView)
            {
                //WireListView(listView);
            }
        }

        protected virtual void UnwireListView(ListView listView)
        {
            listView.ItemSelected -= ListView_ItemSelected;

            //if (listView is CustomListView customListView)
            //{
            //    customListView.Scroll -= CustomListView_Scroll;
            //}
        }

        protected virtual void WireListView(ListView listView)
        {
            listView.ItemSelected += ListView_ItemSelected;
            //if (listView is CustomListView customListView)
            //{
            //    customListView.Scroll += CustomListView_Scroll;
            //}
        }

        //private void CustomListView_Scroll(object sender, System.EventArgs e)
        //{
        //    //IsSelected = false;
        //}

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //if(IsSelected == false)
            //{ return; }
            //var myItem = BindingContext;
            //var selectedItem = e.SelectedItem;

            //if (selectedItem == null || object.ReferenceEquals(myItem, selectedItem) == false)
            //{
            //    if (IsSelected) { IsSelected = false; }
            //}
        }

        protected virtual void OnIsSelectedChanged(bool isSelected)
        {
            MessagingCenter.Send<object, bool>(this, Messages.TREECELL_ISELECTED_CHANGED, isSelected);

            RefreshDrawer(isSelected);
            base.ForceUpdateSize();
            if (isSelected)
            {
                EnsureVisable();
            }
        }

        protected void RefreshDrawer(bool isSelected)
        {
            DrawrView.IsVisible = isSelected;
            base.ForceUpdateSize();
        }

        protected void EnsureVisable()
        {
            var item = BindingContext;
            if (item == null) { return; }
            var parent = RealParent;
            if (parent != null && parent is ListView listView)
            {
                listView.ScrollTo(item, ScrollToPosition.MakeVisible, false);
            }
        }
    }
}