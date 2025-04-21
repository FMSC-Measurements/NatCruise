using CommunityToolkit.Mvvm.ComponentModel;
using CruiseDAL;
using Microsoft.Extensions.Logging;
using NatCruise.Wpf.CheckCruise.Data;
using NatCruise.Wpf.CheckCruise.Models;
using NatCruise.Wpf.CheckCruise.Reports;
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
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

#nullable enable

namespace NatCruise.Wpf.CheckCruise.ViewModels
{
    public class CheckCruiseReportsViewModel : ViewModelBase, IDisposable
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
        private AsyncRelayCommand _showBrowseOriginalCruiseDialogCommand;
        private RelayCommand _generateEvaluationReportCommand;
        private RelayCommand _generateSummaryReportCommand;
        private DAL _checkCruiseV2Database;

        public string TimberSaleCruisePath
        {
            get => _timberSaleCruisePath;
            protected set
            {
                if (value == _timberSaleCruisePath) { return; }
                SetProperty(ref _timberSaleCruisePath, value);
            }
        }

        protected DAL TimberSaleDatabase { get; set; }
        protected DAL CheckCruiseProcessedDatabase
        {
            get => _checkCruiseV2Database;
            set
            {
                SetProperty(ref _checkCruiseV2Database, value);
                OnPropertyChanged(nameof(IsCheckCruiseLoaded));
            }
        }

        public bool IsCheckCruiseLoaded => CheckCruiseProcessedDatabase != null;

        public Sale Sale { get; protected set; }

        public string AnalysisStatus
        {
            get => _analysisStatus;
            protected set => SetProperty(ref _analysisStatus, value);
        }

