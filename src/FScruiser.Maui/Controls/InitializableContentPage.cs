using FScruiser.Maui.Util;
using Microsoft.Extensions.Logging;
using NatCruise.Navigation;

namespace FScruiser.Maui.Controls;

public class InitializableContentPage : ContentPage, IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is ITheRealInitialize viewModel)
        {

            try
            {
                viewModel.Initialize(query);
            }
            catch (Exception ex)
            {
                this.FindMauiContext()?.Services.GetRequiredService<ILogger<InitializableContentPage>>()?.LogError(ex, "Error Initializing ViewModel");
                throw;
            }
        }
    }
}