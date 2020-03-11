using System;
using NatCruise.Cruise.Models;

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
            propertyChanged: (view, oldValue, newValue) => FontSizePropertyChanged(view as PopulationIndicator, (double)oldValue, (double)newValue));
        private static void FontSizePropertyChanged(PopulationIndicator self, double oldValue, double newValue)
        {
            self._stratumLabel.FontSize = newValue;
            self._sgLabel.FontSize = newValue;
            self._spLabel.FontSize = newValue;
            self._ldLabel.FontSize = newValue;
        }

        public static readonly BindableProperty StratumCodeProperty = BindableProperty.Create(
            nameof(StratumCode),
            typeof(string),
            typeof(PopulationIndicator),
            propertyChanged: (view, oldValue, newValue) => StratumCodePropertyChanged(view as PopulationIndicator, oldValue as string, newValue as string));

        private static void StratumCodePropertyChanged(PopulationIndicator self, string oldValue, string newValue)
        {
            self._stratumLabel.Text = newValue;
        }

        public static readonly BindableProperty SampleGroupCodeProperty = BindableProperty.Create(
            nameof(SampleGroupCode),
            typeof(string),
            typeof(PopulationIndicator),
            propertyChanged: (view, oldValue, newValue) => SampleGroupCodePropertyChanged(view as PopulationIndicator, oldValue as string, newValue as string));

        private static void SampleGroupCodePropertyChanged(PopulationIndicator self, string oldValue, string newValue)
        {
            self._sgLabel.Text = newValue;
        }

        public static readonly BindableProperty SpeciesProperty = BindableProperty.Create(
            nameof(Species),
            typeof(string),
            typeof(PopulationIndicator),
            propertyChanged: (view, oldValue, newValue) => SpeciesPropertyChanged(view as PopulationIndicator, oldValue as string, newValue as string));

        private static void SpeciesPropertyChanged(PopulationIndicator self, string oldValue, string newValue)
        {
            self._spLabel.Text = newValue;
            var hasValue = string.IsNullOrEmpty(newValue) == false;
            self._spLabel.IsVisible = hasValue;
            self._spLableSplitter.IsVisible = hasValue;
        }

        public static readonly BindableProperty LiveDeadProperty = BindableProperty.Create(
            nameof(LiveDead),
            typeof(string),
            typeof(PopulationIndicator),
            propertyChanged: (view, oldValue, newValue) => LiveDeadPropertyChanged(view as PopulationIndicator, oldValue as string, newValue as string));

        private static void LiveDeadPropertyChanged(PopulationIndicator self, string oldValue, string newValue)
        {
            self._ldLabel.Text = newValue;
            var hasValue = string.IsNullOrEmpty(newValue) == false;
            self._ldLabel.IsVisible = hasValue;
            self._ldLableSplitter.IsVisible = hasValue;
        }

        private TallyPopulation_Base _tallyPopulation;

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public TallyPopulation_Base TallyPopulation
        {
            get => _tallyPopulation;
            set
            {
                _tallyPopulation = value;
                if (value != null)
                {
                    StratumCode = value.StratumCode;
                    SampleGroupCode = value.SampleGroupCode;
                    Species = value.Species;
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
            InitializeComponent();
        }
    }
}