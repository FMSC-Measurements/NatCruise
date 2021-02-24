﻿using CruiseDAL;
using CruiseDAL.V3.Sync;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Services;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class ImportViewModel : ViewModelBase
    {
        private IEnumerable<Cruise> _cruises;
        private string _selectedCruiseFile;
        private Cruise _selectedCruise;

        public ILoggingService Log { get; }
        public IDataserviceProvider DataserviceProvider { get; }
        public IFileDialogService FileDialogService { get; }
        public IFileSystemService FileSystemService { get; }
        public IDialogService DialogService { get; }
        public string ImportPath { get; protected set; }

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

        public ICommand BrowseFileCommand => new Command(async () => await BrowseCruiseFileAsync());

        public ICommand SelectCruiseCommand => new Command<Cruise>(async (cruise) => await SelectCruiseForImport(cruise));

        public ICommand ImportCruiseCommand => new Command(ImportCruise);

        public ImportViewModel(
            IDataserviceProvider dataserviceProvider,
            IFileDialogService fileDialogService,
            IFileSystemService fileSystemService,
            IDialogService dialogService,
            ILoggingService loggingService)
        {
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            Log = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task BrowseCruiseFileAsync()
        {
            var cruisePath = await FileDialogService.SelectCruiseFileAsync();

            if(cruisePath == null) { return; }

            var importPath = GetPathForImport(cruisePath);
            ImportPath = importPath;

            using (var db = new CruiseDatastore_V3(importPath))
            {
                var dataservice = new SaleDataservice(db, (string)null);
                var cruises = dataservice.GetCruises();

                if (cruises.Count() == 0)
                {
                    DialogService.ShowNotification("File Contains No Cruises");
                }



                Cruises = cruises;
                if (cruises.Count() == 1)
                {

                }
            }
        }

        public async Task SelectCruiseForImport(Cruise cruise)
        {
            if (cruise is null) { throw new ArgumentNullException(nameof(cruise)); }

            var cruiseID = cruise.CruiseID;
            var importPath = ImportPath ?? throw new NullReferenceException("ImportPath was null");
            if (await AnalizeCruiseAsync(importPath, cruiseID) == false)
            {
                SelectedCruise = null;
                DialogService.ShowNotification("Cruise Can Not Be Imported");
                return;
            }

            SelectedCruise = cruise;
        }

        public Task<bool> AnalizeCruiseAsync(string path, string cruise)
        {
            return Task.Run(() => AnalizeCruise(path, cruise));
        }

        public bool AnalizeCruise(string path, string cruiseID)
        {
            using (var db = DataserviceProvider.GetDatabase())
            using (var importDb = new CruiseDatastore_V3(path))
            {
                var cruiseChecker = new CruiseChecker();

                var cruiseConflicts = cruiseChecker.GetCruiseConflicts(importDb, db, cruiseID);
                var isCruiseInConflict = cruiseConflicts.Any();

                var saleConflicts = cruiseChecker.GetSaleConflicts(importDb, db, cruiseID);
                var isSaleInConflict = saleConflicts.Any();

                
                var plotConflicts = cruiseChecker.GetPlotConflicts(importDb, db, cruiseID);
                var treeConflicts = cruiseChecker.GetTreeConflicts(importDb, db, cruiseID);
                var logConflicts = cruiseChecker.GetLogConflicts(importDb, db, cruiseID);
                var hasDesignKeyChanges = cruiseChecker.HasDesignKeyChanges(importDb, db, cruiseID);

                return plotConflicts.Count() == 0
                    && treeConflicts.Count() == 0
                    && logConflicts.Count() == 0
                    && !hasDesignKeyChanges
                    && !isCruiseInConflict
                    && !isSaleInConflict;
            }

        }

        public string GetPathForImport(string path)
        {
            if (string.IsNullOrEmpty(path)) { throw new ArgumentException($"'{nameof(path)}' cannot be null or empty", nameof(path)); }
            if (File.Exists(path) == false) { throw new FileNotFoundException("File Not Found", path); }

            string importPath;
            if (System.IO.Path.GetExtension(path).ToLowerInvariant() == ".cruise")
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                var convertedFileName = fileName + ".crz3";
                var convertFromPath = FileSystemService.GetFileForConvert(path);
                var convertToPath = Path.Combine(FileSystemService.ConvertTempDir, convertedFileName);

                File.Delete(convertToPath);
                try
                {
                    Migrator.MigrateFromV2ToV3(convertFromPath, convertToPath);
                }
                catch (Exception e)
                {
                    var data = new Dictionary<string, string>() { { "FileName", path } };
                    Log.LogException("File Error", "Unable to Migrate File", e, data);
                    DialogService.ShowNotification("Unable to Migrate File", "File Error");

                    return null;
                }

                importPath = FileSystemService.GetFileForImport(convertToPath);
            }
            else
            {
                importPath = FileSystemService.GetFileForImport(path);
            }

            return importPath;

        }

        public void ImportCruise()
        {
            var cruise = SelectedCruise;
            if(cruise == null) { return;}
            ImportCruise(cruise);
        }

        public void ImportCruise(Cruise cruise)
        {
            var cruiseID = cruise.CruiseID;
            var importPath = ImportPath ?? throw new NullReferenceException("ImportPath was null");

            using (var destDb = DataserviceProvider.GetDatabase())
            using (var srcDb = new CruiseDatastore_V3(importPath))
            {
                var srcConn = srcDb.OpenConnection();
                var destConn = destDb.OpenConnection();

                ImportCruise(cruiseID, srcConn, destConn);
            }
            DialogService.ShowNotification("Done");
        }

        public void ImportCruise(string cruiseID, DbConnection fromConn, DbConnection toConn, CruiseSyncOptions options = null)
        {
            options ??= new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.Insert,
                SamplerState = SyncFlags.Insert,
                Validation = SyncFlags.Insert,
            };

            try
            {
                var syncer = new CruiseSyncer();
                syncer.Sync(cruiseID, fromConn, toConn, options);
            }
            catch(Exception e)
            {
                Log.LogException("Import", "Import Failed", e);
                DialogService.ShowNotification("Import Failed");
            }

        }

        protected override void Refresh(INavigationParameters parameters)
        {
            
        }
    }
}