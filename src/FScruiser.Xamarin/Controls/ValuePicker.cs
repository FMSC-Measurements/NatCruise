using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FScruiser.XF.Controls
{
    public class ValuePicker : View, IFontElement
    {
        public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create(
            nameof(SelectedValue),
            typeof(object),
            typeof(ValuePicker),
            (object)null,
            BindingMode.TwoWay,
            (BindableProperty.ValidateValueDelegate)null,
            (target, oldValue, newValue) => ((ValuePicker)target).OnSelectedValueChanged(oldValue, newValue),
            (target, oldValue, newValue) => ((ValuePicker)target).OnSelectedValueChanging(oldValue, newValue),
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty ValueSourceProperty = BindableProperty.Create(
            nameof(ValueSource),
            typeof(IList),
            typeof(ValuePicker),
            (object)null,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            (target, oldValue, newValue) => ((ValuePicker)target).OnValueSourceChanged((IList)oldValue, (IList)newValue),
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(ValuePicker),
            (object)Color.Default,
            (BindingMode)2,
            (BindableProperty.ValidateValueDelegate)null,
            (target, oldValue, newValue) => ((ValuePicker)target).OnTextColorPropertyChanged((Color)oldValue, (Color)newValue),
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty FontProperty = BindableProperty.Create(
            "Font",
            typeof(Font),
            typeof(IFontElement),
            (object)new Font(),
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            OnFontPropertyChanged,
            (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            nameof(FontFamily),
            typeof(string),
            typeof(IFontElement),
            (object)null,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            OnFontFamilyChanged,
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            nameof(FontSize),
            typeof(double),
            typeof(IFontElement),
            (object)-1.0,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            OnFontSizeChanged,
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            FontSizeDefaultValueCreator);

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
            nameof(FontAttributes),
            typeof(FontAttributes),
            typeof(IFontElement),
            (object)FontAttributes.None,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            OnFontAttributesChanged,
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        private static readonly BindableProperty CancelEventsProperty = BindableProperty.Create(
            "CancelEvents",
            typeof(bool),
            typeof(ValuePicker),
            (object)false,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            (BindableProperty.BindingPropertyChangedDelegate)null,
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(ValuePicker),
            (object)null,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            (BindableProperty.BindingPropertyChangedDelegate)null,
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
            nameof(TitleColor),
            typeof(Color),
            typeof(ValuePicker),
            (object)new Color(),
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            (BindableProperty.BindingPropertyChangedDelegate)null,
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        protected virtual void OnSelectedValueChanging(object oldValue, object newValue)
        {
        }

        protected virtual void OnSelectedValueChanged(object oldValue, object newValue)
        {
        }

        public object SelectedValue
        {
            get => ((BindableObject)this).GetValue(ValuePicker.SelectedValueProperty);
            set => ((BindableObject)this).SetValue(ValuePicker.SelectedValueProperty, value);
        }

        protected virtual void OnValueSourceChanged(IList oldValue, IList newValue)
        {
        }

        public IList ValueSource
        {
            get => (IList)((BindableObject)this).GetValue(ValuePicker.ValueSourceProperty);
            set => ((BindableObject)this).SetValue(ValuePicker.ValueSourceProperty, (object)value);
        }

        private void OnTextColorPropertyChanged(Color oldValue, Color newValue)
        {
        }

        public Color TextColor
        {
            get => (Color)((BindableObject)this).GetValue(ValuePicker.TextColorProperty);
            set => ((BindableObject)this).SetValue(ValuePicker.TextColorProperty, (object)value);
        }

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

        public async void OnClick()
        {
            IList values = this.ValueSource;
            if (values == null)
                return;
            int count = values.Count;
            if (count == 0)
                return;
            string[] itemValues = new string[values.Count];
            for (int index = 0; index < count; ++index)
                itemValues[index] = values[index].ToString();
            string cancel = "Cancel";
            string str = await this.DisplayActionSheet(this.Title ?? "", cancel, (string)null, itemValues);
            if (str == null || !(str != cancel))
                return;
            int index1 = EnumerableExtensions.IndexOf<string>(itemValues, str);
            this.SelectedValue = values[index1];
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
    }
}