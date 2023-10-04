using Xamarin.Forms;

namespace FScruiser.XF.Controls
{
    public class DataTemplateContentControl : ContentView
    {
        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate),
            typeof(DataTemplate),
            typeof(DataTemplateContentControl),
            propertyChanged: ContentTemplateChanged);

        private static void ContentTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as DataTemplateContentControl;
            control.CreateContentView();
        }

        private bool _deferLayout;

        protected override bool ShouldInvalidateOnChildRemoved(View child) => !_deferLayout;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            CreateContentView();
        }

        protected void CreateContentView()
        {
            try
            {
                var view = CreateTemplateForItem(BindingContext, ContentTemplate);
                _deferLayout = true;
                Content = null;
                _deferLayout = false;
                Content = view;
            }
            catch
            {
                Content = null;
            }
        }

        public static View CreateTemplateForItem(object bindingContext, DataTemplate itemTemplate)
        {
            DataTemplate template = itemTemplate is DataTemplateSelector templateSelector ? templateSelector.SelectTemplate(bindingContext, null) : itemTemplate;

            return template.CreateContent() as View;
        }
    }
}