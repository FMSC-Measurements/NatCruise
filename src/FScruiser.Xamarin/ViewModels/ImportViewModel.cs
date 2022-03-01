using CruiseDAL;
using CruiseDAL.V3.Sync;
using FScruiser.XF.Services;
using NatCruise;
using NatCruise.Core.Services;
using NatCruise.Cruise.Data;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class ImportViewModel : XamarinViewModelBase
    {
        private static readonly string[] FILE_EXTENSIONS = new[] { ".crz3", ".crz3db" };

        private IEnumerable<Cruise> _cruises;
        private string _selectedCruiseFile;
        private Cruise _selectedCruise;
        private bool _isWorking;
        private string _importPath;

        public ILoggingService Log { get; }
        public IDataserviceProvider DataserviceProvider { get; }
        public IFileDialogService FileDialogService { get; }
        public IFileSystemService FileSystemService { get; }
        public IDialogService DialogService { get; }
        public ICruiseNavigationService NavigationService { get; }
        public IDeviceInfoService DeviceInfoService { get; }

        public string ImportPath
        {
            get => _importPath;
            protected set => SetProperty(ref _importPath, value);
        }

        public bool IsWorking
        {
            get => _isWorking;
            protected set => SetProperty(ref _isWorking, value);
        }

        public string SelectedCruiseFile
        {
            get => _selectedCruiseFile;
            protected set => SetProperty(ref _selectedCruiseFile, value);
        }

        public IEnumerable<Cruise> Cruises
        {
            get => _cruises;
            protected set => SetProperty(ref _cruises, value);
        }

        public Cruise SelectedCruise
        {
            get => _selectedCruise;
            protected set => SetProperty(ref _selectedCruise, value);
        }

        public ICommand BrowseFileCommand => new DelegateCommand(async () => await BrowseCruiseFileAsync());

        public ICommand SelectCruiseCommand => new DelegateCommand<Cruise>((cruise) => SelectCruiseForImport(cruise));

        public ICommand ImportCruiseCommand => new DelegateCommand(async () => await ImportCruise());

        private ICommand cancelCommand;
        public ICommand CancelCommand => cancelCommand ??= new Command(Cancel);

        public ImportViewModel(
            IDataserviceProvider dataserviceProvider,
            IFileDialogService fileDialogService,
            IFileSystemService fileSystemService,
            IDialogService dialogService,
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
            if(importPath == null) { return; }

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

        public void SelectCruiseForImport(Cruise cruise)
        {
            if (cruise is null) { throw new ArgumentNullException(nameof(cruise)); }

            var cruiseID = cruise.CruiseID;
            var importPath = ImportPath ?? throw new NullReferenceException("ImportPath was null");
            if (AnalizeCruise(importPath, cruiseID, out var errors) == false)
            {
                SelectedCruise = null;
                var errorStr = String.Join(Environment.NewLine, errors);
                DialogService.ShowNotification(errorStr, "Cruise Can Not Be Imported");
                return;
            }

            SelectedCruise = cruise;
        }

        public bool AnalizeCruise(string path, string cruiseID, out IEnumerable<string> errors)
        {
            var eList = new List<string>();
            errors = eList;
            var db = DataserviceProvider.Database;
            using (var importDb = new CruiseDatastore_V3(path))
            {
                var cruiseChecker = new CruiseChecker();

                var cruiseConflicts = cruiseChecker.GetCruiseConflicts(importDb, db, cruiseID);
                var isCruiseInConflict = cruiseConflicts.Any();
                if(isCruiseInConflict)
                { eList.Add("Cruise Number Conflict"); }

                //var saleConflicts = cruiseChecker.GetSaleConflicts(importDb, db, cruiseID);
                //var isSaleInConflict = saleConflicts.Any();
                //if(isSaleInConflict)
                //{ eList.Add("Sale Number Conflict"); }

                var plotConflicts = cruiseChecker.GetPlotConflicts(importDb, db, cruiseID);
                if(plotConflicts.Any())
                { eList.Add($"{plotConflicts.Count()} Plot Conflict(s)"); }

                var treeConflicts = cruiseChecker.GetTreeConflicts(importDb, db, cruiseID);
                if(treeConflicts.Any())
                { eList.Add($"{treeConflicts.Count()} Tree Conflict(s)"); }

                var logConflicts = cruiseChecker.GetLogConflicts(importDb, db, cruiseID);
                if(logConflicts.Any())
                { eList.Add($"{logConflicts.Count()} Log Conflict(s)"); }

                var hasDesignKeyChanges = cruiseChecker.HasDesignKeyChanges(importDb, db, cruiseID);
                if(hasDesignKeyChanges)
                { eList.Add("Has chages in design key values"); }

                return !errors.Any() ;
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

        public async Task<bool> ImportCruise(string cruiseID, string importPath, CruiseSyncOptions options = null)
        {
            options ??= new CruiseSyncOptions()
            {
                Design = SyncFlags.InsertUpdate,
                TreeFlags = SyncFlags.InsertUpdate,
                TreeDataFlags = SyncFlags.InsertUpdate,
                FieldData = SyncFlags.InsertUpdate,
                SamplerState = SyncFlags.InsertUpdate,
                Validation = SyncFlags.InsertUpdate,
                Processing = SyncFlags.InsertUpdate,
                TreeDefaultValue = SyncFlags.InsertUpdate,
                Template = SyncFlags.InsertUpdate,

            };

            var destDb = DataserviceProvider.Database;
            using (var srcDb = new CruiseDatastore_V3(importPath))
            {
                var fromConn = srcDb.OpenConnection();
                var toConn = destDb.OpenConnection();
                try
                {
                    IsWorking = true;
                    var syncer = new CruiseSyncer();
                    await syncer.SyncAsync(cruiseID, fromConn, toConn, options);
                    return true;
                }
                catch (Exception e)
                {
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
}