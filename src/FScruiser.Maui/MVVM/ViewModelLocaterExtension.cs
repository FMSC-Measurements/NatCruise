using Android.Views;
using FMSC.ORM.Logging;
using FScruiser.Maui.Util;
using Microsoft.Extensions.Logging;
using NatCruise.MVVM;

namespace FScruiser.Maui.MVVM
{
    [ContentProperty(nameof(Type))]
    public class ViewModelLocaterExtension : IMarkupExtension
    {
        public Type? Type { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Type == null) { throw new InvalidOperationException("Type should not be null"); }

            var targetElement = serviceProvider.GetRequiredService<IProvideValueTarget>().TargetObject;

            var context = Application.Current?.FindMauiContext();
            if (context == null) throw new InvalidOperationException("Count find MauiContext");
            //var app = IPlatformApplication.Current;
            //if (app == null) throw new InvalidOperationException("IPlatformApplication.Current  not set");

            try
            {
                var viewModel = context.Services.GetRequiredService(Type);

                if (targetElement is Page view && viewModel is ViewModelBase)
                {
                    WireView(view);
                }

                return viewModel;
            }
            catch (Exception e)
            {
                context.Services.GetRequiredService<ILogger<ViewModelLocaterExtension>>().LogError(e, "Error Resolving " +Type.Name);
                throw;
            }
        }

        public static void WireView(Page view)
        {
            view.Appearing += View_Appearing;
        }

        private static void View_Appearing(object? sender, EventArgs e)
        {
            var view = sender as Page;
            if(view == null) { return; }
            var viewModel = view.BindingContext as ViewModelBase;
            if (viewModel == null)
            {
                throw new InvalidOperationException("Expected ViewModel");
                //return;
            }

            viewModel.Load();
        }
    }
}