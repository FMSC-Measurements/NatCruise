using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Wpf.Controls
{
    /// <summary>
    /// A horizontal stack panel that allows setting a constant width for the last child
    /// This control is useful for creating a container for editing fields where the panel
    /// contains a label and a UI component for editing a value
    /// </summary>
    public class LastChildWidthPanel : Panel
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        public static readonly DependencyProperty LastChildWidthProperty = DependencyProperty.Register(
            "LastChildWidth",
            typeof(double),
            typeof(LastChildWidthPanel),
            new FrameworkPropertyMetadata(
            (double)0,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register(
            "Level",
            typeof(int),
            typeof(LastChildWidthPanel),
            new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));


        public static readonly DependencyProperty LevelIndentProperty = DependencyProperty.Register(
            "LevelIndent",
            typeof(double),
            typeof(LastChildWidthPanel),
            new FrameworkPropertyMetadata(
            (double)13,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
#pragma warning restore S1104 // Fields should not have public accessibility

        public double LastChildWidth
        {
            get { return (double)this.GetValue(LastChildWidthProperty); }
            set { this.SetValue(LastChildWidthProperty, value); }
        }

        public int Level
        {
            get { return (int)this.GetValue(LevelProperty); }
            set { this.SetValue(LevelProperty, value); }
        }

        public double LevelIndent
        {
            get { return (double)this.GetValue(LevelIndentProperty); }
            set { this.SetValue(LevelIndentProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {

            double lastChildWidth = Math.Max(0, this.LastChildWidth);
            double indent = this.LevelIndent * this.Level;
            double availableWidth = Math.Max(0, availableSize.Width - lastChildWidth - indent);
            int childrenCount = InternalChildren.Count;
            int lastIndex = childrenCount - 1;
            Size actualSize = new Size();

            for (int i = 0; i < childrenCount; i++)
            {
                UIElement child = InternalChildren[i];

                if (i == lastIndex)
                {
                    InternalChildren[i].Measure(new Size(lastChildWidth, availableSize.Height));
                }
                else
                {
                    InternalChildren[i].Measure(new Size(availableWidth, availableSize.Height));
                }

                availableWidth -= child.DesiredSize.Width;
                //Compute the actual size for the propertypanel
                actualSize.Height = Math.Max(actualSize.Height, child.DesiredSize.Height);
                actualSize.Width += child.DesiredSize.Width;
            }

            return actualSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {

            double lastChildWidth = Math.Max(0, this.LastChildWidth);
            double indent = this.LevelIndent * this.Level;
            double availableWidth = Math.Max(0, finalSize.Width - lastChildWidth - indent);
            double left = indent;
            int childrenCount = InternalChildren.Count;
            int lastIndex = childrenCount - 1;

            for (int i = 0; i < childrenCount; i++)
            {

                UIElement child = InternalChildren[i];
                double desiredWidth = child.DesiredSize.Width;
                if (i == lastIndex)
                {
                    child.Arrange(new Rect(Math.Max(0, finalSize.Width - lastChildWidth), 0, lastChildWidth, finalSize.Height));
                }
                else
                {
                    child.Arrange(new Rect(left, 0, Math.Min(desiredWidth, availableWidth), finalSize.Height));
                }

                left += desiredWidth;
                availableWidth -= desiredWidth;
                availableWidth = Math.Max(0, availableWidth);
            }

            return finalSize;
        }
    }
}
