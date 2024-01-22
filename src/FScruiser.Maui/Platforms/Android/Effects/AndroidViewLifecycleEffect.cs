using FScruiser.Maui.Effects;
using Microsoft.Maui.Controls.Platform;

using View = Android.Views.View;

namespace FScruiser.Platforms.Effects
{
    public class AndroidViewLifecycleEffect : PlatformEffect
    {
        private View? _nativeView;
        private ViewLifecycleEffect? _viewLifecycleEffect;

        protected override void OnAttached()
        {
            _viewLifecycleEffect = Element.Effects.OfType<ViewLifecycleEffect>().FirstOrDefault();

            _nativeView = Control ?? Container;
            _nativeView.ViewAttachedToWindow += OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow += OnViewDetachedFromWindow;
        }

        protected override void OnDetached()
        {
            _viewLifecycleEffect?.RaiseUnloaded(Element);

            if (_nativeView == null) return;
            _nativeView.ViewAttachedToWindow -= OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow -= OnViewDetachedFromWindow;
        }

        private void OnViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e) => _viewLifecycleEffect?.RaiseLoaded(Element);

        private void OnViewDetachedFromWindow(object sender, View.ViewDetachedFromWindowEventArgs e) => _viewLifecycleEffect?.RaiseUnloaded(Element);
    }
}