using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace FScruiser.XF.Controls
{
    public class LastChildWidthLayout : Layout<View>, IElementConfiguration<LastChildWidthLayout>
    {
        public static readonly BindableProperty LastChildWidthProperty = BindableProperty.Create(
            nameof(LastChildWidth),
            typeof(double),
            typeof(LastChildWidthLayout),
            defaultValue: 0.0,
            defaultBindingMode: BindingMode.OneWay, propertyChanged: LastChildWidth_Changed);

        private static void LastChildWidth_Changed(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = (LastChildWidthLayout)bindable;
            layout.LastChildWidth = (double)newValue;
        }

        public double LastChildWidth
        {
            get => (double)GetValue(LastChildWidthProperty);
            set => SetValue(LastChildWidthProperty, value);
        }

        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(LastChildWidthLayout), 6d,
            propertyChanged: (bindable, oldvalue, newvalue) => ((LastChildWidthLayout)bindable).InvalidateLayout());

        private LayoutInformation _layoutInformation = new LayoutInformation();
        private readonly Lazy<PlatformConfigurationRegistry<LastChildWidthLayout>> _platformConfigurationRegistry;

        public LastChildWidthLayout()
        {
            _platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<LastChildWidthLayout>>(() =>
                new PlatformConfigurationRegistry<LastChildWidthLayout>(this));
        }

        public IPlatformElementConfiguration<T, LastChildWidthLayout> On<T>() where T : IConfigPlatform
        {
            return _platformConfigurationRegistry.Value.On<T>();
        }

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (!HasVisibleChildren())
            {
                return;
            }

            LayoutInformation layoutInformationCopy = _layoutInformation;
            if (width == layoutInformationCopy.Constraint.Width && height == layoutInformationCopy.Constraint.Height)
            {
                AlignOffAxis(layoutInformationCopy, width, height);
                //ProcessExpanders(layoutInformationCopy, x, y, width, height);
            }
            else
            {
                CalculateLayout(layoutInformationCopy, x, y, width, height);
            }

            if (layoutInformationCopy.Plots == null) { return; }
            for (var i = 0; i < Children.Count; i++)
            {
                var child = (View)Children[i];
                if (child.IsVisible)
                    LayoutChildIntoBoundingRegion(child, layoutInformationCopy.Plots[i], layoutInformationCopy.Requests[i]);
            }
        }

        // copy of a xamarin internal method that provides a efficiency over the standard LayoutChildIntoBoundingRegion method
        // that allows you to use precalculated chisSizeRequest
        private static void LayoutChildIntoBoundingRegion(View child, Rectangle region, SizeRequest childSizeRequest)
        {
            if (region.Size != childSizeRequest.Request)
            {
                bool canUseAlreadyDoneRequest = region.Width >= childSizeRequest.Request.Width && region.Height >= childSizeRequest.Request.Height;

                LayoutOptions horizontalOptions = child.HorizontalOptions;
                if (horizontalOptions.Alignment != LayoutAlignment.Fill)
                {
                    SizeRequest request = canUseAlreadyDoneRequest ? childSizeRequest : child.Measure(region.Width, region.Height, MeasureFlags.IncludeMargins);
                    double diff = Math.Max(0, region.Width - request.Request.Width);
                    double horizontalAlign = ToDouble(horizontalOptions.Alignment);

                    region.X += (int)(diff * horizontalAlign);
                    region.Width -= diff;
                }

                LayoutOptions verticalOptions = child.VerticalOptions;
                if (verticalOptions.Alignment != LayoutAlignment.Fill)
                {
                    SizeRequest request = canUseAlreadyDoneRequest ? childSizeRequest : child.Measure(region.Width, region.Height, MeasureFlags.IncludeMargins);
                    double diff = Math.Max(0, region.Height - request.Request.Height);
                    region.Y += (int)(diff * ToDouble(verticalOptions.Alignment));
                    region.Height -= diff;
                }
            }

            Thickness margin = child.Margin;
            region.X += margin.Left;
            region.Width -= margin.HorizontalThickness;
            region.Y += margin.Top;
            region.Height -= margin.VerticalThickness;

            child.Layout(region);

            double ToDouble(LayoutAlignment align)
            {
                switch (align)
                {
                    case LayoutAlignment.Start:
                        return 0;

                    case LayoutAlignment.Center:
                        return 0.5;

                    case LayoutAlignment.End:
                        return 1;
                }
                throw new ArgumentOutOfRangeException("align");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (!HasVisibleChildren())
            {
                return new SizeRequest();
            }

            // calculate with padding inset for X,Y so we can hopefully re-use this in the layout pass
            Thickness padding = Padding;
            CalculateLayout(_layoutInformation, padding.Left, padding.Top, widthConstraint, heightConstraint);
            var result = new SizeRequest(_layoutInformation.Bounds.Size, _layoutInformation.MinimumSize);
            return result;
        }

        protected override void InvalidateMeasure()
        {
            _layoutInformation = new LayoutInformation();
            base.InvalidateMeasure();
        }

        private void AlignOffAxis(LayoutInformation layout, double widthConstraint, double heightConstraint)
        {
            for (var i = 0; i < layout.Plots?.Length; i++)
            {
                if (!((View)Children[i]).IsVisible)
                    continue;
                layout.Plots[i].Height = heightConstraint;
            }
        }

        private void CalculateLayout(LayoutInformation layout, double x, double y, double widthConstraint, double heightConstraint)
        {
            layout.Constraint = new Size(widthConstraint, heightConstraint);
            layout.Plots = new Rectangle[Children.Count];
            layout.Requests = new SizeRequest[Children.Count];

            CalculateNaiveLayout(layout, x, y, widthConstraint, heightConstraint);
        }

        private void CalculateNaiveLayout(LayoutInformation layout, double x, double y, double widthConstraint, double heightConstraint)
        {
            double requestHeight = 0;
            double minimumHeight = 0;
            double spacing = Spacing;

            int lastChildIndex = Children.Count - 1;
            double lastChildWidth = Math.Max(0, this.LastChildWidth);
            double lastChildMinimumWidth = 0.0;

            var childCount = Children.Count;
            var children = Children;
            double inclusiveWidthRequest = 0.0; // width of all elements
            double secondaryWidthRequest = 0.0; // width of all but last element
            for (var i = 0; i < childCount; i++)
            {
                var child = (View)children[i];
                if (!child.IsVisible)
                    continue;

                if (i == lastChildIndex)
                {
                    SizeRequest request = child.Measure(lastChildWidth, heightConstraint, MeasureFlags.IncludeMargins);
                    layout.Requests[i] = request;

                    requestHeight = Math.Max(requestHeight, request.Request.Height);
                    lastChildMinimumWidth = lastChildWidth;
                    inclusiveWidthRequest += lastChildWidth;
                    minimumHeight = Math.Max(minimumHeight, request.Minimum.Height);
                }
                else
                {
                    SizeRequest request = child.Measure(double.PositiveInfinity, heightConstraint, MeasureFlags.IncludeMargins);
                    layout.Requests[i] = request;

                    requestHeight = Math.Max(requestHeight, request.Request.Height);
                    inclusiveWidthRequest += request.Request.Width + spacing;
                    secondaryWidthRequest += request.Request.Width + spacing;
                    minimumHeight = Math.Max(minimumHeight, request.Minimum.Height);
                }
            }

            double xOffset = x;
            for (int i = 0; i < childCount; i++)
            {
                var request = layout.Requests[i];

                if (i == lastChildIndex)
                {
                    var lastChildxOffset = (widthConstraint == Double.PositiveInfinity) ? xOffset
                        : widthConstraint - lastChildWidth;

                    var bounds = new Rectangle(lastChildxOffset, y, lastChildWidth, request.Request.Height);
                    layout.Plots[i] = bounds;

                    xOffset = bounds.Right;
                }
                else
                {
                    var bounds = new Rectangle(xOffset, y, request.Request.Width, request.Request.Height);
                    layout.Plots[i] = bounds;

                    xOffset = bounds.Right + spacing;
                }
            }

            layout.Bounds = new Rectangle(x, y, xOffset - x, requestHeight);
            layout.MinimumSize = new Size(lastChildMinimumWidth, minimumHeight);
        }

        private bool HasVisibleChildren()
        {
            for (var index = 0; index < Children.Count; index++)
            {
                var child = (VisualElement)Children[index];
                if (child.IsVisible)
                    return true;
            }
            return false;
        }

        private class LayoutInformation
        {
            public Rectangle Bounds;
            public Size Constraint;
            public Size MinimumSize;
            public Rectangle[] Plots;
            public SizeRequest[] Requests;
            public SizeRequest LastChildRequest;
        }
    }
}