using CommunityToolkit.Mvvm.Input;
using CruiseDAL;
using CruiseDAL.V3.Sync;
using FScruiser.Maui.Services;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using System.Text;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class ImportViewModel : ViewModelBase
{
    private static readonly string[] FILE_EXTENSIONS = new[] { ".crz3", ".crz3db" };

    private IEnumerable<Cruise>? _cruises;
    private string? _selectedCruiseFile;
    private Cruise? _selectedCruise;
    private bool _isWorking;
    private string? _importPath;
    private ICommand? _cancelCommand;

    public ILoggingService Log { get; }
    public IDataserviceProvider DataserviceProvider { get; }
    public IFileDialogService FileDialogService { get; }
    public IFileSystemService FileSystemService { get; }
    public INatCruiseDialogService DialogService { get; }
    public ICruiseNavigationService NavigationService { get; }
    public IDeviceInfoService DeviceInfoService { get; }

    public string? ImportPath
    {
        get => _importPath;
        protected set => SetProperty(ref _importPath, value);
    }

    public bool IsWorking
    {
        get => _isWorking;
        protected set => SetProperty(ref _isWorking, value);
    }

    public string? SelectedCruiseFile
    {
        get => _selectedCruiseFile;
        protected set => SetProperty(ref _selectedCruiseFile, value);
    }

    public IEnumerable<Cruise>? Cruises
    {
        get => _cruises;
        protected set => SetProperty(ref _cruises, value);
    }

    public Cruise? SelectedCruise
    {
        get => _selectedCruise;
        protected set => SetProperty(ref _selectedCruise, value);
    }

    public ICommand BrowseFileCommand => new RelayCommand(async () => await BrowseCruiseFileAsync());

    public ICommand SelectCruiseCommand => new RelayCommand<Cruise>((cruise) => SelectCruiseForImport(cruise));

    public ICommand ImportCruiseCommand => new RelayCommand(async () => await ImportCruise());

    public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(Cancel);

    public ImportViewModel(
        IDataserviceProvider dataserviceProvider,
        IFileDialogService fileDialogService,
        IFileSystemService fileSystemService,
        INatCruiseDialogService dialogService,
        ILoggingService loggingService,
        ICruiseNavigationService navigationService,
        IDeviceInfoService deviceInfoService
        )
    {
        DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
        FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
        FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        Log = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        DeviceInfoService = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));
    }

    public async Task BrowseCruiseFileAsync()
    {
        var cruisePath = await FileDialogService.SelectCruiseFileAsync();
        if (cruisePath == null) { return; }

        var srcFileExt = Path.GetExtension(cruisePath).ToLowerInvariant();
        if (string.IsNullOrEmpty(srcFileExt) || FILE_EXTENSIONS.Contains(srcFileExt) is false)
        {
            DialogService.ShowNotification("Invalid File Type");
            return;
        }

        var importPath = GetPathForImport(cruisePath);
        if (importPath == null) { return; }

        ImportPath = importPath;

        using (var db = new CruiseDatastore_V3(importPath))
        {
            var dataservice = new SaleDataservice(db, (string)null, DeviceInfoService.DeviceID);
            var cruises = dataservice.GetCruises();

            if (cruises.Count() == 0)
            {
                DialogService.ShowNotification("File Contains No Cruises");
            }

            Cruises = cruises;
            if (cruises.Count() == 1)
            {
                SelectedCruise = cruises.First();
            }
        }
    }

    public void SelectCruiseForImport(Cruise? cruise)
    {
        if (cruise is null) { throw new ArgumentNullException(nameof(cruise)); }

        var cruiseID = cruise.CruiseID;
        var importPath = ImportPath ?? throw new NullReferenceException("ImportPath was null");

        SelectedCruise = cruise;
    }

    public bool AnalizeCruise(string path, string cruiseID, out IEnumerable<string> errors)
    {
        var eList = new List<string>();
        errors = eList;
        var db = DataserviceProvider.Database;
        using (var importDb = new CruiseDatastore_V3(path))
        {
            var cruise = importDb.From<CruiseDAL.V3.Models.Cruise>().Where("CruiseID = @p1").Query(cruiseID).Single();

            var conflictCheckOptions = new ConflictCheckOptions()
            {
                AllowDuplicateTreeNumberForNestedStrata = !(cruise.UseCrossStrataPlotTreeNumbering ?? false),
            };

            var designErrors = GetDesignMismatchErrors(importDb, db, cruiseID);
            if (designErrors.Any())
            {
                errors = designErrors;
                return false;
            }

            var conflictChecker = new ConflictChecker();

            var conflicts = conflictChecker.CheckConflicts(importDb, db, cruiseID, conflictCheckOptions);

            if (conflicts.HasConflicts)
            {
                DialogService.ShowNotification(GenerateConflitReport(conflicts));
                return false;
            }

            var cruiseChecker = new CruiseChecker();

            var cruiseConflicts = cruiseChecker.GetCruiseConflicts(importDb, db, cruiseID);
            var isCruiseInConflict = cruiseConflicts.Any();
            if (isCruiseInConflict)
            { eList.Add("Another Cruise With Same Cruise Number Already Exits"); }

            //var saleConflicts = cruiseChecker.GetSaleConflicts(importDb, db, cruiseID);
            //var isSaleInConflict = saleConflicts.Any();
            //if (isSaleInConflict)
            //{ eList.Add("Sale Number Conflict"); }

            //var plotConflicts = cruiseChecker.GetPlotConflicts(importDb, db, cruiseID);
            //if (plotConflicts.Any())
            //{ eList.Add($"{plotConflicts.Count()} Plot Conflict(s)"); }

            //var treeConflicts = cruiseChecker.GetTreeConflicts(importDb, db, cruiseID);
            //if (treeConflicts.Any())
            //{ eList.Add($"{treeConflicts.Count()} Tree Conflict(s)"); }

            //var logConflicts = cruiseChecker.GetLogConflicts(importDb, db, cruiseID);
            //if (logConflicts.Any())
            //{ eList.Add($"{logConflicts.Count()} Log Conflict(s)"); }

            //var hasDesignKeyChanges = cruiseChecker.HasDesignKeyChanges(importDb, db, cruiseID);
            //if (hasDesignKeyChanges)
            //{ eList.Add("Has changes in design key values"); }

            return !errors.Any();
        }
    }

    private string GenerateConflitReport(ConflictResolutionOptions conflicts)
    {
        var sb = new StringBuilder();
        if (conflicts.CuttingUnit.Any())
        {
            sb.AppendLine("Cutting Unit Conflicts: " + conflicts.CuttingUnit.Count());
        }
        if (conflicts.Stratum.Any())
        {
            sb.AppendLine("Stratum Conflicts: " + conflicts.Stratum.Count());
        }
        if (conflicts.SampleGroup.Any())
        {
            sb.AppendLine("Sample Group Conflicts: " + conflicts.SampleGroup.Count());
        }
        if (conflicts.Plot.Any())
        {
            sb.AppendLine("Plot Conflicts: " + conflicts.Plot.Count());
        }
        if (conflicts.PlotTree.Any())
        {
            sb.AppendLine("Plot Tree Conflicts: " + conflicts.PlotTree.Count());
        }
        if (conflicts.Tree.Any())
        {
            sb.AppendLine("Tree Conflicts: " + conflicts.Tree.Count());
        }
        if (conflicts.Log.Any())
        {
            sb.AppendLine("Log Conflicts: " + conflicts.Log.Count());
        }

        sb.AppendLine("To Resolve Conflicts Export Data From Device");
        sb.AppendLine("And Resolve Conflicts in NatCruise");

        return sb.ToString();
    }

    private IEnumerable<string> GetDesignMismatchErrors(CruiseDatastore sourceDb, CruiseDatastore destDb, String cruiseID)
    {
        var destConn = destDb.OpenConnection();
        var sourceConn = sourceDb.OpenConnection();
        try
        {
            var errorsList = new List<string>();
            if (CruiseDAL.V3.Sync.Syncers.StratumSyncer.CheckHasDesignMismatchErrors(cruiseID, sourceConn, destConn, out var stratumErrors))
            {
                errorsList.AddRange(stratumErrors);
            }

            if (CruiseDAL.V3.Sync.Syncers.SampleGroupSyncer.CheckHasDesignMismatchErrors(cruiseID, sourceConn, destConn, out var sgErrror))
            {
                errorsList.AddRange(sgErrror);
            }

            return errorsList;
        }
        finally
        {
            destDb.ReleaseConnection();
            sourceDb.ReleaseConnection();
        }
    }

    public string GetPathForImport(string path)
    {
        if (string.IsNullOrEmpty(path)) { throw new ArgumentException($"'{nameof(path)}' cannot be null or empty", nameof(path)); }
        if (File.Exists(path) == false) { throw new FileNotFoundException("File Not Found", path); }

        string importPath;
        if (System.IO.Path.GetExtension(path).ToLowerInvariant() == ".cruise")
        {
            // code should not be reachable, see file type check in BrowseCruiseFileAsync
            DialogService.ShowNotification(".cruise file type not supported");
            return null;

            //var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            //var convertedFileName = fileName + ".crz3";
            //var convertFromPath = FileSystemService.GetFileForConvert(path);
            //var convertToPath = Path.Combine(FileSystemService.ConvertTempDir, convertedFileName);

            //File.Delete(convertToPath);
            //try
            //{
            //    using var v2db = new CruiseDatastore(convertFromPath, false, null, new Updater_V2());
            //    using var v3db = new CruiseDatastore_V3(convertToPath, true);

            //    var migrator = new Migrator();
            //    if (migrator.EnsureCanMigrate(v2db, out var msg))
            //    {
            //        new Migrator().MigrateFromV2ToV3(convertFromPath, convertToPath, DeviceInfoService.DeviceID);
            //    }
            //    else
            //    {
            //        Log.LogEvent("Import Error", new Dictionary<string, string> { { "Message", msg }, });
            //        DialogService.ShowNotification(msg, "Import Error");
            //    }
            //}
            //catch (Exception e)
            //{
            //    var data = new Dictionary<string, string>() { { "FileName", path } };
            //    Log.LogException("File Error", "Unable to Migrate File", e, data);
            //    DialogService.ShowNotification("Unable to Migrate File", "File Error");

            //    return null;
            //}

            //importPath = FileSystemService.GetFileForImport(convertToPath);
        }
        else
        {
            importPath = FileSystemService.GetFileForImport(path);
        }

        return importPath;
    }

    public async Task ImportCruise()
    {
        var cruise = SelectedCruise;
        if (cruise == null) { return; }
        await ImportCruise(cruise);
    }

    public async Task ImportCruise(Cruise cruise)
    {
        var cruiseID = cruise.CruiseID;
        var importPath = ImportPath ?? throw new NullReferenceException("ImportPath was null");

        if (await ImportCruise(cruiseID, importPath) == true)
        {
            //var sampleInfoDataservice = DataserviceProvider.GetDataservice<ISampleInfoDataservice>();
            //if (sampleInfoDataservice.HasSampleStateEnvy())
            //{
            //    //DialogService.ShowNotification()
            //}

            DialogService.ShowNotification("Done");
            await NavigationService.GoBackAsync();
        }
        else
        {
            DialogService.ShowNotification("Import Failed");
        }
    }

    public async Task<bool> ImportCruise(string cruiseID, string importPath, TableSyncOptions options = null)
    {
        if (AnalizeCruise(importPath, cruiseID, out var errors) == false)
        {
            SelectedCruise = null;
            var errorStr = String.Join(Environment.NewLine, errors);
            Log.LogEvent("Import Error", new Dictionary<string, string> { { "Message", errorStr } });
            DialogService.ShowNotification(errorStr, "Cruise Can Not Be Imported");
            return false;
        }

        options ??= new TableSyncOptions(SyncOption.InsertUpdate);

        var destDb = DataserviceProvider.Database;
        using (var srcDb = new CruiseDatastore_V3(importPath))
        {
            var fromConn = srcDb.OpenConnection();
            var toConn = destDb.OpenConnection();

            var transaction = toConn.BeginTransaction();
            try
            {
                IsWorking = true;
                var syncer = new CruiseDatabaseSyncer();
                await syncer.SyncAsync(cruiseID, fromConn, toConn, options);

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Log.LogException("Import", "Import Failed", e);

                return false;
            }
            finally
            {
                IsWorking = false;
                destDb.ReleaseConnection(true);
            }
        }
    }

    public void Cancel()
    {
        NavigationService.GoBackAsync();
    }
}