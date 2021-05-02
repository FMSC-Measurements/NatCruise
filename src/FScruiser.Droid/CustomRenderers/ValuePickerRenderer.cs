using Android.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using FScruiser.XF.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ValuePicker), typeof(FScruiser.Droid.CustomRenderers.ValuePickerRenderer))]
namespace FScruiser.Droid.CustomRenderers
{
    
    public class ValuePickerRenderer : ViewRenderer<ValuePicker, EditText>
    {
        private int _originalHintTextColor;
        private static readonly HashSet<Keycode> availableKeys = new HashSet<Keycode>((IEnumerable<Keycode>)new Keycode[7]
        {
      Keycode.Tab,
      Keycode.Forward,
      Keycode.Back,
      Keycode.DpadDown,
      Keycode.DpadLeft,
      Keycode.DpadRight,
      Keycode.DpadUp
        });

        public ValuePickerRenderer(Context context)
          : base(context)
        {
        }

        protected override EditText CreateNativeControl()
        {
            ValuePickerEditText valuePickerEditText = new ValuePickerEditText(Context);
            valuePickerEditText.Focusable = true;
            valuePickerEditText.Clickable = true;
            valuePickerEditText.InputType = InputTypes.DatetimeVariationNormal;
            valuePickerEditText.KeyPress += new EventHandler<Android.Views.View.KeyEventArgs>(ValuePickerRenderer.OnKeyPress);
            return (EditText)valuePickerEditText;
        }

        private static void OnKeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            if (ValuePickerRenderer.availableKeys.Contains(e.KeyCode))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                if (!(sender is Android.Views.View view2))
                    return;
                view2.CallOnClick();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ValuePicker> e)
        {
            ValuePicker oldElement = e.OldElement;
            if (e.NewElement != null)
            {
                if (this.Control == null)
                {
                    EditText nativeControl = CreateNativeControl();
                    this.SetNativeControl(nativeControl);
                    this._originalHintTextColor = nativeControl.CurrentHintTextColor;
                }
                this.UpdateFont();
                this.UpdateValuePicker();
                this.UpdateTextColor();
            }
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            string propertyName = e.PropertyName;
            if (propertyName == ValuePicker.TitleProperty.PropertyName || propertyName == ValuePicker.TitleColorProperty.PropertyName)
                this.UpdateValuePicker();
            else if (propertyName == ValuePicker.SelectedValueProperty.PropertyName)
                this.UpdateValuePicker();
            else if (propertyName == ValuePicker.TextColorProperty.PropertyName)
            {
                this.UpdateTextColor();
            }
            else
            {
                if (!(propertyName == ValuePicker.FontAttributesProperty.PropertyName) && !(propertyName == ValuePicker.FontFamilyProperty.PropertyName) && (!(propertyName == ValuePicker.FontSizeProperty.PropertyName) && !(propertyName == ValuePicker.FontProperty.PropertyName)))
                    return;
                this.UpdateFont();
            }
        }

        private void UpdateFont() => this.Control.SetTextSize(ComplexUnitType.Sp, (float)((VisualElementRenderer<ValuePicker>)this).Element.FontSize);

        private void UpdateValuePicker()
        {
            EditText control = this.Control;
            ValuePicker element = ((VisualElementRenderer<ValuePicker>)this).Element;
            control.Hint = element.Title;
            if (((BindableObject)((VisualElementRenderer<ValuePicker>)this).Element).IsSet(ValuePicker.TitleColorProperty))
                this.Control.SetHintTextColor(ColorExtensions.ToAndroid(element.TitleColor));
            else
                this.Control.SetHintTextColor(new Android.Graphics.Color(this._originalHintTextColor));
            string text1 = control.Text;
            object selectedValue = element.SelectedValue;
            if (selectedValue == null)
                control.Text = (string)null;
            else
                control.Text = selectedValue.ToString();
            string text2 = control.Text;
            if (text1 == text2)
                return;
            ((IVisualElementController)element).NativeSizeChanged();
        }

        private void UpdateTextColor()
        {
        }

        internal void OnClick() => ((VisualElementRenderer<ValuePicker>)this).Element?.OnClick();
    }
}
