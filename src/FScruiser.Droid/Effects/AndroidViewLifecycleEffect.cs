using Android.Views;
using FScruiser.XF.Effects;
using System.Linq;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ResolutionGroupName(ViewLifecycleEffect.EffectGroupName)]
[assembly: Xamarin.Forms.ExportEffect(typeof(FScruiser.Droid.Effects.AndroidViewLifecycleEffect), ViewLifecycleEffect.EffectName)]

namespace FScruiser.Droid.Effects
{
    public class AndroidViewLifecycleEffect : PlatformEffect
    {
        private View _nativeView;
        private ViewLifecycleEffect _viewLifecycleEffect;

        protected override void OnAttached()
        {
            _viewLifecycleEffect = Element.Effects.OfType<ViewLifecycleEffect>().FirstOrDefault();

            _nativeView = Control ?? Container;
            _nativeView.ViewAttachedToWindow += OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow += OnViewDetachedFromWindow;
        }

        protected override void OnDetached()
        {
            _viewLifecycleEffect.RaiseUnloaded(Element);
            _nativeView.ViewAttachedToWindow -= OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow -= OnViewDetachedFromWindow;
        }

        private void OnViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e) => _viewLifecycleEffect?.RaiseLoaded(Element);

        private void OnViewDetachedFromWindow(object sender, View.ViewDetachedFromWindowEventArgs e) => _viewLifecycleEffect?.RaiseUnloaded(Element);
    }
}