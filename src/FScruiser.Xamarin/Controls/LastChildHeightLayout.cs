using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FScruiser.XF.Controls
{
    public class LastChildHeightLayout : Layout<View>, IElementConfiguration<LastChildHeightLayout>
    {
        public static readonly BindableProperty LastChildHeightProperty = BindableProperty.Create(
            nameof(LastChildHeight),
            typeof(double),
            typeof(LastChildHeightLayout),
            defaultValue: 0.0,
            defaultBindingMode: BindingMode.OneWay, propertyChanged: LastChildHeight_Changed);

        private static void LastChildHeight_Changed(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = (LastChildHeightLayout)bindable;
            layout.LastChildHeight = (double)newValue;
        }

        public double LastChildHeight
        {
            get => (double)GetValue(LastChildHeightProperty);
            set => SetValue(LastChildHeightProperty, value);
        }


        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(LastChildHeightLayout), 6d,
            propertyChanged: (bindable, oldvalue, newvalue) => ((LastChildHeightLayout)bindable).InvalidateLayout());

        LayoutInformation _layoutInformation = new LayoutInformation();
        readonly Lazy<PlatformConfigurationRegistry<LastChildHeightLayout>> _platformConfigurationRegistry;

        public LastChildHeightLayout()
        {
            _platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<LastChildHeightLayout>>(() =>
                new PlatformConfigurationRegistry<LastChildHeightLayout>(this));
        }

        public IPlatformElementConfiguration<T, LastChildHeightLayout> On<T>() where T : IConfigPlatform
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
                CalculateLayout(layoutInformationCopy, x, y, width, height, true);
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
        static void LayoutChildIntoBoundingRegion(View child, Rectangle region, SizeRequest childSizeRequest)
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


        //[Obsolete("OnSizeRequest is obsolete as of version 2.2.0. Please use OnMeasure instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (!HasVisibleChildren())
            {
                return new SizeRequest();
            }

            // calculate with padding inset for X,Y so we can hopefully re-use this in the layout pass
            Thickness padding = Padding;
            CalculateLayout(_layoutInformation, padding.Left, padding.Top, widthConstraint, heightConstraint, false);
            var result = new SizeRequest(_layoutInformation.Bounds.Size, _layoutInformation.MinimumSize);
            return result;
        }

        //internal override void ComputeConstraintForView(View view)
        //{
        //    ComputeConstraintForView(view, false);
        //}

        //void ComputeConstraintForView(View view, bool isOnlyExpander)
        //{
        //    if ((Constraint & LayoutConstraint.HorizontallyFixed) != 0 && view.HorizontalOptions.Alignment == LayoutAlignment.Fill)
        //    {
        //        if (isOnlyExpander && view.VerticalOptions.Alignment == LayoutAlignment.Fill && Constraint == LayoutConstraint.Fixed)
        //        {
        //            view.ComputedConstraint = LayoutConstraint.Fixed;
        //        }
        //        else
        //        {
        //            view.ComputedConstraint = LayoutConstraint.HorizontallyFixed;
        //        }
        //    }
        //    else
        //    {
        //        view.ComputedConstraint = LayoutConstraint.None;
        //    }
        //}

        protected override void InvalidateMeasure()
        {
            _layoutInformation = new LayoutInformation();
            base.InvalidateMeasure();
        }

        //internal override void InvalidateMeasureInternal(InvalidationTrigger trigger)
        //{
        //    _layoutInformation = new LayoutInformation();
        //    base.InvalidateMeasureInternal(trigger);
        //}

        void AlignOffAxis(LayoutInformation layout, double widthConstraint, double heightConstraint)
        {
            for (var i = 0; i < layout.Plots?.Length; i++)
            {
                if (!((View)Children[i]).IsVisible)
                    continue;
                layout.Plots[i].Width = widthConstraint;
            }
        }

        void CalculateLayout(LayoutInformation layout, double x, double y, double widthConstraint, double heightConstraint, bool processExpanders)
        {
            layout.Constraint = new Size(widthConstraint, heightConstraint);
            layout.Expanders = 0;
            layout.CompressionSpace = 0;
            layout.Plots = new Rectangle[Children.Count];
            layout.Requests = new SizeRequest[Children.Count];

            CalculateNaiveLayout(layout, x, y, widthConstraint, heightConstraint);
            CompressNaiveLayout(layout, widthConstraint, heightConstraint);

            if (processExpanders)
            {
                AlignOffAxis(layout, widthConstraint, heightConstraint);
                //ProcessExpanders(layout, x, y, widthConstraint, heightConstraint);
            }
        }

        void CalculateNaiveLayout(LayoutInformation layout, double x, double y, double widthConstraint, double heightConstraint)
        {
            layout.CompressionSpace = 0;

            double xOffset = x;
            
            double requestWidth = 0;
            double minimumWidth = 0;
            double spacing = Spacing;

            int lastChildIndex = Children.Count - 1;
            double lastChildHeight = Math.Max(0, this.LastChildHeight);
            double lastChildMinimumHeight = 0.0;

            var childCount = Children.Count;
            var children = Children;
            double inclusiveHeightRequest = 0.0;
            double secondaryHeightRequest = 0.0;
            for (var i = 0; i < childCount; i++)
            {
                var child = (View)children[i];
                if (!child.IsVisible)
                    continue;

                if(i == lastChildIndex)
                {
                    SizeRequest request = child.Measure(widthConstraint, lastChildHeight, MeasureFlags.IncludeMargins);
                    layout.Requests[i] = request;

                    requestWidth = Math.Max(requestWidth, request.Request.Width);
                    lastChildMinimumHeight = lastChildHeight;
                    inclusiveHeightRequest += lastChildHeight;
                    minimumWidth = Math.Max(minimumWidth, request.Minimum.Width);
                }
                else
                {
                    //if (child.VerticalOptions.Expands)
                    //{
                    //    layout.Expanders++;
                    //    if (expander != null)
                    //    {
                    //        // we have multiple expanders, make sure previous expanders are reset to not be fixed because they no logner are
                    //        //ComputeConstraintForView(child, false);
                    //    }
                    //    expander = child;
                    //}

                    SizeRequest request = child.Measure(widthConstraint, double.PositiveInfinity, MeasureFlags.IncludeMargins);
                    layout.Requests[i] = request;
                    layout.CompressionSpace += Math.Max(0, request.Request.Height - request.Minimum.Height);

                    requestWidth = Math.Max(requestWidth, request.Request.Width);
                    inclusiveHeightRequest += request.Request.Height + spacing;
                    secondaryHeightRequest += request.Request.Height + spacing;
                    minimumWidth = Math.Max(minimumWidth, request.Minimum.Width);
                }

            }
            //if (expander != null)
            //ComputeConstraintForView(expander, layout.Expanders == 1); // warning : slightly obtuse, but we either need to setup the expander or clear the last one

            double yOffset = y;
            for (int i = 0; i < childCount; i++)
            {
                var request = layout.Requests[i];

                if(i == lastChildIndex)
                {
                    var lastChildyOffset = (heightConstraint == Double.PositiveInfinity) ? yOffset
                        : heightConstraint - lastChildHeight;

                    var bounds = new Rectangle(x, lastChildyOffset, request.Request.Width, lastChildHeight);
                    layout.Plots[i] = bounds;

                    yOffset = bounds.Bottom;
                }
                else
                {
                    var bounds = new Rectangle(x, yOffset, request.Request.Width, request.Request.Height);
                    layout.Plots[i] = bounds;

                    yOffset = bounds.Bottom + spacing;
                }
            }

            layout.Bounds = new Rectangle(x, y, requestWidth, yOffset - y);
            layout.MinimumSize = new Size(minimumWidth, lastChildMinimumHeight);
        }

        void CompressNaiveLayout(LayoutInformation layout, double widthConstraint, double heightConstraint)
        {
            if (layout.CompressionSpace <= 0)
                return;

            CompressVerticalLayout(layout, widthConstraint, heightConstraint);
        }

        void CompressVerticalLayout(LayoutInformation layout, double widthConstraint, double heightConstraint)
        {
            double yOffset = 0;

            if (heightConstraint >= layout.Bounds.Height)
            {
                // no need to compress
                return;
            }

            double requiredCompression = layout.Bounds.Height - heightConstraint;
            double compressionSpace = layout.CompressionSpace;
            double compressionPressure = (requiredCompression / layout.CompressionSpace).Clamp(0, 1);

            var numChildren = layout.Plots.Length;
            var lastChild = numChildren - 1;
            var children = Children;
            for (var i = 0; i < numChildren; i++)
            {
                if (i == lastChild) continue; // don't compress last child

                var child = (View)children[i];
                if (!child.IsVisible)
                    continue;

                Size minimum = layout.Requests[i].Minimum;

                layout.Plots[i].Y -= yOffset;

                Rectangle plot = layout.Plots[i];
                double availableSpace = plot.Height - minimum.Height;
                if (availableSpace <= 0)
                    continue;

                compressionSpace -= availableSpace;

                double compression = availableSpace * compressionPressure;
                yOffset += compression;

                double newHeight = plot.Height - compression;
                SizeRequest newRequest = child.Measure(widthConstraint, newHeight, MeasureFlags.IncludeMargins);

                layout.Requests[i] = newRequest;

                plot.Width = newRequest.Request.Width;

                if (newRequest.Request.Height < newHeight)
                {
                    double delta = newHeight - newRequest.Request.Height;
                    newHeight = newRequest.Request.Height;
                    yOffset += delta;
                    requiredCompression = requiredCompression - yOffset;
                    compressionPressure = (requiredCompression / compressionSpace).Clamp(0, 1);
                }
                plot.Height = newHeight;

                layout.Bounds.Width = Math.Max(layout.Bounds.Width, plot.Width);

                layout.Plots[i] = plot;
            }
        }



        bool HasVisibleChildren()
        {
            for (var index = 0; index < Children.Count; index++)
            {
                var child = (VisualElement)Children[index];
                if (child.IsVisible)
                    return true;
            }
            return false;
        }

        //void ProcessExpanders(LayoutInformation layout, double x, double y, double widthConstraint, double heightConstraint)
        //{
        //    if (layout.Expanders <= 0)
        //        return;

        //    double extraSpace = heightConstraint - layout.Bounds.Height;
        //    if (extraSpace <= 0)
        //        return;

        //    double spacePerExpander = extraSpace / layout.Expanders;
        //    double yOffset = 0;

        //    for (var i = 0; i < Children.Count; i++)
        //    {
        //        var child = (View)Children[i];
        //        if (!child.IsVisible)
        //            continue;
        //        Rectangle plot = layout.Plots[i];
        //        plot.Y += yOffset;

        //        if (child.VerticalOptions.Expands)
        //        {
        //            plot.Height += spacePerExpander;
        //            yOffset += spacePerExpander;
        //        }

        //        layout.Plots[i] = plot;
        //    }

        //    layout.Bounds.Height = heightConstraint;
        //}

        class LayoutInformation
        {
            public Rectangle Bounds;
            public double CompressionSpace;
            public Size Constraint;
            public int Expanders;
            public Size MinimumSize;
            public Rectangle[] Plots;
            public SizeRequest[] Requests;
            public SizeRequest LastChildRequest;
        }
    }

    //public class LastChildHeightLayout : Layout<View>
    //{

    //    public static readonly BindableProperty LastChildHeightProperty = BindableProperty.Create(
    //        nameof(LastChildHeight),
    //        typeof(double),
    //        typeof(LastChildHeightLayout),
    //        defaultValue:0.0,
    //        defaultBindingMode: BindingMode.OneWay, propertyChanged: LastChildHeight_Changed);

    //    private static void LastChildHeight_Changed(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        var layout = (LastChildHeightLayout)bindable;
    //        layout.LastChildHeight = (double)newValue;
    //    }

    //    public double LastChildHeight
    //    {
    //        get => (double)GetValue(LastChildHeightProperty);
    //        set => SetValue(LastChildHeightProperty, value);
    //    }

    //    SizeRequest[] _childSizes;

    //    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
    //    {

    //        double lastChildHeight = Math.Max(0, this.LastChildHeight);

    //        double desiredHeight = 0.0;
    //        double desiredWidth = 0.0;

    //        var children = Children;
    //        var childrenCount = children.Count;
    //        var childSizes = new SizeRequest[childrenCount];
    //        var lastIndex = childrenCount - 1;

    //        for (int i = 0; i < childrenCount; i++)
    //        {
    //            var child = children[i] as VisualElement;
    //            if (child == null || child.IsVisible == false) { continue; }

    //            var childSize = childSizes[i] = (i == lastIndex) ?
    //                child.Measure(widthConstraint, lastChildHeight)
    //                : child.Measure(widthConstraint, heightConstraint);

    //            desiredHeight += childSize.Request.Height;

    //            desiredWidth = Math.Max(desiredWidth, childSize.Request.Width);
    //        }
    //        _childSizes = childSizes;

    //        return new SizeRequest(new Size(desiredWidth, desiredHeight));
    //    }

    //    protected override void LayoutChildren(double x, double y, double width, double height)
    //    {
    //        double lastChildHeight = Math.Max(0, this.LastChildHeight);
    //        double availableHeight = Math.Max(0, height - lastChildHeight);
    //        double top = availableHeight;
    //        var children = Children;
    //        var childrenCount = children.Count;
    //        var lastIndex = childrenCount -1;

    //        for(int i = childrenCount -1; i >= 0; i--)
    //        {
    //            var child = children[i];
    //            if(child == null || child.IsVisible == false) { continue; }
    //            var childSizeRequest = _childSizes[i];
    //            var desiredHeight = childSizeRequest.Request.Height;

    //            if(i == lastIndex)
    //            {
    //                child.Layout(new Rectangle(0, Math.Max(0, height - lastChildHeight), width, lastChildHeight));
    //            }
    //            else
    //            {
    //                child.Layout(new Rectangle(0, Math.Min(top, 0), width, desiredHeight));
    //            }

    //            top -= desiredHeight;
    //            availableHeight -= desiredHeight;
    //            availableHeight = Math.Max(0, availableHeight);
    //        }
    //    }
    //}
}
