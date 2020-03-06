using Backpack.XF.Converters;
using CSharpForMarkup;
using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogsListPage : ContentPage
    {
        private const int COLUMN_COUNT = 3;
        private const int error_col_width = 50;
        private static readonly IValueConverter _greaterThanZeroConverter =
            new ComparisonConverter<int>()
            {
                Default = true,
                GreaterThan = true,
                CompareToValue = 0
            };

        #region LogFields

        /// <summary>
        /// Identifies the <see cref="LogFields"/> bindable property.
        /// </summary>
        private static readonly BindableProperty logFieldsProperty =
            BindableProperty.Create(nameof(LogFields),
              typeof(IEnumerable<LogFieldSetup>),
              typeof(LogsListPage),
              defaultValue: default(IEnumerable<LogFieldSetup>),
              defaultBindingMode: BindingMode.OneWay,
              propertyChanged: (bindable, oldValue, newValue) => ((LogsListPage)bindable).OnLogFieldsChanged(oldValue, newValue));

        /// <summary>
        /// Invoked after changes have been applied to the <see cref="LogFields"/> property.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="LogFields"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="LogFields"/> property.</param>
        protected virtual void OnLogFieldsChanged(object oldValue, object newValue)
        {
            if (newValue is IEnumerable<LogFieldSetup> fields && fields != null)
            {
                _logListView.ItemTemplate = new DataTemplate(() =>
                {
                    return CreateViewCellFromLogFields(fields);
                });
            }
        }

        private static ViewCell CreateViewCellFromLogFields(IEnumerable<LogFieldSetup> fields)
        {
            var grid = new Grid() { Padding = 5 };
            for (int i = 0; i < COLUMN_COUNT; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = error_col_width });

            grid.Children.Add(
                new Label()
                { FontAttributes = FontAttributes.Bold }
                    .Bind(Label.TextProperty, "LogNumber", stringFormat: "Log #{0}")
                    .ColSpan(3)
                    .Col(0)
                    .Row(0));

            foreach (var (field, index) in fields.Where(x => x.Field != "LogNumber").Select((v, i) => (v, i)))
            {
                var frame = new Frame()
                {
                    Padding = 5,
                    BackgroundColor = Color.Blue,
                    Content = new Label
                    { TextColor = Color.White }
                    .Bind(Label.TextProperty, field.Field, stringFormat: $"{field.Heading}: {{0}}")
                    ,
                }
                .Col(index % COLUMN_COUNT)
                .Row((index / COLUMN_COUNT) + 1);//ajust down one because logNumber is first

                grid.Children.Add(frame);
            }

            var errorCountIndicator = new Frame()
            {
                BackgroundColor = Color.Red,
                CornerRadius = 5,
                VerticalOptions = LayoutOptions.Center,
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Xamarin.Forms.Image {Source = new FileImageSource{ File = "ic_error_outline_black_24dp.png"}},
                        new Label
                        {
                            TextColor = Color.Black,
                            VerticalTextAlignment = TextAlignment.Center
                        }
                        .Bind(Label.TextProperty, "ErrorCount"),
                    }
                }.Padding(0, 0)
            }
            .Padding(0, 0)
            .Bind(Frame.IsVisibleProperty, "ErrorCount", converter: _greaterThanZeroConverter)
            .Col(COLUMN_COUNT + 1)
            .Row(0);

            grid.Children.Add(errorCountIndicator);

            var viewCell = new ViewCell
            {
                View = grid
            };

            return viewCell;
        }

        /// <summary>
        /// Gets or sets the <see cref="LogFields" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public IEnumerable<LogFieldSetup> LogFields
        {
            get { return (IEnumerable<LogFieldSetup>)GetValue(LogFieldsProperty); }
            set { SetValue(LogFieldsProperty, value); }
        }

        public static BindableProperty LogFieldsProperty => logFieldsProperty;

        #endregion LogFields

        public LogsListPage()
        {
            InitializeComponent();

            this.SetBinding(logFieldsProperty, "LogFields");

            _logListView.ItemTapped += _logListView_ItemTapped;


        }

        private void _logListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (BindingContext is LogsListViewModel vm
                && e.Item is Log log && log != null)
            {
                vm.ShowEditLogPage(log);
            }

            _logListView.SelectedItem = null; //deselect selected item
        }
    }
}