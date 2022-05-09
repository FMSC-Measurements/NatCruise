using CruiseDAL;
using CruiseDAL.V3.Sync;
using NatCruise.Data;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class CombineFileViewModel : ViewModelBase, IDisposable
    {
        private CruiseDatastore_V3 _sourceDatabase;
        private IEnumerable<Conflict> _cuttingUnitConflicts;
        private IEnumerable<Conflict> _stratumConflicts;
        private IEnumerable<Conflict> _sampleGroupConflicts;
        private IEnumerable<Conflict> _plotConflicts;
        private IEnumerable<Conflict> _treeConflicts;
        private IEnumerable<Conflict> _logConflicts;
        private ICommand _selectSourceFileCommand;
        private ICommand _resolveConflicts;
        private ICommand _combineFileCommand;
        private bool _canCombine;
        private IEnumerable<Conflict> _plotTreeConflicts;
        private ConflictResolutionOptions _conflictOptions;
        private bool _disposedValue;
        private Progress<double> _progress;
        private double _progressValue;
        private CruiseDatastore_V3 _workingDestinationDb;
        private CruiseDatastore_V3 _workingSourceDb;

        public CombineFileViewModel(IFileDialogService fileDialogService, IDataserviceProvider dataserviceProvider, IDialogService dialogService)
        {
            Syncer = new CruiseSyncer();
            DeleteSyncer = new DeleteSysncer();
            ConflictChecker = new ConflictChecker();
            ConflictResolver = new ConflictResolver();
            Options = new CruiseSyncOptions();

            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            DestinationDatabase = dataserviceProvider.Database;
            CruiseID = dataserviceProvider.CruiseID;
        }

        protected Progress<double> Progress
        {
            get => _progress;
            set
            {
                if(_progress != null)
                { _progress.ProgressChanged -= ProgressChanged; }
                _progress = value;
                ProgressValue = 0.0;
                if(value != null)
                { value.ProgressChanged += ProgressChanged; }

                void ProgressChanged(object sender, double p)
                {
                    ProgressValue = p;
                }
            }
        }

        public double ProgressValue
        {
            get => _progressValue;
            protected set => SetProperty(ref _progressValue, value);
        }

        public string FileName { get; protected set; }
        public string FilePath { get; protected set; }

        public ConflictChecker ConflictChecker { get; protected set; }
        public ConflictResolver ConflictResolver { get; }
        public CruiseSyncer Syncer { get; protected set; }
        public DeleteSysncer DeleteSyncer { get; }
        public string CruiseID { get; protected set; }

        public IFileDialogService FileDialogService { get; }
        public IDialogService DialogService { get; }

        public CruiseDatastore_V3 SourceDatabase
        {
            get => _sourceDatabase;
            set
            {
                if (value != null)
                {
                    var path = value.Path;
                    var fileName = System.IO.Path.GetFileName(path);
                    _sourceDatabase = value;
                    FileName = fileName;
                    FilePath = path;
                }
                else
                {
                    _sourceDatabase = null;
                    FileName = "";
                    FilePath = "";
                }
                RaisePropertyChanged(nameof(FileName));
                RaisePropertyChanged(nameof(FilePath));
            }
        }

        public CruiseDatastore_V3 DestinationDatabase { get; set; }

        public CruiseDatastore_V3 WorkingSourceDb
        {
            get => _workingSourceDb;
            set
            {
                if(_workingSourceDb != null)
                { _workingSourceDb.Dispose(); }
                _workingSourceDb = value;
            }
        }

        public CruiseDatastore_V3 WorkingDestinationDb
        {
            get => _workingDestinationDb;
            set
            {
                if(_workingDestinationDb != null)
                { _workingDestinationDb.Dispose();}
                _workingDestinationDb = value;
            }
        }

        public CruiseSyncOptions Options { get; protected set; }

        #region Conflict Properties

        public ConflictResolutionOptions ConflictOptions
        {
            get => _conflictOptions;
            protected set
            {
                SetProperty(ref _conflictOptions, value);
                RaisePropertyChanged(nameof(CuttingUnitConflicts));
                RaisePropertyChanged(nameof(StratumConflicts));
                RaisePropertyChanged(nameof(SampleGroupConflicts));
                RaisePropertyChanged(nameof(PlotConflicts));
                RaisePropertyChanged(nameof(TreeConflicts));
                RaisePropertyChanged(nameof(PlotTreeConflicts));
                RaisePropertyChanged(nameof(LogConflicts));
            }
        }

        public IEnumerable<Conflict> CuttingUnitConflicts => ConflictOptions?.CuttingUnit;

        public IEnumerable<Conflict> StratumConflicts => ConflictOptions?.Stratum;

        public IEnumerable<Conflict> SampleGroupConflicts => ConflictOptions?.SampleGroup;

        public IEnumerable<Conflict> PlotConflicts => ConflictOptions?.Plot;

        public IEnumerable<Conflict> TreeConflicts => ConflictOptions?.Tree;

        public IEnumerable<Conflict> PlotTreeConflicts => ConflictOptions?.PlotTree;

        public IEnumerable<Conflict> LogConflicts => ConflictOptions?.Log;

        #endregion Conflict Properties

        #region Commands

        public ICommand SelectSourceFileCommand => _selectSourceFileCommand ??= new DelegateCommand(() => SelectSourceFile().FireAndForget());

        public ICommand ResolveConflictsCommand => _resolveConflicts ??= new DelegateCommand(() => ResolveConflicts().FireAndForget());

        public ICommand CombineFileCommand => _combineFileCommand ??= new DelegateCommand(() => CombineFile().FireAndForget());

        #endregion Commands

        public void CheckFile()
        {
            ConflictOptions = CheckDatabases(WorkingSourceDb, WorkingDestinationDb);
        }

        protected ConflictResolutionOptions CheckDatabases(CruiseDatastore_V3 sourceDb, CruiseDatastore_V3 destDb)
        {
            var checker = ConflictChecker;
            var cruiseID = CruiseID;

            var source = sourceDb.OpenConnection();
            var dest = destDb.OpenConnection();
            try
            {
                var unitConflicts = checker.CheckCuttingUnits(source, dest, cruiseID).ToArray();
                var stratumConflicts = checker.CheckStrata(source, dest, cruiseID).ToArray();
                var sampleGroupConflicts = checker.CheckSampleGroups(source, dest, cruiseID).ToArray();
                var plotConflicts = checker.CheckPlots(source, dest, cruiseID).ToArray();
                var treeConflicts = checker.CheckTrees(source, dest, cruiseID).ToArray();
                var plotTreeConflicts = checker.CheckPlotTrees(source, dest, cruiseID).ToArray();
                var logConflicts = checker.CheckLogs(source, dest, cruiseID).ToArray();

                var conflicts = new ConflictResolutionOptions(
                        cuttingUnit: unitConflicts,
                        strata: stratumConflicts,
                        sampleGroup: sampleGroupConflicts,
                        plot: plotConflicts,
                        tree: treeConflicts,
                        plotTree: plotTreeConflicts,
                        log: logConflicts
                    );

                return conflicts;
            }
            finally
            {
                sourceDb.ReleaseConnection();
                destDb.ReleaseConnection();
            }
        }

        public async Task ResolveConflicts()
        {
            var source = WorkingSourceDb;
            var destination = WorkingDestinationDb;

            var conflictOptions = ConflictOptions;

            if (!conflictOptions.AllHasResolutions())
            {
                DialogService.ShowNotification("Please Resolve All Conflicts Before Continuing");
                return;
            }

            //var progress = new Progress<float>();

            var sourceConn = source.OpenConnection();
            try
            {
                var destConn = destination.OpenConnection();
                try
                {
                    var resolver = ConflictResolver;
                    resolver.ResolveUnitConflicts(sourceConn, destConn, conflictOptions.CuttingUnit);

                    resolver.ResolveStratumConflicts(sourceConn, destConn, conflictOptions.Stratum);

                    resolver.ResolveSampleGroupConflicts(sourceConn, destConn, conflictOptions.SampleGroup);

                    resolver.ResolvePlotConflicts(sourceConn, destConn, conflictOptions.Plot);

                    resolver.ResolveTreeConflicts(sourceConn, destConn, conflictOptions.Tree);

                    resolver.ResolveTreeConflicts(sourceConn, destConn, conflictOptions.PlotTree);

                    resolver.ResolveLogConflicts(sourceConn, destConn, conflictOptions.Log);
                }
                finally
                {
                    destination.ReleaseConnection();
                }

                //await DialogService.ShowNotificationAsync("done", "message");
            }
            finally
            {
                source.ReleaseConnection();
            }
        }

        public async Task CombineFile()
        {
            await ResolveConflicts();

            var source = WorkingSourceDb;
            var destination = WorkingDestinationDb;
            var cruiseID = CruiseID;

            var conflicts = CheckDatabases(source, destination);

            if (conflicts.HasAny())
            {
                ConflictOptions = conflicts;

                DialogService.ShowNotification("Some conflicts remain unresolved, please recheck conflicts and ensure all conflicts have a resolution");
                return;
            }

            var progress = new Progress<double>();
            Progress = progress;
            

            var sourceConn = source.OpenConnection();
            try
            {
                var destConn = destination.OpenConnection();
                try
                {
                    await Syncer.SyncAsync(cruiseID, sourceConn, destConn, Options, progress: progress);
                }
                finally
                {
                    destination.ReleaseConnection();
                }

                destination.BackupDatabase(DestinationDatabase);

                await DialogService.ShowNotificationAsync("combine complete", "message");
            }
            finally
            {
                source.ReleaseConnection();
            }

            destination.Dispose();
            source.Dispose();
            WorkingDestinationDb = null;
            WorkingSourceDb = null;
        }

        public async Task SelectSourceFile()
        {
            var path = await FileDialogService.SelectCruiseFileAsync();

            if (string.IsNullOrEmpty(path) is false && System.IO.File.Exists(path))
            {
                try
                {
                    var cruiseID = CruiseID;
                    var database = new CruiseDatastore_V3(path);

                    var hasCruise = database.From<CruiseDAL.V3.Models.Cruise>()
                        .Where("CruiseID = @p1")
                        .Count(CruiseID) == 1;
                    if (hasCruise)
                    {
                        SourceDatabase = database;

                        var workingDestDb = new CruiseDatastore_V3();
                        DestinationDatabase.BackupDatabase(workingDestDb);

                        var workingSourceDb = new CruiseDatastore_V3();
                        SourceDatabase.BackupDatabase(workingSourceDb);

                        WorkingDestinationDb = workingDestDb;
                        WorkingSourceDb = workingSourceDb;

                        // run the delete in both directions
                        // we want to delete deleted records in the source as well as the destination
                        // so that conflicts that have already been resolved in the destination by deleting a record
                        // no longer cause conflicts in the source, as well we don't want records deleted from the source
                        // to cause conflicts in the destination 
                        DeleteSyncer.Sync(cruiseID, workingSourceDb, workingDestDb, Options);
                        DeleteSyncer.Sync(cruiseID, workingDestDb, workingSourceDb, Options);


                        // run the sync operation from the destination to the source
                        // this allows records that have been modified in the destination
                        // to be updated pre-conflict-check removing false positive conflicts
                        // where the destination record has been moved out of conflict
                        var preCheckSyncOptions = new CruiseSyncOptions
                        {
                            Design = SyncFlags.Update,
                            FieldData = SyncFlags.Update,
                            TreeFlags = SyncFlags.Update,
                            Processing = SyncFlags.Lock,
                            Template = SyncFlags.Lock,
                            SamplerState = SyncFlags.Lock,
                            TreeDataFlags = SyncFlags.Lock,
                            TreeDefaultValue = SyncFlags.Lock,
                            Validation = SyncFlags.Lock,
                        };
                        Syncer.Sync(cruiseID, workingDestDb, workingSourceDb, preCheckSyncOptions);
                        // TODO way need a separate syncer that just updates record's identities 

                        CheckFile();
                    }
                    else
                    {
                        DialogService.ShowNotification("Selected File Does not Contain Cruise Matches the Current Cruise");
                    }
                }
                catch
                {
                    WorkingDestinationDb = null;
                    WorkingSourceDb = null;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    WorkingDestinationDb = null;
                    WorkingSourceDb = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}