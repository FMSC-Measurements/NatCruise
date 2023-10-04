using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Controls
{
    public class UniformGrid : Layout<View>
    {
        double childWidth, childHeight;

        /// <summary>
        /// Backing BindableProperty for the <see cref="MaxRows"/> property.
        /// </summary>
        public static readonly BindableProperty MaxRowsProperty = BindableProperty.Create(nameof(MaxRows), typeof(int), typeof(UniformGrid), int.MaxValue);

        /// <summary>
        /// Backing BindableProperty for the <see cref="MaxColumns"/> property.
        /// </summary>
        public static readonly BindableProperty MaxColumnsProperty = BindableProperty.Create(nameof(MaxColumns), typeof(int), typeof(UniformGrid), int.MaxValue);

        /// <summary>
        /// Max rows
        /// </summary>
        public int MaxRows
        {
            get => (int)GetValue(MaxRowsProperty);
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxRows)} must be greater or equal to 1.");
                }

                SetValue(MaxRowsProperty, value);
            }
        }

        /// <summary>
        /// Max columns
        /// </summary>
        public int MaxColumns
        {
            get => (int)GetValue(MaxColumnsProperty);
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxColumns)} must be greater or equal to 1.");
                }

                SetValue(MaxColumnsProperty, value);
            }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            width = width - Padding.HorizontalThickness;
            var visibleChildren = Children.Where(x => x.IsVisible).ToArray();

            var columns = GetColumnsCount(visibleChildren.Length, width);
            var rows = GetRowsCount(visibleChildren.Length, columns);
            var boundsWidth = width / columns;
            var boundsHeight = childHeight;
            var bounds = new Rect(0, 0, boundsWidth, boundsHeight);
            var count = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns && count < visibleChildren.Length; j++)
                {
                    var item = visibleChildren[count];
                    bounds.X = j * boundsWidth + Padding.Left;
                    bounds.Y = i * boundsHeight + Padding.Top;
                    
                    //item.Measure(bounds.Width, bounds.Height);
                    item.Layout(bounds);
                    count++;
                }
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var visibleChildren = Children.Where(x => x.IsVisible).ToArray();

            if (childWidth == 0 && visibleChildren.Any())
            {
                foreach (var child in visibleChildren)
                {
                    var sizeRequest = child.Measure(double.PositiveInfinity, double.PositiveInfinity);

                    childWidth = Math.Max(childWidth, sizeRequest.Request.Width);
                    childHeight = Math.Max(childHeight, sizeRequest.Request.Height);
                }
            }

            var columns = GetColumnsCount(visibleChildren.Length, widthConstraint - Padding.HorizontalThickness);
            var rows = GetRowsCount(visibleChildren.Length, columns);

            return new SizeRequest(new Size(columns * childWidth + Padding.HorizontalThickness, rows * childHeight + Padding.VerticalThickness));
        }

        int GetColumnsCount(int visibleChildrenCount, double widthConstraint)
        {
            var columnsCount = visibleChildrenCount;
            if (childWidth != 0 && !double.IsPositiveInfinity(widthConstraint))
            {
                columnsCount = Math.Clamp((int)(widthConstraint / childWidth), 1, visibleChildrenCount);
            }

            return Math.Min(columnsCount, MaxColumns);
        }

        int GetRowsCount(int visibleChildrenCount, int columnsCount)
            => Math.Min(
                (int)Math.Ceiling((double)visibleChildrenCount / columnsCount),
                MaxRows);
    }
}
