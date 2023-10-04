using NatCruise.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopulationIndicator : ContentView
    {
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            nameof(FontSize),
            typeof(double),
            typeof(PopulationIndicator),
            -1.0);

        public static readonly BindableProperty AltFontSizeProperty = BindableProperty.Create(
            nameof(AltFontSize),
            typeof(double),
            typeof(PopulationIndicator),
            -1.0);

        public static readonly BindableProperty AltFontAttributesProperty = BindableProperty.Create(
            nameof(AltFontAttributes),
            typeof(FontAttributes),
            typeof(PopulationIndicator),
            FontAttributes.None);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(PopulationIndicator),
            Color.White);

        public static readonly BindableProperty CellPaddingProperty = BindableProperty.Create(
            nameof(CellPadding),
            typeof(Thickness),
            typeof(PopulationIndicator),
            default(Thickness));

        public static readonly BindableProperty CellMarginProperty = BindableProperty.Create(
            nameof(CellMargin),
            typeof(Thickness),
            typeof(PopulationIndicator),
            default(Thickness));

        public static readonly BindableProperty StratumCodeProperty = BindableProperty.Create(
            nameof(StratumCode),
            typeof(string),
            typeof(PopulationIndicator),
            default(string));

        public static readonly BindableProperty SampleGroupCodeProperty = BindableProperty.Create(
            nameof(SampleGroupCode),
            typeof(string),
            typeof(PopulationIndicator),
            default(string));

        public static readonly BindableProperty SpeciesProperty = BindableProperty.Create(
            nameof(Species),
            typeof(string),
            typeof(PopulationIndicator),
            default(string));

        public static readonly BindableProperty LiveDeadProperty = BindableProperty.Create(
            nameof(LiveDead),
            typeof(string),
            typeof(PopulationIndicator),
            default(string));

        private TallyPopulation _tallyPopulation;

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double AltFontSize
        {
            get => (double)GetValue(AltFontSizeProperty);
            set => SetValue(AltFontSizeProperty, value);
        }

        public FontAttributes AltFontAttributes
        {
            get => (FontAttributes)GetValue(AltFontAttributesProperty);
            set => SetValue(AltFontAttributesProperty, value);
        }

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        [TypeConverter(typeof(ThicknessTypeConverter))]
        public Thickness CellPadding
        {
            get => (Thickness)GetValue(CellPaddingProperty);
            set => SetValue(CellPaddingProperty, value);
        }

        [TypeConverter(typeof(ThicknessTypeConverter))]
        public Thickness CellMargin
        {
            get => (Thickness)GetValue(CellMarginProperty);
            set => SetValue(CellMarginProperty, value);
        }

        public TallyPopulation TallyPopulation
        {
            get => _tallyPopulation;
            set
            {
                _tallyPopulation = value;
                if (value != null)
                {
                    StratumCode = value.StratumCode;
                    SampleGroupCode = value.SampleGroupCode;
                    Species = value.SpeciesCode;
                    LiveDead = value.LiveDead;
                }
            }
        }

        public string StratumCode
        {
            get => GetValue(StratumCodeProperty) as string;
            set => SetValue(StratumCodeProperty, value);
        }

        public string SampleGroupCode
        {
            get => GetValue(SampleGroupCodeProperty) as string;
            set => SetValue(SampleGroupCodeProperty, value);
        }

        public string Species
        {
            get => GetValue(SpeciesProperty) as string;
            set => SetValue(SpeciesProperty, value);
        }

        public string LiveDead
        {
            get => GetValue(LiveDeadProperty) as string;
            set => SetValue(LiveDeadProperty, value);
        }

        public PopulationIndicator()
        {
            BackgroundColor = Color.Black;
            TextColor = Color.White;

            InitializeComponent();
        }
    }
}