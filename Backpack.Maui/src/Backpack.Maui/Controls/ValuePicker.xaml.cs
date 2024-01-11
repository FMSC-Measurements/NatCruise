using Microsoft.Maui.Controls.Internals;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Input;
using System.Diagnostics;
using System.Reflection;
using Backpack.Maui.Extensions;

namespace Backpack.Maui.Controls;

public partial class ValuePicker : View, IValuePicker, IFontElement
{
    #region IFontElement properties

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(IFontElement),
        (object)FontAttributes.None,
        propertyChanged: OnFontAttributesChanged);

    private static void OnFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        throw new NotImplementedException();
    }

    public FontAttributes FontAttributes
    {
        get { return (FontAttributes)GetValue(FontAttributesProperty); }
        set { SetValue(FontAttributesProperty, value); }
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(IFontElement),
        default(string),
        propertyChanged: (BindableObject bindable, object oldValue, object newValue)
            => ((IFontElement)bindable).OnFontFamilyChanged((string)oldValue, (string)newValue));

    public string FontFamily
    {
        get { return (string)GetValue(FontFamilyProperty); }
        set { SetValue(FontFamilyProperty, value); }
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(IFontElement),
        (object)-1.0,
        propertyChanged: (BindableObject bindable, object oldValue, object newValue)
            => ((IFontElement)bindable).OnFontSizeChanged((double)oldValue, (double)newValue),
        defaultValueCreator: (BindableObject bindable)
            => ((IFontElement)bindable).FontSizeDefaultValueCreator());

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public static readonly BindableProperty FontAutoScalingEnabledProperty =
            BindableProperty.Create("FontAutoScalingEnabled",
                typeof(bool),
                typeof(IFontElement),
                true,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue)
                     => ((IFontElement)bindable).OnFontAutoScalingEnabledChanged((bool)oldValue, (bool)newValue));

    public bool FontAutoScalingEnabled
    {
        get => (bool)GetValue(FontAutoScalingEnabledProperty);
        set => SetValue(FontAutoScalingEnabledProperty, value);
    }

    double IFontElement.FontSizeDefaultValueCreator() =>
            GetDefaultFontSize();

    double GetDefaultFontSize()
            => this.FindMauiContext()?.Services?.GetService<IFontManager>()?.DefaultFontSize ?? 0d;

    void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
            HandleFontChanged();

    void IFontElement.OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue) =>
        HandleFontChanged();

    void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
            HandleFontChanged();

    void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
        HandleFontChanged();

    void HandleFontChanged()
    {
        Handler?.UpdateValue(nameof(ITextStyle.Font));
        InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
    }

    #endregion IFontElement properties

    #region IPicker properties
    IList<string> IPicker.Items => this.Items.OfType<object>().Select(x => GetItemDisplayValue(x)).ToList();

    Microsoft.Maui.Font ITextStyle.Font => this.ToFont();

    int IItemDelegate<string>.GetCount()
    {
        return Items?.Count ?? ItemsSource?.Count ?? 0;
    }

    string IItemDelegate<string>.GetItem(int index) => GetItemDisplayValue(index);
    #endregion

    #region ITextElement properties
    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(ValuePicker.CharacterSpacing), 
        typeof(double), 
        typeof(ValuePicker), 
        0.0d,
        propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            ((ValuePicker)bindable).OnCharacterSpacingPropertyChanged((double)oldValue, (double)newValue));

    void OnCharacterSpacingPropertyChanged(double oldValue, double newValue) => InvalidateMeasure();

    public double CharacterSpacing
    {
        get { return (double)GetValue(CharacterSpacingProperty); }
        set { SetValue(CharacterSpacingProperty, value); }
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(ValuePicker.TextColor), 
        typeof(Color), 
        typeof(ValuePicker), 
        default(Color));

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    //public static readonly BindableProperty TextTransformProperty =
    //        BindableProperty.Create(nameof(ITextElement.TextTransform), typeof(TextTransform), typeof(ITextElement), TextTransform.Default,
    //                        propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
    //                        ((ITextElement)bindable).OnTextTransformChanged((TextTransform)oldValue, (TextTransform)newValue));

    //void ITextElement.OnTextTransformChanged(TextTransform oldValue, TextTransform newValue) =>
    //        InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);

    //TextTransform ITextElement.TextTransform
    //{
    //    get => TextTransform.Default;
    //    set { }
    //}

    #endregion

    #region ITextAlignment properties

    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
        nameof(ValuePicker.HorizontalTextAlignment),
        typeof(TextAlignment),
        typeof(ValuePicker),
        TextAlignment.Start,
        propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            ((ValuePicker)bindable).OnHorizontalTextAlignmentPropertyChanged((TextAlignment)oldValue, (TextAlignment)newValue));

    private void OnHorizontalTextAlignmentPropertyChanged(TextAlignment oldValue, TextAlignment newValue)
    {
        // do nothing
    }

    public TextAlignment HorizontalTextAlignment
    {
        get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
        set { SetValue(HorizontalTextAlignmentProperty, value); }
    }

    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(
        nameof(ValuePicker.VerticalTextAlignment),
        typeof(TextAlignment),
        typeof(ValuePicker),
        TextAlignment.Center);

    public TextAlignment VerticalTextAlignment
    {
        get { return (TextAlignment)GetValue(VerticalTextAlignmentProperty); }
        set { SetValue(VerticalTextAlignmentProperty, value); }
    }

    #endregion


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

    /// <summary>
    /// Add a optional additional action to the value picker. 
    /// Indented use is to allow the user add a new item to the list, or manually enter a value.
    /// Use <see cref="AuxiliaryActionHeading"/> to customize button text for action.
    /// </summary>
    public ICommand AuxiliaryActionCommand
    {
        get => (ICommand)GetValue(AuxiliaryActionCommandProperty);
        set => SetValue(AuxiliaryActionCommandProperty, value);
    }

    public event EventHandler AuxiliaryActionClicked;

    void IValuePicker.RaiseAuxiliaryActionClicked()
    {
        AuxiliaryActionClicked?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    private object PendingSelectByValueItem { get; set; } // used in OnSelectedValueChanged, otherwise PendingSelectedItem is used
    private object PendingSelectedItem { get; set; }

    private bool _isSelectionChanging;
    private bool _selectedValueDrivesSelection;
    private bool _selectedValueWaitsForItems;

    public ValuePicker()
    {
        InitializeComponent();
    }


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
        string auxAction = (AuxiliaryActionHeading.IsNullOrEmpty()) ? (string)null : AuxiliaryActionHeading;
        string str = await this.DisplayActionSheet(this.Title ?? "", cancel, auxAction, displayValues);
        if (str == null || str == cancel)
            return;
        if (str == auxAction)
        {
            AuxiliaryActionCommand?.Execute(this);
            AuxiliaryActionClicked?.Invoke(this, EventArgs.Empty);
            return;
        }
        int index1 = displayValues.IndexOf(str);
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
        if (index < 0 || index > items.Count - 1) { throw new ArgumentOutOfRangeException(); }

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

    //string? IValuePicker.GetItemDisplayValue(object item)
    //{
    //    return this.GetItemDisplayValue(item);
    //}

    string GetItemDisplayValue(int index)
    {
        if (index < 0)
            return string.Empty;

        var items = Items;
        if (items != null && index < items?.Count)
            return GetItemDisplayValue(items[index]);

        var itemSource = ItemsSource;
        if (itemSource != null && index < itemSource.Count)
            return GetItemDisplayValue(itemSource[index]);
        return string.Empty;
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