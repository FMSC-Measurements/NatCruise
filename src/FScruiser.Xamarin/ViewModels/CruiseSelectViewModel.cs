using CruiseDAL;
using CruiseDAL.V3.Sync;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using NatCruise.Services;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class CruiseSelectViewModel : XamarinViewModelBase
    {
        //public ICommand SelectCruiseCommand => new

        private Sale _sale;
        private IEnumerable<Cruise> _cruises;
        private Cruise _selectedCruise;

        protected ISaleDataservice SaleDataservice { get; }
        protected ICruiseNavigationService NavigationService { get; }
        protected IDataserviceProvider DataserviceProvider { get; }
        protected IFileSystemService FileSystemService { get; }
        protected IDialogService DialogService { get; }

        public ICommand OpenSelectedCruiseCommand => new Command(OpenSelectedCruise);
        public ICommand ExportSelectedCruiseCommand => new Command(ExportSelectedCruise);
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

        public CruiseSelectViewModel(IDataserviceProvider dataserviceProvider, ICruiseNavigationService navigationService, IFileSystemService fileSystemService, IDialogService dialogService)
        {
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            SaleDataservice = dataserviceProvider.GetDataservice<ISaleDataservice>();
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
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

        public void ExportSelectedCruise()
        {
            var selectedCruise = SelectedCruise;
            if (selectedCruise == null) { return; }
            ExportCruise(selectedCruise);
        }

        public void ExportCruise(Cruise cruise)
        {
            var timestamp = DateTime.Today.ToString("ddMMyyyy");
            var defaultFileName = $"{cruise.SaleNumber}_{cruise.SaleName}_{cruise.Purpose.Replace(' ', '_')}_{timestamp}.crz3";
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
            var saleID = parameters.GetValue<string>(NavParams.SaleID);
            var saleDataservice = SaleDataservice;
            var sale = saleDataservice.GetSale(saleID);
            Sale = sale;

            var cruises = saleDataservice.GetCruises(saleID);
            Cruises = cruises;
        }
    }
}