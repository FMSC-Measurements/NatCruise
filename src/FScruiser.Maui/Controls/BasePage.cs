using FScruiser.Maui.MVVM;
using FScruiser.Maui.Util;
using Microsoft.Extensions.Logging;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Controls
{
    public class BasePage : ContentPage, IQueryAttributable
    {
        protected ILogger? Log { get; init; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            

            if (BindingContext is ITheRealInitialize viewModel && viewModel != null)
            {
                try
                {
                    viewModel.Initialize(query);
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, "Error Initializing ViewModel");
                    throw;
                }
            }
        }
    }
}
