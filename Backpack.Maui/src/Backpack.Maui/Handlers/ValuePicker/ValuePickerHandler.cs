using Backpack.Maui.Controls;
using Backpack.Maui.Extensions;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Collections;
using System.Linq;

#if __IOS__ || MACCATALYST
using PlatformView = Microsoft.Maui.Platform.MauiPicker;
#elif ANDROID
using PlatformView = Microsoft.Maui.Platform.MauiPicker;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.Controls.ComboBox;
#elif TIZEN
using PlatformView = Tizen.UIExtensions.NUI.Entry;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID && !TIZEN)
using PlatformView = System.Object;
#endif

namespace Backpack.Maui.Handlers.ValuePicker;

public partial class ValuePickerHandler : ViewHandler<IValuePicker, PlatformView>, IViewHandler
{
    public static readonly IPropertyMapper<IValuePicker, ValuePickerHandler> Mapper = new PropertyMapper<IValuePicker, ValuePickerHandler>(ViewMapper)
    {
#if __ANDROID__ || WINDOWS
			[nameof(IValuePicker.Background)] = MapBackground,
#endif
        [nameof(IValuePicker.CharacterSpacing)] = MapCharacterSpacing,
        [nameof(IValuePicker.Font)] = MapFont,
        [nameof(IValuePicker.SelectedIndex)] = MapSelectedIndex,
        [nameof(IValuePicker.TextColor)] = MapTextColor,
        [nameof(IValuePicker.Title)] = MapTitle,
        [nameof(IValuePicker.TitleColor)] = MapTitleColor,
        [nameof(IValuePicker.HorizontalTextAlignment)] = MapHorizontalTextAlignment,
        [nameof(IValuePicker.VerticalTextAlignment)] = MapVerticalTextAlignment,
        [nameof(IValuePicker.Items)] = MapItems,
    };

    public static readonly CommandMapper<IValuePicker, ValuePickerHandler> CommandMapper = new(ViewCommandMapper)
    {
    };

    public ValuePickerHandler() : base(Mapper, CommandMapper)
    {
    }

    public ValuePickerHandler(IPropertyMapper? mapper)
    : base(mapper ?? Mapper, CommandMapper)
    {
    }

    public ValuePickerHandler(IPropertyMapper mapper, CommandMapper? commandMapper = null) : base(mapper, commandMapper)
    {
    }

    //IValuePicker VirtualView => VirtualView;

    //PlatformView PlatformView => PlatformView;

    public async void OnClick(object sender, EventArgs ea)
    {
        var virtualView = VirtualView;

        int count = virtualView.GetCount();
        if (count == 0)
            return;
        string[] displayValues = ((IPicker)virtualView).Items.Select(x => x ?? "").ToArray();
        string cancel = "Cancel";
        string? auxAction = (virtualView.AuxiliaryActionHeading.IsNullOrEmpty()) ? (string)null : virtualView.AuxiliaryActionHeading;
        string str = await DisplayActionSheet(virtualView.Title ?? "", cancel, auxAction, displayValues);
        if (str == null || str == cancel)
            return;
        if (str == auxAction)
        {
            virtualView.AuxiliaryActionCommand?.Execute(this);
            virtualView.RaiseAuxiliaryActionClicked();
            return;
        }
        int index1 = displayValues.IndexOf(str);
        virtualView.SelectedIndex = index1;
    }

    private Task<string> DisplayActionSheet(
          string title,
          string cancel,
          string destruction,
          params string[] buttons)
    {
        var page = base.VirtualView.GetPage();
        return page.DisplayActionSheet(title, cancel, destruction, buttons);

    }


}

