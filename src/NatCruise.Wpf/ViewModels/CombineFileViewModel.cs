using ControlzEx.Standard;
using CruiseDAL;
using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync;
using CruiseDAL.V3.Sync.Syncers;
using CruiseDAL.V3.Sync.Util;
using FMSC.ORM.SQLite;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class CombineFileViewModel : ViewModelBase
    {
        private const string TIMESTAMP_FORMAT = "yyyyMMddhhmm";

        private CrewFileInfo _currentSyncFile;
        private FileInfo _destinationFile;
        private NatCruise.Models.Cruise _cruise;
        private ObservableCollection<CrewFileInfo> _crewFiles;
        private string _outputFileName;
        private ICommand _selectFilesCommand;
        private ICommand _startCombineFilesCommand;
        private ICommand _resolveConflictAndContinueCommand;
        private bool _isSyncRunning;
        private ICommand _selectOutputFileCommand;

        public CombineFileViewModel(IFileDialogService fileDialogService, INatCruiseDialogService dialogService, ILoggingService loggingService)
        {
            CrewFiles = new ObservableCollection<CrewFileInfo>();

            Syncer = new CruiseDatabaseSyncer();
            ConflictChecker = new ConflictChecker();
            ConflictResolver = new ConflictResolver();
            Options = new TableSyncOptions(SyncOption.InsertUpdate);

            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        protected IFileDialogService FileDialogService { get; }
        protected INatCruiseDialogService DialogService { get; }
        protected ILoggingService LoggingService { get; }

        protected ConflictChecker ConflictChecker { get; }
        protected ConflictResolver ConflictResolver { get; }
        protected CruiseDatabaseSyncer Syncer { get; }

        public TableSyncOptions Options { get; }

        public event EventHandler ConflictsDetected;

        public string CruiseID => Cruise?.CruiseID;

        public NatCruise.Models.Cruise Cruise
        {
            get => _cruise;
            protected set => SetProperty(ref _cruise, value);
        }

        public ObservableCollection<NatCruise.Models.Cruise> CruiseOptions { get; set; } = new ObservableCollection<NatCruise.Models.Cruise>();

        public ObservableCollection<CrewFileInfo> CrewFiles
        {
            get => _crewFiles;
            protected set => SetProperty(ref _crewFiles, value);
        }

        public Queue<CrewFileInfo> SyncQue { get; protected set; }

        public string OutputFileName
        {
            get => _outputFileName;
            set => SetProperty(ref _outputFileName, value);
        }

        public CrewFileInfo CurrentSyncFile
        {
            get => _currentSyncFile;
            protected set => SetProperty(ref _currentSyncFile, value);
        }

        public bool IsSyncRunning
        {
            get => _isSyncRunning;
            protected set
            {
                SetProperty(ref _isSyncRunning, value);
                RaisePropertyChanged(nameof(CanStartSync));
            }
        }

        public bool CanStartSync => !IsSyncRunning && CrewFiles.Any();

        #region commands

        public ICommand SelectFilesCommand => _selectFilesCommand ??= new DelegateCommand(() => SelectFiles().FireAndForget());

        public ICommand SelectOutputFileCommand => _selectOutputFileCommand ??= new DelegateCommand(() => SelectOutputFile().FireAndForget());

        public ICommand StartCombineFilesCommand => _startCombineFilesCommand ??= new DelegateCommand(() => StartSync().FireAndForget());

        public ICommand ResolveConflictsAndContinueCommand => _resolveConflictAndContinueCommand ??= new DelegateCommand(() => ResolveConflictsAndContinue().FireAndForget());

        #endregion commands

        public async Task SelectFiles()
        {
            var files = await FileDialogService.SelectCruiseFilesAsync();

            SelectFiles(files);
        }

        public void SelectFiles(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var cruiseRecords = GetCruises(path);
                if (cruiseRecords.Any() is false)
                {
                    DialogService.ShowNotification("Warning: " + path + " doesn't contain any cruises");
                }
                else
                {
                    AddCruiseOptions(cruiseRecords);

                    if (Cruise == null)
                    {
                        var cruise = Cruise = cruiseRecords.First();
                        var destFileName = OutputFileName = GetDestinationPathFromCruise(path, cruise);
                    }
                    else
                    {
                        if (!cruiseRecords.Any(x => x.CruiseID == Cruise.CruiseID))
                        {
                            DialogService.ShowNotification("Warning: " + path + " doesn't contain selected cruise");
                            continue;
                        }
                    }

                    CrewFiles.Add(new CrewFileInfo(path));
                }
            }

            RaisePropertyChanged(nameof(CanStartSync));
        }

        protected void AddCruiseOptions(IEnumerable<NatCruise.Models.Cruise> cruises)
        {
            var cruiseOptions = CruiseOptions;
            foreach (var cruise in cruises)
            {
                if (!cruiseOptions.Any(x => x.CruiseID == cruise.CruiseID))
                {
                    cruiseOptions.Add(cruise);
                }
            }
        }

        public string GetDestinationPathFromCruise(string path, NatCruise.Models.Cruise cruise)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            if (!System.IO.File.Exists(path)) throw new System.IO.FileNotFoundException();

            var timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            var mergeFileName = $"{cruise.SaleNumber}_{cruise.SaleName}_{cruise.PurposeShortCode?.Replace(' ', '_')}_{timestamp}_MergeFile.crz3";

            var dir = Path.GetDirectoryName(path);

            return Path.Combine(dir, mergeFileName);
        }

        public NatCruise.Models.Cruise[] GetCruises(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            if (!System.IO.File.Exists(path)) throw new System.IO.FileNotFoundException();

            // use sqlitedatastore to prevent any unnecessary modification of the original file
            // either by updates or logging. if the file needs updating, the sync temp will be updated
            using var tempFileDb = new SQLiteDatastore(path);
            var cruses = tempFileDb.From<NatCruise.Models.Cruise>()
                .LeftJoin("LK_Purpose", "USING (Purpose)")
                .Query().ToArray();
            return cruses;
        }

        public static string GetWorkingDirectory(string destPath)
        {
            var destDir = Path.GetDirectoryName(destPath);

            var baseWorkingDirName = "Sync_Temp_" + DateTime.Today.ToString("yyyy_MM_dd");
            var workingDir = Path.Combine(destDir, baseWorkingDirName);

            return workingDir;
        }

        public Task StartSync()
        {
            var syncQue = SyncQue = new Queue<CrewFileInfo>(CrewFiles);
            

            foreach (var file in syncQue)
            {
                file.DeleteSyncTempIfExists();
            }

            // initialize destination file
            var firstFile = syncQue.Dequeue();
            firstFile.OriginalFile.CopyTo(OutputFileName, true);
            firstFile.IsSynced = true;
            firstFile.Status = CrewFileInfo.FileStatus.Combined;

            IsSyncRunning = true;
            return RunSync();
        }

        public Task ResolveConflictsAndContinue()
        {
            var currentFile = CurrentSyncFile;

            if (!currentFile.Conflicts.AllHasResolutions())
            {
                DialogService.ShowNotification("Please Make Sure All Conflicts Have Resolutions Before Continuing");
                return Task.CompletedTask;
            }

            currentFile.EnsureSyncTemp();
            var srcDb = new SQLiteDatastore(currentFile.SyncTempFile.FullName);
            var destDb = new CruiseDatastore_V3(OutputFileName);
            ConflictResolver.ResolveConflicts(srcDb, destDb, currentFile.Conflicts);

            return RunSync();
        }

        public async Task SelectOutputFile()
        {
            string defaultDir = null;
            string defaultFileName = null;
            if(OutputFileName != null)
            {
                defaultDir = Path.GetDirectoryName(OutputFileName);
                defaultFileName = Path.GetFileName(OutputFileName);
            }

            var filePath = await FileDialogService.SelectCruiseFileDestinationAsync(defaultDir, defaultFileName);
            if (filePath != null)
            {
                OutputFileName = filePath;
            }
        }

        protected async Task RunSync()
        {
            var syncQue = SyncQue;

            do
            {
                var nextFile = syncQue.Peek();
                CurrentSyncFile = nextFile;

                CheckFile(nextFile);

                if (nextFile.Conflicts.HasConflicts || nextFile.DesignErrors.Any())
                {
                    DialogService.ShowNotification("File Has Conflicts");
                    ConflictsDetected?.Invoke(this, EventArgs.Empty);
                    return;
                }

                try
                {
                    var syncResult = await SyncFile(nextFile);
                    nextFile.SyncResult = syncResult;
                    nextFile.Status = CrewFileInfo.FileStatus.Combined;
                    nextFile.IsSynced = true;
                    CurrentSyncFile = null;
                    nextFile.DeleteSyncTempIfExists();
                    syncQue.Dequeue();
                }
                catch (Exception e)
                {
                    DialogService.ShowNotification("Sync Error: " + Environment.NewLine
                        + "::::" + e.GetType().Name + ":::: " + e.Message);

                    LoggingService.LogException(nameof(CombineFileViewModel), nameof(RunSync), e);
                }
            } while (syncQue.Count > 0);

            OnSyncDone();
        }

        protected void OnSyncDone()
        {
            IsSyncRunning = false;
            DialogService.ShowNotification("Done Combining Files");

            Cruise = null;
            CurrentSyncFile = null;
            CrewFiles.Clear();
            OutputFileName = null;
        }

        protected async Task<SyncResult> SyncFile(CrewFileInfo file)
        {
            var destDb = new CruiseDatastore_V3(OutputFileName);

            file.EnsureSyncTemp();
            var sourceDb = new CruiseDatastore(file.SyncTempFile.FullName, false, builder: null, updater: new Updater_V3());

            var destConn = destDb.OpenConnection();
            var srcCon = sourceDb.OpenConnection();

            var destTransaction = destConn.BeginTransaction();
            try
            {
                var syncResult = await Syncer.SyncAsync(CruiseID, srcCon, destConn, Options, progress: file.Progress);
                destTransaction.Commit();
                return syncResult;
            }
            catch
            {
                destTransaction.Rollback();
                throw;
            }
            finally
            {
                destDb.ReleaseConnection();
                sourceDb.ReleaseConnection();
            }
        }

        protected void CheckFile(CrewFileInfo file)
        {
            var destDb = new CruiseDatastore_V3(OutputFileName);

            file.EnsureSyncTemp();
            // create an in memory database to check for conflicts
            var sourceDb = new CruiseDatastore(file.SyncTempFile.FullName, false, builder: null, updater: new Updater_V3());
            var workingSourceDb = new SQLiteDatastore();
            sourceDb.BackupDatabase(workingSourceDb);

            var cruiseID = CruiseID;

            // run the sync operation from the destination to the source
            // this allows records that have been modified in the destination
            // to be updated pre-conflict-check removing false positive conflicts
            // where the destination record has been moved out of conflict
            var preCheckSyncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                CuttingUnit = SyncOption.Update,
                CuttingUnitStratum = SyncOption.Update,
                Stratum = SyncOption.Update,
                SampleGroup = SyncOption.Update,
                Plot = SyncOption.Update,
                TreeAuditResolution = SyncOption.Update,
                Log = SyncOption.Update,
                Stem = SyncOption.Update,
            };
            Syncer.Sync(cruiseID, destDb, workingSourceDb, preCheckSyncOptions);

            var conflicts = ConflictChecker.CheckConflicts(workingSourceDb, destDb, cruiseID);
            file.Conflicts = conflicts;

            var designErrors = new List<string>();

            var destConn = destDb.OpenConnection();
            var sourceConn = sourceDb.OpenConnection();
            try
            {
                var errorsList = new List<string>();
                if (StratumSyncer.CheckHasDesignMismatchErrors(cruiseID, sourceConn, destConn, out var stratumErrors))
                {
                    errorsList.AddRange(stratumErrors);
                }

                if (SampleGroupSyncer.CheckHasDesignMismatchErrors(cruiseID, sourceConn, destConn, out var sgErrror))
                {
                    errorsList.AddRange(sgErrror);
                }

                file.DesignErrors = designErrors;
            }
            finally
            {
                destDb.ReleaseConnection();
                sourceDb.ReleaseConnection();
            }
        }

        public class CrewFileInfo : INPC_Base
        {
            public enum FileStatus { NotCombined, Combined }

            public CrewFileInfo(string originalPath)
            {
                var originalFile = OriginalFile = new FileInfo(originalPath);
                var tempSourcePath = originalFile.FullName + ".syncTemp~";
                SyncTempFile = new FileInfo(tempSourcePath);
                Progress = new Progress<float>();
            }

            private Progress<float> _progress;
            private float _progressValue;
            private bool _isChecked;
            private bool _isSynced;
            private ConflictResolutionOptions _conflicts;
            private IEnumerable<string> _designErrors;
            private FileStatus _status;

            public FileInfo OriginalFile { get; set; }

            public FileInfo SyncTempFile { get; set; }

            public SyncResult SyncResult { get; set; }

            public ConflictResolutionOptions Conflicts
            {
                get => _conflicts;
                set => SetProperty(ref _conflicts, value);
            }

            public IEnumerable<string> DesignErrors
            {
                get => _designErrors;
                set => SetProperty(ref _designErrors, value);
            }

            public bool IsSynced
            {
                get => _isSynced;
                set => SetProperty(ref _isSynced, value);
            }

            public bool IsChecked
            {
                get => _isChecked;
                set => SetProperty(ref _isChecked, value);
            }

            public FileStatus Status
            {
                get => _status;
                set => SetProperty(ref _status, value);
            }

            public Progress<float> Progress
            {
                get => _progress;
                set
                {
                    if (_progress != null)
                    { _progress.ProgressChanged -= ProgressChanged; }
                    _progress = value;
                    ProgressValue = 0.0f;
                    if (value != null)
                    { value.ProgressChanged += ProgressChanged; }

                    void ProgressChanged(object sender, float p)
                    {
                        ProgressValue = p;
                    }
                }
            }

            public float ProgressValue
            {
                get => _progressValue;
                protected set => SetProperty(ref _progressValue, value);
            }

            public void DeleteSyncTempIfExists()
            {
                if (File.Exists(SyncTempFile.FullName))
                {
                    SyncTempFile.Delete();
                }
            }

            public void EnsureSyncTemp()
            {
                // HACK FileInfo.Exists is apparently not updating after file
                // created so I'm using File.Exists to make sure I'm getting
                // the current status of the file
                if (!File.Exists(SyncTempFile.FullName))
                {
                    OriginalFile.CopyTo(SyncTempFile.FullName, true);
                }
            }
        }
    }
}