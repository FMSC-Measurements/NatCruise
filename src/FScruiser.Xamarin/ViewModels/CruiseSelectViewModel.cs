using CruiseDAL;
using CruiseDAL.V3.Sync;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using NatCruise.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class CruiseSelectViewModel : ViewModelBase
    {
        //public ICommand SelectCruiseCommand => new

        private Sale _sale;
        private IEnumerable<Cruise> _cruises;
        private Cruise _selectedCruise;

        protected ISaleDataservice SaleDataservice { get; }
        protected ICruiseNavigationService NavigationService { get; }
        protected IDataserviceProvider DataserviceProvider { get; }
        protected IFileSystemService FileSystemService { get; }

        public ICommand OpenSelectedCruiseCommand => new Command(OpenSelectedCruise);
        public ICommand ExportSelectedCruiseCommand => new Command(ExportSelectedCruise);
        //public ICommand SelectCruiseCommand => new Command<Cruise>((cruise) => SelectCruise(cruise));

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

        public CruiseSelectViewModel(IDataserviceProvider dataserviceProvider, ICruiseNavigationService navigationService, IFileSystemService fileSystemService)
        {
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            SaleDataservice = dataserviceProvider.GetDataservice<ISaleDataservice>();
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            
        }

        public void OpenSelectedCruise()
        {
            var selectedCruise = SelectedCruise;
            if(selectedCruise == null) { return; }
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

            using (var db = DataserviceProvider.GetDatabase())
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



        protected override void Refresh(INavigationParameters parameters)
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