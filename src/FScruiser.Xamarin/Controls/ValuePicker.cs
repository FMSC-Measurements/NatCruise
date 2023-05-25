using NatCruise.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FScruiser.XF.Controls
{
    // the standard picker works more off of object.ReferenceEquals
    // value picker equates selectedValue from ValueSource by their string value
    // making it work better with value types
    public class ValuePicker : View, IFontElement
    {
        #region TextElement Properties

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(ValuePicker),
            (object)Color.Default,
            propertyChanged: (target, oldValue, newValue) => ((ValuePicker)target).OnTextColorPropertyChanged((Color)oldValue, (Color)newValue));

        public Color TextColor
        {
            get => (Color)((BindableObject)this).GetValue(ValuePicker.TextColorProperty);
            set => ((BindableObject)this).SetValue(ValuePicker.TextColorProperty, (object)value);
        }

        private void OnTextColorPropertyChanged(Color oldValue, Color newValue)
        {
        }

        #endregion TextElement Properties

        #region FontElement Properties

        public static readonly BindableProperty FontProperty = BindableProperty.Create(
            "Font",
            typeof(Font),
            typeof(IFontElement),
            (object)new Font(),
            propertyChanged: OnFontPropertyChanged);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            nameof(FontFamily),
            typeof(string),
            typeof(IFontElement),
            default(string),
            propertyChanged: OnFontFamilyChanged);

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            nameof(FontSize),
            typeof(double),
            typeof(IFontElement),
            (object)-1.0,
            propertyChanged: OnFontSizeChanged,
            defaultValueCreator: FontSizeDefaultValueCreator);

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
            nameof(FontAttributes),
            typeof(FontAttributes),
            typeof(IFontElement),
            (object)FontAttributes.None,
            propertyChanged: OnFontAttributesChanged);

        private static readonly BindableProperty CancelEventsProperty = BindableProperty.Create(
            "CancelEvents",
            typeof(bool),
            typeof(ValuePicker),
            (object)false);

        private static void OnFontPropertyChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (ValuePicker.GetCancelEvents(bindable))
                return;
            ValuePicker.SetCancelEvents(bindable, true);
            Font font = (Font)newValue;
            if (font == Font.Default)
            {
                bindable.ClearValue(ValuePicker.FontFamilyProperty);
                bindable.ClearValue(ValuePicker.FontSizeProperty);
                bindable.ClearValue(ValuePicker.FontAttributesProperty);
            }
            else
            {
                bindable.SetValue(ValuePicker.FontFamilyProperty, font.FontFamily);
                if (font.UseNamedSize)
                    bindable.SetValue(ValuePicker.FontSizeProperty, Device.GetNamedSize(font.NamedSize, bindable.GetType(), true));
                else
                    bindable.SetValue(ValuePicker.FontSizeProperty, font.FontSize);
                bindable.SetValue(ValuePicker.FontAttributesProperty, font.FontAttributes);
            }
            ValuePicker.SetCancelEvents(bindable, false);
        }

        private static void OnFontFamilyChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (ValuePicker.GetCancelEvents(bindable))
                return;
            ValuePicker.SetCancelEvents(bindable, true);
            double num = (double)bindable.GetValue(ValuePicker.FontSizeProperty);
            FontAttributes fontAttributes = (FontAttributes)bindable.GetValue(ValuePicker.FontAttributesProperty);
            string str = (string)newValue;
            if (str != null)
            {
                BindableObject bindableObject = bindable;
                BindableProperty fontProperty = ValuePicker.FontProperty;
                Font font1 = Font.OfSize(str, num);
                Font font2 = font1.WithAttributes(fontAttributes);
                bindableObject.SetValue(fontProperty, (object)font2);
            }
            else
                bindable.SetValue(ValuePicker.FontProperty, Font.SystemFontOfSize(num, fontAttributes));
            ValuePicker.SetCancelEvents(bindable, false);
            ((IFontElement)bindable).OnFontFamilyChanged((string)oldValue, (string)newValue);
        }

        private static void OnFontSizeChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (ValuePicker.GetCancelEvents(bindable))
                return;
            ValuePicker.SetCancelEvents(bindable, true);
            double num = (double)newValue;
            FontAttributes fontAttributes = (FontAttributes)bindable.GetValue(ValuePicker.FontAttributesProperty);
            string str = (string)bindable.GetValue(ValuePicker.FontFamilyProperty);
            if (str != null)
            {
                BindableObject bindableObject = bindable;
                BindableProperty fontProperty = ValuePicker.FontProperty;
                Font font1 = Font.OfSize(str, num);
                Font font2 = font1.WithAttributes(fontAttributes);
                bindableObject.SetValue(fontProperty, (object)font2);
            }
            else
                bindable.SetValue(ValuePicker.FontProperty, (object)Font.SystemFontOfSize(num, fontAttributes));
            ValuePicker.SetCancelEvents(bindable, false);
            ((IFontElement)bindable).OnFontSizeChanged((double)oldValue, (double)newValue);
        }

        private static object FontSizeDefaultValueCreator(BindableObject bindable) => ((IFontElement)bindable).FontSizeDefaultValueCreator();

        private static void OnFontAttributesChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (ValuePicker.GetCancelEvents(bindable))
                return;
            ValuePicker.SetCancelEvents(bindable, true);
            double num = (double)bindable.GetValue(ValuePicker.FontSizeProperty);
            FontAttributes fontAttributes = (FontAttributes)newValue;
            string str = (string)bindable.GetValue(ValuePicker.FontFamilyProperty);
            if (str != null)
            {
                BindableObject bindableObject = bindable;
                BindableProperty fontProperty = ValuePicker.FontProperty;
                Font font1 = Font.OfSize(str, num);
                Font font2 = font1.WithAttributes(fontAttributes);
                bindableObject.SetValue(fontProperty, font2);
            }
            else
                bindable.SetValue(ValuePicker.FontProperty, (object)Font.SystemFontOfSize(num, fontAttributes));
            ValuePicker.SetCancelEvents(bindable, false);
            ((IFontElement)bindable).OnFontAttributesChanged((FontAttributes)oldValue, (FontAttributes)newValue);
        }

        private static bool GetCancelEvents(BindableObject bindable) => (bool)bindable.GetValue(ValuePicker.CancelEventsProperty);

        private static void SetCancelEvents(BindableObject bindable, bool value) => bindable.SetValue(ValuePicker.CancelEventsProperty, value);

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(ValuePicker.FontAttributesProperty);
            set => SetValue(ValuePicker.FontAttributesProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(ValuePicker.FontFamilyProperty);
            set => SetValue(ValuePicker.FontFamilyProperty, value);
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(ValuePicker.FontSizeProperty);
            set => SetValue(ValuePicker.FontSizeProperty, value);
        }

        void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) => InvalidateMeasure();

        void IFontElement.OnFontSizeChanged(double oldValue, double newValue) => InvalidateMeasure();

        double IFontElement.FontSizeDefaultValueCreator() => Device.GetNamedSize(NamedSize.Default, this);

        void IFontElement.OnFontAttributesChanged(
          FontAttributes oldValue,
          FontAttributes newValue)
        {
            InvalidateMeasure();
        }

        void IFontElement.OnFontChanged(Font oldValue, Font newValue) => InvalidateMeasure();

        #endregion FontElement Properties

        #region Title Properties

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(ValuePicker),
            (object)null);

        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
            nameof(TitleColor),
            typeof(Color),
            typeof(ValuePicker),
            (object)new Color());

        // Title displayed on the popup when selecting items and in hint text
        public string Title
        {
            get => (string)GetValue(ValuePicker.TitleProperty);
            set => SetValue(ValuePicker.TitleProperty, value);
        }

        public Color TitleColor
        {
            get => (Color)((BindableObject)this).GetValue(ValuePicker.TitleColorProperty);
            set => ((BindableObject)this).SetValue(ValuePicker.TitleColorProperty, value);
        }

        #endregion Title Properties

        #region selected value property

        public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create(
            nameof(SelectedValue),
            typeof(object),
            typeof(ValuePicker),
            (object)null,
            BindingMode.TwoWay,
            propertyChanged: (target, oldValue, newValue) => ((ValuePicker)target).OnSelectedValueChanged(oldValue, newValue),
            coerceValue: (bindable, value) => ((ValuePicker)bindable).CoerceSelectedValue(bindable, value));

        private object CoerceSelectedValue(BindableObject bindable, object value)
        {
            if (!_isSelectionChanging)
            {
                var item = SelectItemWithValue(value, false);

                if (item == BindableProperty.UnsetValue && HasItems)
                { value = null; }
            }
            else
            {
                _selectedValueDrivesSelection = false;
            }

            return value;
        }

        protected void OnSelectedValueChanged(object oldValue, object newValue)
        {
            var pendingItem = PendingSelectByValueItem;
            if (pendingItem == null) return;

            try
            {
                if (!_isSelectionChanging)
                {
                    _selectedValueDrivesSelection = true;
                    SelectItem(pendingItem);
                }
            }
            finally
            {
                _selectedValueDrivesSelection = false;
                PendingSelectByValueItem = null;
            }
        }

        public object SelectedValue
        {
            get => GetValue(ValuePicker.SelectedValueProperty);
            set => SetValue(ValuePicker.SelectedValueProperty, value);
        }

        #endregion selected value property

        #region SelectedValuePath Property

        public static readonly BindableProperty SelectedValuePathProperty = BindableProperty.Create(
            nameof(SelectedValuePath),
            typeof(string),
            typeof(ValuePicker),
            default(string),
            propertyChanged: (target, oldValue, newValue) => ((ValuePicker)target).OnSelectedValuePathChanged((string)oldValue, (string)newValue));

        private void OnSelectedValuePathChanged(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                _selectecValuePathGetter = null;
            }
        }

        public string SelectedValuePath
        {
            get => (string)GetValue(SelectedValuePathProperty);
            set => SetValue(SelectedValuePathProperty, value);
        }

        private MethodInfo _selectecValuePathGetter;

        #endregion SelectedValuePath Property

        #region SelectedIndex Property

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(ValuePicker),
            -1,
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) => ((ValuePicker)bindable).OnSelectedIndexChanged((int)oldValue, (int)newValue),
            coerceValue: (bindable, value) => ((ValuePicker)bindable).CoerceSelectedIndex(value),
            validateValue: (bindable, value) => ((ValuePicker)bindable).ValidateSelectedIndex(value));

        private bool ValidateSelectedIndex(object value)
        {
            return ((int)value) >= -1;
        }

        private object CoerceSelectedIndex(object value)
        {
            if (_isSelectionChanging) { return value; }

            if (value is int i && i > Items.Count)
            {
                return BindableProperty.UnsetValue;
            }
            return value;
        }

        private void OnSelectedIndexChanged(int oldValue, int newValue)
        {
            if (_isSelectionChanging) return;

            var items = Items;
            if (newValue > -1 && newValue < items.Count)
            {
                SelectItem(items[newValue]);
            }
        }

        public int SelectedIndex
        {
            get => (int)GetValue(ValuePicker.SelectedIndexProperty);
            set => SetValue(ValuePicker.SelectedIndexProperty, value);
        }

        #endregion SelectedIndex Property

        //public static readonly BindableProperty ValueSourceProperty = BindableProperty.Create(
        //    nameof(ValueSource),
        //    typeof(IList),
        //    typeof(ValuePicker),
        //    (object)null,
        //    propertyChanged: (target, oldValue, newValue) => ((ValuePicker)target).OnValueSourceChanged((IList)oldValue, (IList)newValue));

        //public IList ValueSource
        //{
        //    get => (IList)((BindableObject)this).GetValue(ValuePicker.ValueSourceProperty);
        //    set => ((BindableObject)this).SetValue(ValuePicker.ValueSourceProperty, (object)value);
        //}

        //protected virtual void OnValueSourceChanged(IList oldValue, IList newValue)
        //{
        //}

        #region SelectedItem property

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(ValuePicker),
            default(object),
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) => ((ValuePicker)bindable).OnSelectedItemChanged(oldValue, newValue),
            coerceValue: (bindable, value) => ((ValuePicker)bindable).CoerceSelectedItem(value)
            );

        private object CoerceSelectedItem(object value)
        {
            if (value == null || _skipCoerceSelectedItem) { return value; }

            // check Items contains value, but optimize by checking item at selected index first
            var selectedIndex = SelectedIndex;
            if ((selectedIndex > -1 && selectedIndex < Items.Count && Items[selectedIndex] == value)
                || Items.Contains(value))
            {
                return SelectItem(value);
            }

            return BindableProperty.UnsetValue;
        }

        private void OnSelectedItemChanged(object oldValue, object newValue)
        {
            if (!_isSelectionChanging)
            {
                newValue = SelectItem(newValue);
            }

            Text = GetItemDisplayValue(newValue);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        // can we simplify and just use _isSelectionChanging
        private bool _skipCoerceSelectedItem;

        #endregion SelectedItem property

        #region Text Property

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(String),
            typeof(ValuePicker),
            default(string),
            propertyChanged: (bindable, oldValue, newValue) => ((ValuePicker)bindable).OnTextChanged(oldValue, newValue)
            );

        private void OnTextChanged(object oldValue, object newValue)
        {
            // don't do anything
            // when text updated it should only be reflected in by changed in the ui,
            // it should not result in update to selectedIndex, SelectedItem or SelectedValue
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        //bool _isUpdatingText;

        #endregion Text Property

        #region ItemSource property

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(ValuePicker),
            default(IList),
            propertyChanged: (bindable, oldValue, newValue) => ((ValuePicker)bindable).OnItemsSourceChanged((IList)oldValue, (IList)newValue)
            );

        private void OnItemsSourceChanged(IList oldValue, IList newValue)
        {
            var oldObservable = oldValue as INotifyCollectionChanged;
            if (oldObservable != null)
                oldObservable.CollectionChanged -= ItemSource_CollectionChanged;

            var newObservable = newValue as INotifyCollectionChanged;
            if (newObservable != null)
            {
                newObservable.CollectionChanged += ItemSource_CollectionChanged;
            }

            if (newValue != null)
            {
                ResetItems();
            }
            else
            {
                Items.Clear();
            }

            void ItemSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddItems(e);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        RemoveItems(e);
                        break;

                    default: //Move, Replace, Reset
                        ResetItems();
                        break;
                }
            }

            void AddItems(NotifyCollectionChangedEventArgs e)
            {
                var items = Items;
                int index = e.NewStartingIndex < 0 ? items.Count : e.NewStartingIndex;
                foreach (object newItem in e.NewItems)
                {
                    items.Insert(index++, newItem);
                }
            }

            void RemoveItems(NotifyCollectionChangedEventArgs e)
            {
                var items = Items;
                int index = e.OldStartingIndex < items.Count ? e.OldStartingIndex : items.Count;
                foreach (object _ in e.OldItems)
                {
                    items.RemoveAt(index--);
                }
            }

            void ResetItems()
            {
                var items = Items;
                if (ItemsSource == null)
                    return;
                items.Clear();
                foreach (object item in ItemsSource)
                {
                    items.Add(item);
                }

                CoerceValue(SelectedIndexProperty);
                CoerceValue(SelectedItemProperty);

                if (_selectedValueWaitsForItems
                    && !Object.Equals(SelectedValue, GetItemValue(PendingSelectedItem)))
                {
                    SelectItemWithValue(SelectedValue, true);
                }
            }
        }

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IList Items { get; } = new List<object>();

        public bool HasItems => Items.Count > 0;

        #endregion ItemSource property

        #region Auxiliary action property

        public static readonly BindableProperty AuxiliaryActionHeadingProperty = BindableProperty.Create(
            nameof(AuxiliaryActionHeading),
            typeof(string),
            typeof(ValuePicker),
            default(string));

        public string AuxiliaryActionHeading
        {
            get => (string)GetValue(AuxiliaryActionHeadingProperty);
            set => SetValue(AuxiliaryActionHeadingProperty, value);
        }

        public static readonly BindableProperty AuxiliaryActionCommandProperty = BindableProperty.Create(
            nameof(AuxiliaryActionCommand),
            typeof(ICommand),
            typeof(ValuePicker),
            default(ICommand));

        public ICommand AuxiliaryActionCommand
        {
            get => (ICommand)GetValue(AuxiliaryActionCommandProperty);
            set => SetValue(AuxiliaryActionCommandProperty, value);
        }

        public event EventHandler AuxiliaryActionClicked;

        #endregion

        private object PendingSelectByValueItem { get; set; } // used in OnSelectedValueChanged, otherwise PendingSelectedItem is used
        private object PendingSelectedItem { get; set; }

        private bool _isSelectionChanging;
        private bool _selectedValueDrivesSelection;
        private bool _selectedValueWaitsForItems;

        public async void OnClick()
        {
            IList items = Items;
            if (items == null)
                return;
            int count = items.Count;
            if (count == 0)
                return;
            string[] displayValues = new string[items.Count];
            for (int index = 0; index < count; ++index)
                displayValues[index] = GetItemDisplayValue(items[index]) ?? "";
            string cancel = "Cancel";
            string distruction = (AuxiliaryActionHeading.IsNullOrEmpty()) ? (string)null : AuxiliaryActionHeading;
            string str = await this.DisplayActionSheet(this.Title ?? "", cancel, distruction, displayValues);
            if (str == null || !(str != cancel))
                return;
            if (str == distruction)
            {
                AuxiliaryActionCommand?.Execute(this);
                AuxiliaryActionClicked?.Invoke(this, EventArgs.Empty);
                return;
            }
            int index1 = EnumerableExtensions.IndexOf<string>(displayValues, str);
            SelectedIndex = index1;
        }

        private Task<string> DisplayActionSheet(
          string title,
          string cancel,
          string destruction,
          params string[] buttons)
        {
            ActionSheetArguments actionSheetArguments = new ActionSheetArguments(title, cancel, destruction, (IEnumerable<string>)buttons);
            MessagingCenter.Send<Page, ActionSheetArguments>((this.GetPage() ?? throw new ArgumentNullException("page")),
                "Xamarin.ShowActionSheet",
                actionSheetArguments);
            return actionSheetArguments.Result.Task;
        }

        protected void UpdateSelection()
        {
            Debug.Assert(_isSelectionChanging);

            var desiredSelectedItem = PendingSelectedItem;
            if (SelectedItem != desiredSelectedItem)
            {
                try
                {
                    _skipCoerceSelectedItem = true;
                    if (desiredSelectedItem != BindableProperty.UnsetValue)
                    { SelectedItem = desiredSelectedItem; }
                    else
                    { ClearValue(SelectedItemProperty); }
                }
                finally
                {
                    _skipCoerceSelectedItem = false;
                }
            }

            var selectedIndex = SelectedIndex;
            if ((selectedIndex > Items.Count - 1) // out of range?
                || (selectedIndex == -1 && desiredSelectedItem != null) // dirty selected item?
                || (selectedIndex > -1 // inrange but not updated selected item or dirty selected index
                    && (desiredSelectedItem == null || selectedIndex != Items.IndexOf(desiredSelectedItem))))
            {
                // selected index needs to be updated
                SelectedIndex = CalculateSelectedIndex();
            }

            if (desiredSelectedItem != null)
            {
                _selectedValueWaitsForItems = false;
            }

            if (!_selectedValueDrivesSelection && !_selectedValueWaitsForItems)
            {
                var desiredSelectedValue = GetItemValue(desiredSelectedItem); // returns null if item is unset item
                if (!Object.Equals(SelectedValue, desiredSelectedValue))
                {
                    SelectedValue = desiredSelectedValue;
                }
            }
        }

        private int CalculateSelectedIndex()
        {
            var selectedItem = SelectedItem;
            if (selectedItem == null)
                return -1;

            var index = Items.IndexOf(selectedItem);

            return index;
        }

        protected void SelectItemAtIndex(int index)
        {
            var items = Items;
            if (index < 0 || index > items.Count - 1) { throw new IndexOutOfRangeException(); }

            var item = items[index];
            SelectItem(item);
        }

        protected object SelectItem(object item)
        {
            try
            {
                _isSelectionChanging = true;

                PendingSelectedItem = item;
                UpdateSelection();

                return item;
            }
            finally
            {
                _isSelectionChanging = false;
            }
        }

        protected object SelectItemWithValue(object value, bool selectNow)
        {
            object item;
            if (HasItems)
            {
                item = FindItemWithValue(value, out var index);

                if (selectNow)
                {
                    try
                    {
                        _selectedValueDrivesSelection = true;
                        return SelectItem(item);
                    }
                    finally
                    { _selectedValueDrivesSelection = false; }
                }
                else
                {
                    PendingSelectByValueItem = item;
                }
            }
            else
            {
                item = BindableProperty.UnsetValue;
                _selectedValueWaitsForItems = true;
            }
            return item;
        }

        protected object FindItemWithValue(object value, out int index)
        {
            index = -1;

            if (!HasItems) return BindableProperty.UnsetValue;

            if (string.IsNullOrEmpty(SelectedValuePath))
            {
                index = 0;
                foreach (var item in Items)
                {
                    if (Object.Equals(item, value))
                    {
                        return item;
                    }
                    index++;
                }
                index = -1;
                return BindableProperty.UnsetValue;
            }

            var getter = PrepareValueGetter(Items.OfType<object>().FirstOrDefault(x => x != null));
            index = 0;
            foreach (var item in Items)
            {
                var itemValue = getter.Invoke(item, new object[] { });
                if (Object.Equals(value, itemValue))
                {
                    return item;
                }
                index++;
            }
            index = -1;
            return BindableProperty.UnsetValue;
        }

        protected object GetItemValue(object item)
        {
            if (item == null || item == BindableProperty.UnsetValue) return null;

            var getter = PrepareValueGetter(item);
            if (getter != null)
            {
                return getter.Invoke(item, new object[] { });
            }
            return item;
        }

        protected string GetItemDisplayValue(object item)
        {
            return item?.ToString();
        }

        protected MethodInfo PrepareValueGetter(object item)
        {
            var path = SelectedValuePath;
            if (String.IsNullOrEmpty(path)) { return null; }

            TypeInfo currentType = (item is IReflectableType rItem) ? rItem.GetTypeInfo() : item.GetType().GetTypeInfo();
            if (_selectecValuePathGetter != null)
            {
                var getterDeclaringType = _selectecValuePathGetter.DeclaringType;
                if (getterDeclaringType.IsAssignableFrom(currentType)) { return _selectecValuePathGetter; }
            }

            PropertyInfo property = null;
            while (currentType != null && property == null)
            {
                property = currentType.GetDeclaredProperty(path);
                currentType = currentType.BaseType?.GetTypeInfo();
            }

            if (property != null)
            {
                _selectecValuePathGetter = property.GetMethod;
            }

            return _selectecValuePathGetter;
        }
    }
}