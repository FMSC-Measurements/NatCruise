using CheckCruise;
using CheckCruise.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CruiseDAL;
using NatCruise.Data;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.ViewModels
{
    public partial class CreateCheckCruiseViewModel : ObservableObject, IDisposable
    {
        private bool _isDisposed;

        [ObservableProperty]
        IEnumerable<UnitStratumSelectionItem> _unitSelection;
        private string _checkCruisePath;

        public partial class UnitStratumSelectionItem : ObservableObject
        {
            [ObservableProperty]
            bool _isSelected;

            public UnitStratum Item { get; set; }
        }

        public IDataContextService DataContext { get; }
        protected dataBaseCommands CheckCruiseDataService { get; set; }
        public INatCruiseDialogService DialogService { get; }

        public string CheckCruisePath
        {
            get => _checkCruisePath;
            set => SetProperty(ref _checkCruisePath, value);
        }

        protected DAL V2_Database { get; set; }

        

        public CreateCheckCruiseViewModel(IDataContextService dataContext, INatCruiseDialogService dialogService)
        {
            DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }


        protected void OnInitialize(IDictionary<string, object> parameters)
        {
            var database = DataContext.Database;

            var cruiseID = DataContext.CruiseID;
            var migrator = new DownMigrator();
            if(!migrator.EnsureCanMigrate(cruiseID, database, out var errors))
            {
                DialogService.ShowNotification("Error Creating File: migration error ", string.Join(Environment.NewLine, errors));
            }

            var v2_db = new DAL();
            try
            {
                

                migrator.MigrateFromV3ToV2(cruiseID, database, v2_db);

                
            }
            catch
            {
                v2_db.Dispose();
                V2_Database = null;
                CheckCruiseDataService = null;
                return;
            }

            CheckCruiseDataService = new dataBaseCommands(v2_db);
            V2_Database = v2_db;


            UnitSelection = CheckCruiseDataService.GetUnitStrata().Select(x => new UnitStratumSelectionItem { Item = x });

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
                    if(DialogService.AskYesNoAsync("Check Cruise File Already Exists", "Overwrite Existing Check Cruise File?").Result == false)
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

            var checkCruiseDb = new DAL(checkCruisePath, true);

            var selectedUnits = UnitSelection.Where(x => x.IsSelected).Select(x => x.Item);
            var checkCruise = CheckCruiseDataService.CreateCheckCruise(selectedUnits);
            DialogService.ShowCheckCruise(checkCruise);
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    var v2_db = V2_Database;
                    if(v2_db != null)
                    {
                        v2_db.Dispose();
                    }
                    V2_Database = null;
                    CheckCruiseDataService = null;
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
