using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NatCruise.Wpf.Controls
{
    //public class SharedLastChildWidthHelper : DependencyObject, INotifyPropertyChanged
    //{
    //    public static readonly DependencyProperty OwningSharedWidthProperty = DependencyProperty.RegisterAttached(
    //        nameof(OwningSharedWidthProperty),
    //        typeof(SharedLastChildWidthHelper),
    //        typeof(SharedLastChildWidthHelper),
    //        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    //    public static void SetOwningSharedPropertyValueColumnWidthContainer(DependencyObject obj, SharedLastChildWidthHelper value)
    //    {
    //        if (obj == null) { throw new ArgumentNullException(nameof(obj)); }

    //        obj.SetValue(OwningSharedWidthProperty, value);
    //    }

    //    public static SharedLastChildWidthHelper GetOwningSharedPropertyValueColumnWidthContainer(DependencyObject obj)
    //    {
    //        if (obj == null) { throw new ArgumentNullException(nameof(obj)); }

    //        return (SharedLastChildWidthHelper)obj.GetValue(OwningSharedWidthProperty);
    //    }

    //    private const double DefaultPercentage = 0.4f;

    //    private bool _changeTriggeredInternally;
    //    private double _sharedLastChildWidth = 0.0;
    //    private double _containerWidth;
    //    private double _valueWidthPercentage = DefaultPercentage;

    //    [EditorBrowsable(EditorBrowsableState.Never)]
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public double SharedLastChildWidth
    //    {
    //        get
    //        {
    //            return _sharedLastChildWidth;
    //        }
    //        set
    //        {
    //            if (value.GridUnitType != GridUnitType.Pixel)
    //            {
    //                throw new InvalidOperationException();
    //            }

    //            _sharedLastChildWidth = value;
    //            OnPropertyChanged(nameof(SharedLastChildWidth));

    //            if (this.ContainerWidth > 0)
    //            {
    //                if (!_changeTriggeredInternally)
    //                {
    //                    try
    //                    {
    //                        // Don't modify ValueWidth again
    //                        _changeTriggeredInternally = true;

    //                        this.ValueWidthPercentage = value.Value / this.ContainerWidth;
    //                    }
    //                    finally
    //                    {
    //                        _changeTriggeredInternally = false;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public double ContainerWidth
    //    {
    //        get
    //        {
    //            return _containerWidth;
    //        }
    //        internal set
    //        {
    //            _containerWidth = value;
    //            OnPropertyChanged(nameof(ContainerWidth));

    //            if (!_changeTriggeredInternally)
    //            {
    //                try
    //                {
    //                    // Don't modify ValueWidthPercentage, just ValueWidth
    //                    _changeTriggeredInternally = true;

    //                    this.SharedLastChildWidth = new GridLength(value * this.ValueWidthPercentage);
    //                }
    //                finally
    //                {
    //                    _changeTriggeredInternally = false;
    //                }
    //            }
    //        }
    //    }

    //    public double ValueWidthPercentage
    //    {
    //        get
    //        {
    //            return _valueWidthPercentage;
    //        }
    //        set
    //        {
    //            _valueWidthPercentage = Normalize(value, 0, 1);
    //            OnPropertyChanged(nameof(ValueWidthPercentage));

    //            if (!_changeTriggeredInternally)
    //            {
    //                try
    //                {
    //                    // Don't modify ValueWidthPercentage again
    //                    _changeTriggeredInternally = true;

    //                    this.SharedLastChildWidth = new GridLength(value * this.ContainerWidth);
    //                }
    //                finally
    //                {
    //                    _changeTriggeredInternally = false;
    //                }
    //            }
    //        }
    //    }

    //    private static double Normalize(double value, double min, double max)
    //    {
    //        return Math.Max(min, Math.Min(max, value));
    //    }

    //    private void OnPropertyChanged(string propertyName)
    //    {
    //        if (propertyName == null)
    //        {
    //            throw new ArgumentNullException(nameof(propertyName));
    //        }

    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }

    //}
}
