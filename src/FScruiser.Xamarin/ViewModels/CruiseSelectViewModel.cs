using CruiseDAL;
using CruiseDAL.V3.Sync;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using NatCruise.Services;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class CruiseSelectViewModel : XamarinViewModelBase
    {
        //public ICommand SelectCruiseCommand => new

        const string EXPORT_TIMESTAMP_FORMAT = "yyyyMMddhhmm";

        private Sale _sale;
        private IEnumerable<Cruise> _cruises;
        private Cruise _selectedCruise;

        protected ISaleDataservice SaleDataservice { get; }
        protected ICruiseNavigationService NavigationService { get; }
        protected IDataserviceProvider DataserviceProvider { get; }
        protected IFileSystemService FileSystemService { get; }
        protected IFileDialogService FileDialogService { get; }
        protected IDialogService DialogService { get; }
        protected IDeviceInfoService DeviceInfo { get; }

        public ICommand OpenSelectedCruiseCommand => new Command(OpenSelectedCruise);
        public ICommand ShareSelectedCruiseCommand => new Command(ShareSelectedCruise);
        public ICommand ExportSelectedCruiseCommand => new Command(async () => await ExportSelectedCruise());
        public ICommand DeleteSelectedCruiseCommand => new Command(DeleteSelectedCruise);

        public Cruise SelectedCruise
        {
            get => _selectedCruise;
            set => SetProperty(ref _selectedCruise, value);
        }

        public Sale Sale
        {
            get => _sale;
            protected set => SetProperty(ref _sale, value);
        }

        public IEnumerable<Cruise> Cruises
        {
            get => _cruises;
            set => SetProperty(ref _cruises, value);
        }

        

        public CruiseSelectViewModel(IDataserviceProvider dataserviceProvider, ICruiseNavigationService navigationService, IFileSystemService fileSystemService, IDialogService dialogService, IFileDialogService fileDialogService, IDeviceInfoService deviceInfo)
        {
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            SaleDataservice = dataserviceProvider.GetDataservice<ISaleDataservice>();
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            DeviceInfo = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
        }

        public void OpenSelectedCruise()
        {
            var selectedCruise = SelectedCruise;
            if (selectedCruise == null) { return; }
            SelectCruise(selectedCruise);
        }

        public void SelectCruise(Cruise cruise)
        {
            var cruiseID = cruise.CruiseID;
            DataserviceProvider.CruiseID = cruiseID;
            NavigationService.ShowCruiseLandingLayout();
        }

        public Task ExportSelectedCruise()
        {
            var selectedCruise = SelectedCruise;
            if (selectedCruise == null) { return null; }
            return ExportCruise(selectedCruise);
        }

        public void ShareSelectedCruise()
        {
            var selectedCruise = SelectedCruise;
            if (selectedCruise == null) { return; }
            ShareCruise(selectedCruise);
        }

        public void ShareCruise(Cruise cruise)
        {
            var timestamp = DateTime.Now.ToString(EXPORT_TIMESTAMP_FORMAT);
            var deviceName = DeviceInfo.DeviceName;
            var defaultFileName = $"{cruise.SaleNumber}_{cruise.SaleName}_{cruise.PurposeShortCode.Replace(' ', '_')}_{timestamp}_{deviceName}.crz3";
            var exportTempDir = FileSystemService.ExportTempDir;
            var fileToExport = Path.Combine(exportTempDir, defaultFileName);

            var db = DataserviceProvider.Database;
            using (var destDb = new CruiseDatastore_V3(fileToExport, true))
            {
                var cruiseCopier = new CruiseCopier();
                cruiseCopier.Copy(db, destDb, cruise.CruiseID);
            }

            Share.RequestAsync(new ShareFileRequest
            {
                Title = defaultFileName,
                File = new ShareFile(fileToExport),
            });
        }

        public async Task ExportCruise(Cruise cruise)
        {
            var timestamp = DateTime.Now.ToString(EXPORT_TIMESTAMP_FORMAT);
            var deviceName = DeviceInfo.DeviceName;
            var defaultFileName = $"{cruise.SaleNumber}_{cruise.SaleName}_{cruise.PurposeShortCode.Replace(' ', '_')}_{timestamp}_{deviceName}.crz3";

            // create file to export before geting the destination path
            // on android requesting the desination file creates an empty file
            // if creating the file to export fails we don't want to create an empty file
            var exportTempDir = FileSystemService.ExportTempDir;
            var fileToExport = Path.Combine(exportTempDir, defaultFileName);

            var db = DataserviceProvider.Database;
            using (var destDb = new CruiseDatastore_V3(fileToExport, true))
            {
                var cruiseCopier = new CruiseCopier();
                cruiseCopier.Copy(db, destDb, cruise.CruiseID);
            }

            var destPath = await FileDialogService.SelectCruiseFileDestinationAsync(defaultFileName: defaultFileName);
            if (destPath != null)
            {
                FileSystemService.CopyTo(fileToExport, destPath);
                // todo need to delete the android content if creating file fails

                //File.Copy(fileToExport, destPath);
            }
        }

        public void DeleteSelectedCruise()
        {
            var selectedCruise = SelectedCruise;
            if (selectedCruise == null) { return; }
            DeleteSelectedCruise(selectedCruise);
        }

        public async void DeleteSelectedCruise(Cruise cruise)
        {
            if (await DialogService.AskYesNoAsync("Do you want to delete the cruise", "Warning", true) == true)
            {
                var cruiseID = cruise.CruiseID;
                SaleDataservice.DeleteCruise(cruiseID);
                Load();
                if (DataserviceProvider.CruiseID == cruiseID)
                {
                    DataserviceProvider.CruiseID = null;
                }
            }
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var saleNumber = parameters.GetValue<string>(NavParams.SaleNumber);
            var saleDataservice = SaleDataservice;
            var sale = saleDataservice.GetSaleBySaleNumber(saleNumber);
            Sale = sale;

            var cruises = saleDataservice.GetCruisesBySaleNumber(saleNumber);
            Cruises = cruises;
        }
    }
}