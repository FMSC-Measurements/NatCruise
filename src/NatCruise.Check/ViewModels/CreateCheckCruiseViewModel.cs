using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CruiseDAL;
using Microsoft.Extensions.Logging;
using NatCruise.Check.Data;
using NatCruise.Check.Models;
using NatCruise.Data;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.ViewModels
{
    public partial class CreateCheckCruiseViewModel : ObservableObject, IDisposable
    {
        private bool _isDisposed;

        private IReadOnlyCollection<UnitStratumSelectionItem> _unitSelection;
        private string _checkCruisePath;
        private RelayCommand _createCheckCruiseCommand;

        public partial class UnitStratumSelectionItem : ObservableObject
        {
            [ObservableProperty]
            private bool _isSelected;

            public UnitStratum Item { get; set; }
        }

        public IReadOnlyCollection<UnitStratumSelectionItem> UnitSelection
        {
            get => _unitSelection;
            set
            {
                if (_unitSelection != null)
                {
                    foreach (var item in _unitSelection)
                    {
                        item.PropertyChanged -= UnitSelectionItem_PropertyChanged;
                    }
                }
                SetProperty(ref _unitSelection, value);
                if (value != null)
                {
                    foreach (var item in _unitSelection)
                    {
                        item.PropertyChanged += UnitSelectionItem_PropertyChanged;
                    }
                }
            }
        }

        public bool IncludeLogRecords { get; set; }

        public bool IncludeTreeMeasurments { get; set; }

        public IDataContextService DataContext { get; }
        protected dataBaseCommands CheckCruiseDataService { get; set; }
        public INatCruiseDialogService DialogService { get; }
        public ILogger Log { get; }

        public string CheckCruisePath
        {
            get => _checkCruisePath;
            set => SetProperty(ref _checkCruisePath, value);
        }

        //protected DAL V2_Database { get; set; }

        public RelayCommand CreateCheckCruiseCommand => _createCheckCruiseCommand ??= new RelayCommand(CreateCheckCruise, CanCreateCheckCruise);

        public CreateCheckCruiseViewModel(IDataContextService dataContext, , INatCruiseDialogService dialogService, ILogger<CreateCheckCruiseViewModel> log)
        {
            DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            Log = log;
        }

        protected void OnInitialize(IDictionary<string, object> parameters)
        {
            var database = DataContext.Database;

            //var cruiseID = DataContext.CruiseID;
            //var migrator = new DownMigrator();
            //if(!migrator.EnsureCanMigrate(cruiseID, database, out var errors))
            //{
            //    DialogService.ShowNotification("Error Creating File: migration error ", string.Join(Environment.NewLine, errors));
            //}

            //var v2_db = new DAL();
            //try
            //{
            //    migrator.MigrateFromV3ToV2(cruiseID, database, v2_db);

            //}
            //catch
            //{
            //    v2_db.Dispose();
            //    V2_Database = null;
            //    CheckCruiseDataService = null;
            //    return;
            //}

            //CheckCruiseDataService = new dataBaseCommands(v2_db);
            //V2_Database = v2_db;

            UnitSelection = CheckCruiseDataService.GetUnitStrata()
                .Select(x => new UnitStratumSelectionItem { Item = x })
                .ToArray();

            var currentCruisePath = database.Path;
            var currentCruiseDir = Path.GetDirectoryName(currentCruisePath);

            var sale = CheckCruiseDataService.GetSale();
        }

        public bool CanCreateCheckCruise()
        {
            if (string.IsNullOrEmpty(CheckCruisePath)) { return false; }
            if (UnitSelection == null) { return false; }
            return UnitSelection.Any(x => x.IsSelected);
        }

        public void CreateCheckCruise()
        {
            var checkCruisePath = CheckCruisePath;
            if (string.IsNullOrEmpty(checkCruisePath)
                )
            {
                DialogService.ShowNotification("No Check Cruise Path Selected", "");
                return;
            }

            var checkCruiseDirectoryPath = Path.GetDirectoryName(checkCruisePath);
            var checkCruiseFileDir = new DirectoryInfo(checkCruiseDirectoryPath);
            try
            {
                if (!checkCruiseFileDir.Exists)
                {
                    checkCruiseFileDir.Create();
                }
                else
                {
                    //var dirAttrs = checkCruiseFileDir.Attributes;
                    //dirAttrs.HasFlag(FileAttributes.)
                }
            }
            catch (System.IO.IOException e)
            {
                DialogService.ShowNotification("Error Creating Directory", e.Message);
                return;
            }

            FileInfo checkCruiseFile;
            try
            {
                checkCruiseFile = new FileInfo(checkCruisePath);
                if (checkCruiseFile.Exists)
                {
                    if (DialogService.AskYesNoAsync("Check Cruise File Already Exists", "Overwrite Existing Check Cruise File?").Result == false)
                    {
                        return;
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                DialogService.ShowNotification("Error Creating File", e.Message);
                return;
            }

            var selectedUnits = UnitSelection.Where(x => x.IsSelected).Select(x => x.Item).ToArray();
            var destinationCruiseID = Guid.NewGuid().ToString();

            var checkCruiseDb = new CruiseDatastore_V3(checkCruisePath, true);
            try
            {
                var checkCruiseCopier = new CheckCruiseCopier(selectedUnits)
                {
                    IncludeLogRecords = IncludeLogRecords,
                    IncludeTreeMeasurments = IncludeTreeMeasurments,
                };
                checkCruiseCopier.Copy(DataContext.Database, checkCruiseDb, DataContext.CruiseID, destinationCruiseID: destinationCruiseID);
            }
            catch (Exception e)
            {
                Log?.LogWarning(e, "Error Creating Check Cruise");
                DialogService.ShowNotification("Error Creating File", e.Message);
                return;
            }
            finally
            {
                checkCruiseDb.Dispose();
            }

            // open check cruise
            //DataContext.OpenDatabase(checkCruisePath);
            //DataContext.CruiseID = destinationCruiseID;
        }

        private void UnitSelectionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateCheckCruiseCommand.NotifyCanExecuteChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //var v2_db = V2_Database;
                    //if (v2_db != null)
                    //{
                    //    v2_db.Dispose();
                    //}
                    //V2_Database = null;
                    CheckCruiseDataService = null;
                    UnitSelection = null;
                }

                _isDisposed = true;
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