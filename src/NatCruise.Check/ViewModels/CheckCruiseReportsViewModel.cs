using CommunityToolkit.Mvvm.ComponentModel;
using CruiseDAL;
using Microsoft.Extensions.Logging;
using NatCruise.Check.Data;
using NatCruise.Data;
using NatCruise.MVVM;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace NatCruise.Check.ViewModels
{
    public class CheckCruiseReportsViewModel : ObservableObject, IDisposable
    {
        private string _analysisStatus;
        private bool _isAnalized;
        private string _timberSaleCruisePath;
        private bool disposedValue;
        private bool _isCheckCruiseReady;
        private string _processedCheckCruisePath;

        public string TimberSaleCruisePath
        {
            get => _timberSaleCruisePath;
            protected set
            {
                if (value == _timberSaleCruisePath) { return; }
                SetProperty(ref _timberSaleCruisePath, value);
                OnTimberSaleCruisePatch_Changed(value);
            }
        }

        protected DAL TimberSaleDatabase { get; set; }
        protected DAL CheckCruiseProcessedDatabase { get; set; }
        

        public string AnalysisStatus
        {
            get => _analysisStatus;
            protected set => SetProperty(ref _analysisStatus, value);
        }

        public string ProcessedCheckCruisePath
        {
            get => _processedCheckCruisePath;
            protected set
            {
                SetProperty(ref _processedCheckCruisePath, value);
            }
        }


        public bool IsAnalized
        {
            get => _isAnalized;
            private set
            {
                SetProperty(ref _isAnalized, value);
            }
        }

        protected ILogger? Log { get; }
        public INatCruiseDialogService DialogService { get; }
        public IDataContextService DataContext { get; }

        public CheckCruiseReportsViewModel(IDataContextService dataContextService, INatCruiseDialogService dialogService, ILogger<CheckCruiseReportsViewModel>? log)
        {
            DataContext = dataContextService ?? throw new ArgumentNullException(nameof(dataContextService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            Log = log;

        }

        protected void OnInitialize(IDictionary<string, object> parameters)
        {
            var checkFilePath = DataContext.DatabasePath;

            var fileDir = Path.GetDirectoryName(checkFilePath);
            var fileName = Path.GetFileName(checkFilePath);
            var processPath = Path.Combine(fileDir, fileName + ".process");

            if (!File.Exists(processPath))
            {
                Log?.LogInformation("No process file found for Check Cruise {CheckFilePath}", checkFilePath);
                return;
            }

            try
            {

            }
                        catch (Exception e)
            {
                Log?.LogError(e, "Error loading Check Cruise {CheckFilePath}", checkFilePath);
                DialogService.ShowNotification($"Error loading Check Cruise {e.Message}");
            }

            ProcessedCheckCruisePath = processPath;

        }

        private void OnTimberSaleCruisePatch_Changed(string tsFilePath)
        {
            if (string.IsNullOrWhiteSpace(tsFilePath)) { return; }
            var fileExtention = Path.GetExtension(tsFilePath).ToLower();

            if(fileExtention == ".crz3")
            {
                var fileDir = Path.GetDirectoryName(tsFilePath);
                var fileName = Path.GetFileName(tsFilePath);

                var processPath = Path.Combine(fileDir, fileName + ".process");
                if(!File.Exists(processPath))
                {
                    Log?.LogInformation("No process file found for Timber Sale {TsFilePath}", tsFilePath);
                    DialogService.ShowNotification($"Please process file first");
                    return;
                }

                tsFilePath = processPath;
            }

            TimberSaleDatabase = new DAL(tsFilePath);

            var tsCheckDataservice = new dataBaseCommands(TimberSaleDatabase);

            var analizer = new CheckCruiseAnalyzer((TimberSaleDatabase);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CheckCruiseReportsViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