        public Progress<string> AnalysisProgress { get; protected set; }
        private void HandleAnalysisProgressChanged(string obj)
        {
            AnalysisStatus = obj;
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
        public DefaultTolerances DefaultTolerancesDataService { get; }
        public IDataContextService DataContext { get; }
        public ISaleDataservice SaleDataservice { get; }

        public ObservableCollection<ReportOutput> Reports { get; private set; } = new ObservableCollection<ReportOutput>();

        public AsyncRelayCommand ShowBrowseOriginalCruiseDialogCommand => _showBrowseOriginalCruiseDialogCommand ??= new AsyncRelayCommand(ShowBrowseOriginalCruiseDialogAsync);

        public RelayCommand GenerateEvaluationReportCommand => _generateEvaluationReportCommand ??= new RelayCommand(GenerateEvaluationReport);

        public RelayCommand GenerateSummaryReportCommand => _generateSummaryReportCommand ??= new RelayCommand(GenerateSummaryReport);



        public CheckCruiseReportsViewModel(IDataContextService dataContextService, ISaleDataservice saleDataservice, INatCruiseDialogService dialogService, IContainerProvider services, ILogger<CheckCruiseReportsViewModel>? log)
        {
            DataContext = dataContextService ?? throw new ArgumentNullException(nameof(dataContextService));
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            DefaultTolerancesDataService = new DefaultTolerances();
            Log = log;

            AnalysisProgress = new Progress<string>(HandleAnalysisProgressChanged);

        }

        protected override void OnInitialize(IDictionary<string, object> parameters)
        {
            // when initializing the view model we should already have the check cruise
            // we just need to check that it has been processed, load check cruiser initials, ...

            var checkFilePath = DataContext.DatabasePath;

            var fileDir = Path.GetDirectoryName(checkFilePath);
            var fileName = Path.GetFileNameWithoutExtension(checkFilePath);
            var processPath = Path.Combine(fileDir, fileName + ".process");

            if (!File.Exists(processPath))
            {
                Log?.LogInformation("No process file found for Check Cruise {CheckFilePath}", checkFilePath);
                return;

                var v2Db = new DAL();

                var downConverter = new DownMigrator();
                downConverter.MigrateFromV3ToV2(DataContext.CruiseID, DataContext.Database, v2Db);
                CheckCruiseProcessedDatabase = v2Db;
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

            ToleranceConfig = DefaultTolerancesDataService.GetDefaultConfig(int.Parse(Sale.Region));

        }

        private void OnTimberSaleCruisePatch_Changed(string tsFilePath)
        {
            
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
                var fileName = Path.GetFileNameWithoutExtension(tsFilePath);

                var processPath = Path.Combine(fileDir, fileName + ".process");
                if (!File.Exists(processPath))
                {
                    Log?.LogInformation("No process file found for Timber Sale {TsFilePath}", tsFilePath);
                    DialogService.ShowNotification($"Please process file first");
                    return;
                }

                tsFilePath = processPath;
            }
            else if (fileExtention == ".cruise")
            {
                // do nothing
            }
            else
            {
                DialogService.ShowNotification("Unknown File Type");
            }

                var tsDatabase = new DAL(tsFilePath);
            var tsCheckDataservice = new dataBaseCommands(tsDatabase);
            var checkCruiseDataService = new dataBaseCommands(CheckCruiseProcessedDatabase);

            var analizer = new CheckCruiseAnalyzer(checkCruiseDataService, tsCheckDataservice, Services?.Resolve<ILogger<CheckCruiseAnalyzer>>());
            var analisisResults = analizer.Analyze(checkCruiserInitials, toleranceConfig);
            AnalysisResult = analisisResults;
            IsAnalized = true;
            TimberSaleDatabase = tsDatabase;
            CruiserInitialsOptions = tsDatabase.QueryScalar<string>("SELECT DISTINCT Initials FROM Tree;").ToArray();
        }

        public async Task AnalyzeAsync(string tsFilePath, string checkCruiserInitials, ToleranceConfig toleranceConfig)
        {
            if (string.IsNullOrWhiteSpace(tsFilePath)) { return; }


            // check if its a V3 or V2 file
            // if its a V3 file then we need to find the process file and use that instead
            var fileExtention = Path.GetExtension(tsFilePath).ToLower();
            if (fileExtention == ".crz3")
            {
                var fileDir = Path.GetDirectoryName(tsFilePath);
                var fileName = Path.GetFileNameWithoutExtension(tsFilePath);

                var processPath = Path.Combine(fileDir, fileName + ".process");
                if (!File.Exists(processPath))
                {
                    Log?.LogInformation("No process file found for Timber Sale {TsFilePath}", tsFilePath);
                    DialogService.ShowNotification($"Please process file first");
                    return;
                }

                tsFilePath = processPath;
            }
            else if (fileExtention == ".cruise")
            {
                // do nothing
            }
            else
            {
                DialogService.ShowNotification("Unknown File Type");
            }

            var tsDatabase = new DAL(tsFilePath);
            var tsCheckDataservice = new dataBaseCommands(tsDatabase);
            var checkCruiseDataService = new dataBaseCommands(CheckCruiseProcessedDatabase);

            var analizer = new CheckCruiseAnalyzer(checkCruiseDataService, tsCheckDataservice, Services?.Resolve<ILogger<CheckCruiseAnalyzer>>());
            var analisisResults = await analizer.AnalyzeAsync(checkCruiserInitials, toleranceConfig, AnalysisProgress);
            AnalysisResult = analisisResults;
            IsAnalized = true;
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
            evaluationReportGenerator.BuildReport(writer, ToleranceConfig, AnalysisResult, ReportLevel, CruiserName, CruiserInitials);

            var evaluationReportFileName = $"{sale.SaleNumber}_{sale.Name}_{timestamp}_Eval_{reportLevelStr}.rtf";
            var checkCruiseDir = Path.GetDirectoryName(DataContext.DatabasePath);
            var evaluationReportPath = Path.Combine(checkCruiseDir, evaluationReportFileName);
            var report = new ReportOutput()
            {
                Content = writer.ToString(),
                FilePath = evaluationReportPath,
                FileName = evaluationReportFileName,
            };

            SaveReportOutput(report);

            Reports.Add(report);

        }

        public void GenerateSummaryReport()
        {
            var checkCruiseDataService = new dataBaseCommands(CheckCruiseProcessedDatabase);
            var originalCruiseDataService = new dataBaseCommands(TimberSaleDatabase);
            var summaryReportBuilder = new SummaryReportBuilder(checkCruiseDataService, originalCruiseDataService, "0.0.0");
            var sale = Sale;

            var timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            var reportLevelStr = ReportLevel switch
            {
                ReportLevel.BySale => "BySale",
                ReportLevel.ByCruiser => "ByCruiser",
                _ => throw new InvalidEnumArgumentException(nameof(ReportLevel)),
            };


            using var writer = new StringWriter();
            summaryReportBuilder.BuildReport(writer, CruiserInitials, ToleranceConfig, AnalysisResult, ReportLevel);

            var evaluationReportFileName = $"{sale.SaleNumber}_{sale.Name}_{timestamp}_Summary_{reportLevelStr}.rtf";
            var checkCruiseDir = Path.GetDirectoryName(DataContext.DatabasePath);
            var evaluationReportPath = Path.Combine(checkCruiseDir, evaluationReportFileName);
            var report = new ReportOutput()
            {
                Content = writer.ToString(),
                FilePath = evaluationReportPath,
                FileName = evaluationReportFileName,
            };

            SaveReportOutput(report);

            Reports.Add(report);
        }

        public void SaveReportOutput(ReportOutput reprot)
        {
            Log?.LogInformation("Saving Report {ReportFileName} To {ReportPath}", reprot.FileName, reprot.FilePath);

            //var file = new FileInfo(reprot.Path);
            using var writer = new StreamWriter(reprot.FilePath);
            writer.Write(reprot.Content);
        }

        public async Task ShowBrowseOriginalCruiseDialogAsync()
        {
            var curCruisePath = DataContext.DatabasePath;
            var curDir = Path.GetDirectoryName(curCruisePath);

            var dialog = new OpenFileDialog()
            {
                InitialDirectory = curDir,
                DefaultExt = "*.crz3",
                Filter = "All Cruise file types|*.crz3;*.cruise|" +
                         "cruise files V3 (*.crz3)|*.crz3|" +
                         "cruise file V2 (*.cruise)|*.cruise" 
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var filePath = dialog.FileName;


                var checkCruiserInitials = CheckCruiserInitials;
                var toleranceConfig = ToleranceConfig;

                await AnalyzeAsync(filePath, checkCruiserInitials, toleranceConfig);

                TimberSaleCruisePath = filePath;
            }
            else
            {
                AnalysisResult = null;
                IsAnalized = false;
                TimberSaleDatabase = null;
                CruiserInitialsOptions = null;
                TimberSaleCruisePath = null;
            }
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
        public string FilePath { get; set; }
        public string Content { get; set; }
    }
}
