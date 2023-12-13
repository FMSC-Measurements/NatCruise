using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.Maui.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HeaderControl : ContentView
{
    private const string PartHeaderPresenter = "PART_Header";

    #region Header Property

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header),
        typeof(object),
        typeof(HeaderControl),
        propertyChanged: (sender, oldVal, newVal) => ((HeaderControl)sender).OnHeaderChanged(oldVal, newVal)
        );

    private void OnHeaderChanged(object oldVal, object newVal)
    {
        SetHeaderVisibility();
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    #endregion Header Property

    #region HeaderTemplate Property

    public static readonly BindableProperty HeaderTemplateProperty =
        BindableProperty.Create(
            nameof(HeaderTemplate),
            typeof(DataTemplate),
            typeof(HeaderControl),
            propertyChanged: (sender, oldVal, newVal) => ((HeaderControl)sender).OnHeaderTemplateChanged((DataTemplate)oldVal, (DataTemplate)newVal));

    private void OnHeaderTemplateChanged(DataTemplate oldVal, DataTemplate newVal)
    {
        //throw new NotImplementedException();
    }

    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    #endregion HeaderTemplate Property

    #region Orientation Property

    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(
            nameof(Orientation),
            typeof(StackOrientation),
            typeof(HeaderControl),
            defaultValue: StackOrientation.Vertical);

    public StackOrientation Orientation
    {
        get => (StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    #endregion Orientation Property

    #region HeaderForground Property

    public static readonly BindableProperty HeaderForegroundProperty =
        BindableProperty.Create(
            nameof(HeaderForeground),
            typeof(Color),
            typeof(HeaderControl),
            defaultValue: InputView.TextColorProperty.DefaultValue);

    public Color HeaderForeground
    {
        get => (Color)GetValue(HeaderForegroundProperty);
        set => SetValue(HeaderForegroundProperty, value);
    }

    #endregion HeaderForground Property

    #region HeaderBackground Property

    public static readonly BindableProperty HeaderBackgroundProperty =
        BindableProperty.Create(
            nameof(HeaderBackground),
            typeof(Color),
            typeof(HeaderControl),
            defaultValue: VisualElement.BackgroundColorProperty.DefaultValue);

    public Color HeaderBackground
    {
        get => (Color)GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    #endregion HeaderBackground Property

    #region HeaderFontSize Property

    public static readonly BindableProperty HeaderFontSizeProperty =
        BindableProperty.Create(
            nameof(HeaderFontSize),
            typeof(double),
            typeof(HeaderControl),
            defaultValueCreator: (self) => Device.GetNamedSize(NamedSize.Header, (Element)self));

    [TypeConverter(typeof(FontSizeConverter))]
    public double HeaderFontSize
    {
        get { return (double)GetValue(HeaderFontSizeProperty); }
        set => SetValue(HeaderFontSizeProperty, value);
    }

    #endregion HeaderFontSize Property

    #region HeaderFontFamily Property

    public static readonly BindableProperty HeaderFontFamilyProperty =
        BindableProperty.Create(
            nameof(HeaderFontFamily),
            typeof(HeaderControl),
            typeof(string),
            defaultValue: default(string));

    public string? HeaderFontFamily
    {
        get => (string)GetValue(HeaderFontFamilyProperty);
        set => SetValue(HeaderFontFamilyProperty, value);
    }

    #endregion HeaderFontFamily Property

    #region HeaderMargin

    public static readonly BindableProperty HeaderMarginProperty =
            BindableProperty.Create(
                nameof(HeaderMargin),
                typeof(Thickness),
                typeof(HeaderControl),
                defaultValue: default(Thickness));

    public Thickness HeaderMargin
    {
        get => (Thickness)GetValue(HeaderMarginProperty);
        set => SetValue(HeaderMarginProperty, value);
    }

    #endregion HeaderMargin

    #region HeaderHorizontalOptions Property

    public static readonly BindableProperty HeaderHorizontalOptionsProperty =
        BindableProperty.Create(
            nameof(HeaderHorizontalOptions),
            typeof(LayoutOptions),
            typeof(HeaderControl),
            defaultValue: default(LayoutOptions));

    public LayoutOptions HeaderHorizontalOptions
    {
        get => (LayoutOptions)GetValue(HeaderHorizontalOptionsProperty);
        set => SetValue(HeaderHorizontalOptionsProperty, value);
    }

    #endregion HeaderHorizontalOptions Property

    #region HeaderVerrticalOptions Property

    public static readonly BindableProperty HeaderVerticalOptionsProperty =
        BindableProperty.Create(
            nameof(HeaderVerticalOptions),
            typeof(LayoutOptions),
            typeof(HeaderControl),
            defaultValue: default(LayoutOptions));

    public LayoutOptions HeaderVerticalOptions
    {
        get => (LayoutOptions)GetValue(HeaderVerticalOptionsProperty);
        set => SetValue(HeaderVerticalOptionsProperty, value);
    }

    #endregion HeaderVerrticalOptions Property

    public HeaderControl()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        SetHeaderVisibility();
    }

    private void SetHeaderVisibility()
    {
        if (this.GetTemplateChild(PartHeaderPresenter) is VisualElement headerPresenter)
        {
            if (this.Header is string headerText)
            {
                headerPresenter.IsVisible = !string.IsNullOrEmpty(headerText);
            }
            else
            {
                headerPresenter.IsVisible = this.Header != null;
            }
        }
    }
}