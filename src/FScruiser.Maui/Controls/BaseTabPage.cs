using Microsoft.Extensions.Logging;
using NatCruise.MVVM;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Controls
{
    public class BaseTabbedPage : TabbedPage
    {
        protected ILogger? Log { get; init; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {


            if (BindingContext is ITheRealInitialize viewModel && viewModel != null)
            {
                try
                {
                    Log?.LogInformation("Initializing View Model {viewModelType} with {navParams}", viewModel.GetType().Name, query);

                    viewModel.Initialize(query);
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, "Error Initializing ViewModel");
                    throw;
                }
            }
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is ViewModelBase viewModel && viewModel != null)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    viewModel.Load();
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, "Error Loading ViewModel");
                    throw;
                }

                stopwatch.Stop();

                var tenthsecond = stopwatch.ElapsedMilliseconds / 100; //round to tenth of second, this helps reduces the number of data points in reporting
                var viewModelType = this.GetType().Name;
                Log?.LogInformation("View Model Loaded {time_TenthSec}, {view_model_type:time_TenthSec}, {view_model_type}",
                    tenthsecond.ToString(),
                    viewModelType + ":" + tenthsecond.ToString(),
                    viewModelType);

            }
        }
    }
}
