using Backpack.Maui.Controls;
using Java.Time.Format;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack.Maui.Handlers.ValuePicker;

public partial class ValuePickerHandler : ViewHandler<IValuePicker, MauiPicker>
{
    protected override MauiPicker CreatePlatformView() =>
            new MauiPicker(Context);

    protected override void ConnectHandler(MauiPicker platformView)
    {
        platformView.FocusChange += OnFocusChange;
        platformView.Click += OnClick;
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(MauiPicker platformView)
    {
        platformView.FocusChange -= OnFocusChange;
        platformView.Click -= OnClick;

        base.DisconnectHandler(platformView);
    }

    public static void MapBackground(ValuePickerHandler handler, IPicker picker)
    {
        handler.PlatformView?.UpdateBackground(picker);
    }

    internal static void MapItems(ValuePickerHandler handler, IPicker picker) => Reload(handler);

    private static void Reload(ValuePickerHandler handler)
    {
        // todo replace logic from PickerExtensions with coustom logic

        UpdatePicker(handler.PlatformView, handler.VirtualView);
    }

    private static void UpdatePicker(MauiPicker platformPicker, IValuePicker picker)
    {
        platformPicker.Hint = picker.Title;
        platformPicker.Text = picker.Text;
    }

    public static void MapTitle(ValuePickerHandler handler, IValuePicker picker)
    {
        UpdatePicker(handler.PlatformView, handler.VirtualView);
    }

    public static void MapTitleColor(ValuePickerHandler handler, IValuePicker picker)
    {
        PickerExtensions.UpdateTitleColor(handler.PlatformView, picker);
    }

    public static void MapSelectedIndex(ValuePickerHandler handler, IValuePicker picker)
    {
        UpdatePicker(handler.PlatformView, handler.VirtualView);
    }

    public static void MapCharacterSpacing(ValuePickerHandler handler, IValuePicker picker)
    {
        handler.PlatformView.LetterSpacing = picker.CharacterSpacing.ToEm();
    }

    public static void MapFont(ValuePickerHandler handler, IPicker picker)
    {
        var services = handler.Services ?? throw new InvalidOperationException("Unable to find service provider");
        var fontManager = services.GetRequiredService<IFontManager>();

        handler.PlatformView?.UpdateFont(picker, fontManager);
    }

    public static void MapHorizontalTextAlignment(ValuePickerHandler handler, IPicker picker)
    {
        TextViewExtensions.UpdateHorizontalTextAlignment(handler.PlatformView, picker);
    }

    public static void MapTextColor(ValuePickerHandler handler, IPicker picker)
    {
        TextViewExtensions.UpdateTextColor(handler.PlatformView, picker);
    }

    public static void MapVerticalTextAlignment(ValuePickerHandler handler, IPicker picker)
    {
        TextAlignmentExtensions.UpdateVerticalAlignment(handler.PlatformView, picker.VerticalTextAlignment);
    }

    private void OnFocusChange(object? sender, global::Android.Views.View.FocusChangeEventArgs e)
    {
        if (PlatformView == null)
            return;

        if (e.HasFocus)
        {
            if (PlatformView.Clickable)
                PlatformView.CallOnClick();
            else
                OnClick(VirtualView, EventArgs.Empty);
        }
        //else if (_dialog != null)
        //{
        //    _dialog.Hide();
        //    _dialog = null;
        //}
    }
}