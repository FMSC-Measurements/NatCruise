using FScruiser.Maui.Services;

namespace FScruiser.Maui.Controls;

public static class TapGestureRecognizerHelper
{
    public static ISoundService? SoundService { get; set; }

    public static readonly BindableProperty EnableHapticFeedbackProperty = BindableProperty.CreateAttached(
        "EnableHapticFeedback",
        typeof(bool),
        typeof(TapGestureRecognizerHelper),
        false,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: OnEnableHapticFeedbackChanged);

    public static bool GetEnableHapticFeedback(BindableObject target)
    {
        return (bool)target.GetValue(EnableHapticFeedbackProperty);
    }

    public static void SetEnableHapticFeedback(BindableObject element, bool value)
    {
        element.SetValue(EnableHapticFeedbackProperty, value);
    }

    private static void OnEnableHapticFeedbackChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var gestureRecognizer = (TapGestureRecognizer)bindable;

        if ((bool)oldValue && !(bool)newValue)
        {
            gestureRecognizer.Tapped -= EnableHapticFeedBack_HandelTapped;
        }
        else if (!(bool)oldValue && (bool)newValue)
        {
            gestureRecognizer.Tapped += EnableHapticFeedBack_HandelTapped;
        }
    }

    private static void EnableHapticFeedBack_HandelTapped(object? sender, EventArgs e)
    {
        HapticFeedback.Perform(HapticFeedbackType.Click);
    }

    public static readonly BindableProperty EnableClickSoundProperty = BindableProperty.CreateAttached(
        "EnableClickSound",
        typeof(bool),
        typeof(TapGestureRecognizerHelper),
        false,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: OnEnableClickSoundChanged
        );

    public static bool GetEnableClickSound(BindableObject target) => (bool)target.GetValue(EnableClickSoundProperty);

    public static void SetEnableClickSound(BindableObject target, bool value) => target.SetValue(EnableClickSoundProperty, value);

    private static void OnEnableClickSoundChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var gestureRecognizer = (TapGestureRecognizer)bindable;

        if ((bool)oldValue && !(bool)newValue)
        {
            gestureRecognizer.Tapped -= EnableClickSound_HandelTapped;
        }
        else if (!(bool)oldValue && (bool)newValue)
        {
            gestureRecognizer.Tapped += EnableClickSound_HandelTapped;
        }
    }

    private static void EnableClickSound_HandelTapped(object? sender, EventArgs e)
    {
        SoundService?.PlayClickSound();
    }
}