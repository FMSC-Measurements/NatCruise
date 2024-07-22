using System.ComponentModel;

namespace FScruiser.Maui.Controls
{
    public class HeaderControlBase : ContentView
    {
        protected const string PartHeaderPresenter = "PART_Header";

        #region Header Property

        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
            nameof(Header),
            typeof(object),
            typeof(HeaderControlBase),
            propertyChanged: (sender, oldVal, newVal) => ((HeaderControlBase)sender).OnHeaderChanged(oldVal, newVal)
            );

        protected void OnHeaderChanged(object oldVal, object newVal)
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
                typeof(HeaderControlBase),
                propertyChanged: (sender, oldVal, newVal) => ((HeaderControlBase)sender).OnHeaderTemplateChanged((DataTemplate)oldVal, (DataTemplate)newVal));

        protected void OnHeaderTemplateChanged(DataTemplate oldVal, DataTemplate newVal)
        {
            //throw new NotImplementedException();
        }

        public DataTemplate? HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        #endregion HeaderTemplate Property

        #region HeaderForground Property

        public static readonly BindableProperty HeaderForegroundProperty =
            BindableProperty.Create(
                nameof(HeaderForeground),
                typeof(Color),
                typeof(HeaderControlBase),
                defaultValue: InputView.TextColorProperty.DefaultValue);

        public Color HeaderForeground
        {
            get => (Color)GetValue(HeaderForegroundProperty);
            set => SetValue(HeaderForegroundProperty, value);
        }

        #endregion HeaderForground Property

        #region HeaderBackground Property

        public static readonly BindableProperty HeaderBackgroundColorProperty =
            BindableProperty.Create(
                nameof(HeaderBackgroundColor),
                typeof(Color),
                typeof(HeaderControlBase),
                defaultValue: VisualElement.BackgroundColorProperty.DefaultValue);

        public Color HeaderBackgroundColor
        {
            get => (Color)GetValue(HeaderBackgroundColorProperty);
            set => SetValue(HeaderBackgroundColorProperty, value);
        }

        #endregion HeaderBackground Property

        #region HeaderFontSize Property

        public static readonly BindableProperty HeaderFontSizeProperty =
            BindableProperty.Create(
                nameof(HeaderFontSize),
                typeof(double),
                typeof(HeaderControlBase),
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
                typeof(HeaderControlBase),
                typeof(string),
                defaultValue: default(string));

        public string? HeaderFontFamily
        {
            get => (string)GetValue(HeaderFontFamilyProperty);
            set => SetValue(HeaderFontFamilyProperty, value);
        }

        #endregion HeaderFontFamily Property

        #region HeaderFontAttributes Property

        public static readonly BindableProperty HeaderFontAttributesProperty =
            BindableProperty.Create(
                nameof(HeaderFontAttributes),
                typeof(FontAttributes),
                typeof(HeaderControlBase),
                defaultValue: FontAttributes.None);

        public FontAttributes HeaderFontAttributes 
        {
            get => (FontAttributes)GetValue(HeaderFontAttributesProperty);
            set => SetValue(HeaderFontAttributesProperty, value);
        }

        #endregion HeaderFontAttributes Property

        #region HeaderMargin

        public static readonly BindableProperty HeaderMarginProperty =
                BindableProperty.Create(
                    nameof(HeaderMargin),
                    typeof(Thickness),
                    typeof(HeaderControlBase),
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
                typeof(HeaderControlBase),
                defaultValue: default(LayoutOptions));

        public LayoutOptions HeaderHorizontalOptions
        {
            get => (LayoutOptions)GetValue(HeaderHorizontalOptionsProperty);
            set => SetValue(HeaderHorizontalOptionsProperty, value);
        }

        #endregion HeaderHorizontalOptions Property

        #region HeaderVerticalOptions Property

        public static readonly BindableProperty HeaderVerticalOptionsProperty =
            BindableProperty.Create(
                nameof(HeaderVerticalOptions),
                typeof(LayoutOptions),
                typeof(HeaderControlBase),
                defaultValue: default(LayoutOptions));

        public LayoutOptions HeaderVerticalOptions
        {
            get => (LayoutOptions)GetValue(HeaderVerticalOptionsProperty);
            set => SetValue(HeaderVerticalOptionsProperty, value);
        }

        #endregion HeaderVerticalOptions Property

        //public HeaderControlBase()
        //{
        //}

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetHeaderVisibility();
        }

        protected void SetHeaderVisibility()
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
}