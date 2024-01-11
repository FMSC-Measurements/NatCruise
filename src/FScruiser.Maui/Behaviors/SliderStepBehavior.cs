namespace FScruiser.Maui.Behaviors;

public class SliderStepBehavior : Behavior<Slider>
{
    private bool _courcingSliderValue;

    protected override void OnAttachedTo(Slider slider)
    {
        base.OnAttachedTo(slider);

        slider.ValueChanged += Slider_ValueChanged;
    }

    private void Slider_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if(sender is null) return;
        if (_courcingSliderValue) { return; }

        try
        {
            _courcingSliderValue = true;
            var slider = (Slider)sender;
            var value = e.NewValue;
            slider.Value = Math.Round(value, 1);
        }
        finally { _courcingSliderValue = false; }
    }

    protected override void OnDetachingFrom(Slider slider)
    {
        base.OnDetachingFrom(slider);
    }
}