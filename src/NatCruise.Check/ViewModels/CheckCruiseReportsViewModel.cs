using CommunityToolkit.Mvvm.ComponentModel;
using CruiseDAL;
using Microsoft.Extensions.Logging;
using NatCruise.Check.Data;
using NatCruise.Check.Models;
using NatCruise.Check.Reports;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace NatCruise.Check.ViewModels
{
    public class CheckCruiseReportsViewModel : ObservableObject, IDisposable
    {
        private const string TIMESTAMP_FORMAT = "yyyyMMddhhmm";

        private string _analysisStatus;
        private bool _isAnalized;
        private string _timberSaleCruisePath;
        private bool disposedValue;
        private bool _isCheckCruiseReady;
        private string _processedCheckCruisePath;
        private string _checkCruiserInitials;
        private IReadOnlyCollection<string> _checkCruiserInitialsOptions;
        private string _cruiserInitials;
        private string _cruiserName;
        private IReadOnlyCollection<string> _cruiserInitialsOptions;
        private ToleranceConfig _toleranceConfig;
        private AnalysisResult _analysisResult;
        private ReportLevel _reportLevel;

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

        public Sale Sale { get; protected set; }

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

        public ReportLevel ReportLevel
        {
            get => _reportLevel;
            set => SetProperty(ref _reportLevel, value);
        }

        public string CruiserInitials
        {
            get => _cruiserInitials;
            set => SetProperty(ref _cruiserInitials, value);
        }

        public string CruiserName
        {
            get => _cruiserName;
            set => SetProperty(ref _cruiserName, value);
        }

        public IReadOnlyCollection<string> CruiserInitialsOptions
        {
            get => _cruiserInitialsOptions;
            protected set => SetProperty(ref _cruiserInitialsOptions, value);
        }

        public string CheckCruiserInitials
        {
            get => _checkCruiserInitials;
            set => SetProperty(ref _checkCruiserInitials, value);
        }



        public IReadOnlyCollection<string> CheckCruiserInitialsOptions
        {
            get => _checkCruiserInitialsOptions;
            protected set => SetProperty(ref _checkCruiserInitialsOptions, value);
        }
        
        public ToleranceConfig ToleranceConfig
        {
            get => _toleranceConfig;
            set => SetProperty(ref _toleranceConfig, value);
        }

        public AnalysisResult AnalysisResult
        {
            get => _analysisResult;
            set => SetProperty(ref _analysisResult, value);
        }

        protected ILogger? Log { get; }
        public INatCruiseDialogService DialogService { get; }
        public IContainerProvider Services { get; }
        public IDataContextService DataContext { get; }
        public ISaleDataservice SaleDataservice { get; }

        public ObservableCollection<ReportOutput> Reports { get; private set; } = new ObservableCollection<ReportOutput>();

        public CheckCruiseReportsViewModel(IDataContextService dataContextService, ISaleDataservice saleDataservice, INatCruiseDialogService dialogService, IContainerProvider services, ILogger<CheckCruiseReportsViewModel>? log)
        {
            DataContext = dataContextService ?? throw new ArgumentNullException(nameof(dataContextService));
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Log = log;

        }

        protected void OnInitialize(IDictionary<string, object> parameters)
        {
            // when initializing the view model we should already have the check cruise
            // we just need to check that it has been processed, load check cruiser initials, ...

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
                CheckCruiseProcessedDatabase = new DAL(processPath);
                CheckCruiserInitialsOptions = SaleDataservice.GetCruisers();
                Sale = SaleDataservice.GetSale();

                ProcessedCheckCruisePath = processPath;
            }
            catch (Exception e)
            {
                Log?.LogError(e, "Error loading Check Cruise {CheckFilePath}", checkFilePath);
                DialogService.ShowNotification($"Error loading Check Cruise {e.Message}");
            }

        }

        private void OnTimberSaleCruisePatch_Changed(string tsFilePath)
        {
            var checkCruiserInitials = CheckCruiserInitials;
            var toleranceConfig = ToleranceConfig;

            OnAnalyze(tsFilePath, checkCruiserInitials, toleranceConfig);
        }

        public void OnAnalyze(string tsFilePath, string checkCruiserInitials, ToleranceConfig toleranceConfig)
        {
            if (string.IsNullOrWhiteSpace(tsFilePath)) { return; }


            // check if its a V3 or V2 file
            // if its a V3 file then we need to find the process file and use that instead
            var fileExtention = Path.GetExtension(tsFilePath).ToLower();
            if (fileExtention == ".crz3")
            {
                var fileDir = Path.GetDirectoryName(tsFilePath);
                var fileName = Path.GetFileName(tsFilePath);

                var processPath = Path.Combine(fileDir, fileName + ".process");
                if (!File.Exists(processPath))
                {
                    Log?.LogInformation("No process file found for Timber Sale {TsFilePath}", tsFilePath);
                    DialogService.ShowNotification($"Please process file first");
                    return;
                }

                tsFilePath = processPath;
            }

            var tsDatabase = new DAL(tsFilePath);


            var tsCheckDataservice = new dataBaseCommands(TimberSaleDatabase);
            var checkCruiseDataService = new dataBaseCommands(CheckCruiseProcessedDatabase);

            var analizer = new CheckCruiseAnalyzer(checkCruiseDataService, tsCheckDataservice, Services?.Resolve<ILogger<CheckCruiseAnalyzer>>());
            var analisisResults = analizer.Analyze(checkCruiserInitials, toleranceConfig);
            AnalysisResult = analisisResults;
            TimberSaleDatabase = tsDatabase;
            CruiserInitialsOptions = tsDatabase.QueryScalar<string>("SELECT DISTINCT Initials FROM Tree;").ToArray();
        }

        public void GenerateEvaluationReport()
        {
            var checkCruiseDataService = new dataBaseCommands(CheckCruiseProcessedDatabase);
            var evaluationReportGenerator = new EvaluationReportBuilder(checkCruiseDataService);
            var sale = Sale;

            var timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            var reportLevelStr = ReportLevel switch
            {
                ReportLevel.BySale => "BySale",
                ReportLevel.ByCruiser => "ByCruiser",
                _ => throw new InvalidEnumArgumentException(nameof(ReportLevel)),
            };
            

            using var writer = new StringWriter();
            evaluationReportGenerator.BuildReport(writer, ToleranceConfig, ReportLevel, CruiserName, CruiserInitials);

            var evaluationReportFileName = $"{sale.SaleNumber}_{sale.Name}_{timestamp}_Eval_{reportLevelStr}.rtf";
            var checkCruiseDir = Path.GetDirectoryName(DataContext.DatabasePath);
            var evaluationReportPath = Path.Combine(checkCruiseDir, evaluationReportFileName);
            var report = new ReportOutput()
            {
                Content = writer.ToString(),
                Path = evaluationReportPath,
                FileName = evaluationReportFileName,
            };

            SaveReportOutput(report);

        }

        public void SaveReportOutput(ReportOutput reprot)
        {
            Log?.LogInformation("Saving Report {ReportFileName} To {ReportPath}", reprot.FileName, reprot.Path);

            //var file = new FileInfo(reprot.Path);
            using var writer = new StreamWriter(reprot.Path);
            writer.Write(reprot.Content);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Reports = null;

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

    public class ReportOutput
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
    }
}
