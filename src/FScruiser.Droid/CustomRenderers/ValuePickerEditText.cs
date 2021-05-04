using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace FScruiser.Droid.CustomRenderers
{
    public class ValuePickerEditText : EditText
    {
        public ValuePickerEditText(Context context)
          : base(context)
        {
            this.SetOnClickListener((View.IOnClickListener)ValuePickerEditText.ValuePickerClickListener.Instance);
        }

        protected override void OnFocusChanged(
          bool gainFocus,
          [GeneratedEnum] FocusSearchDirection direction,
          Rect previouslyFocusedRect)
        {
            base.OnFocusChanged(gainFocus, direction, previouslyFocusedRect);
            if (!gainFocus)
                return;
            if (this.Parent is ValuePickerRenderer parent2)
            {
                parent2.OnClick();
            }
            else
            {
                if (!(this.Parent?.Parent?.Parent is ValuePickerRenderer parent3))
                    return;
                parent3.OnClick();
            }
        }

        private class ValuePickerClickListener : Java.Lang.Object, View.IOnClickListener, IJavaObject, IDisposable
        {
            public static readonly ValuePickerEditText.ValuePickerClickListener Instance = new ValuePickerEditText.ValuePickerClickListener();

            public void OnClick(View v)
            {
                if (v?.Parent is ValuePickerRenderer parent2)
                {
                    parent2.OnClick();
                }
                else
                {
                    if (!(v?.Parent?.Parent?.Parent is ValuePickerRenderer parent3))
                        return;
                    parent3.OnClick();
                }
            }
        }
    }
}
