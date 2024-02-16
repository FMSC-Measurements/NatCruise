using Microsoft.Maui.Controls.Compatibility;

namespace FScruiser.Maui.Controls;

public class UniformGrid : Layout<View>
{
    private double _childWidth;
    private double _childHeight;


    public static readonly BindableProperty MaxRowsProperty = BindableProperty.Create(
        nameof(MaxRows),
        typeof(int),
        typeof(UniformGrid),
        int.MaxValue);


    public static readonly BindableProperty MaxColumnsProperty = BindableProperty.Create(
        nameof(MaxColumns),
        typeof(int),
        typeof(UniformGrid),
        int.MaxValue);

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
        var boundsHeight = _childHeight;
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

        if (_childWidth == 0 && visibleChildren.Any())
        {
            foreach (var child in visibleChildren)
            {
                var sizeRequest = child.Measure(double.PositiveInfinity, double.PositiveInfinity);

                _childWidth = Math.Max(_childWidth, sizeRequest.Request.Width);
                _childHeight = Math.Max(_childHeight, sizeRequest.Request.Height);
            }
        }

        var columns = GetColumnsCount(visibleChildren.Length, widthConstraint - Padding.HorizontalThickness);
        var rows = GetRowsCount(visibleChildren.Length, columns);

        return new SizeRequest(new Size(columns * _childWidth + Padding.HorizontalThickness, rows * _childHeight + Padding.VerticalThickness));
    }

    private int GetColumnsCount(int visibleChildrenCount, double widthConstraint)
    {
        var columnsCount = visibleChildrenCount;
        if (_childWidth != 0 && !double.IsPositiveInfinity(widthConstraint))
        {
            columnsCount = Math.Clamp((int)(widthConstraint / _childWidth), 1, visibleChildrenCount);
        }

        return Math.Min(columnsCount, MaxColumns);
    }

    private int GetRowsCount(int visibleChildrenCount, int columnsCount)
        => Math.Min(
            (int)Math.Ceiling((double)visibleChildrenCount / columnsCount),
            MaxRows);
}