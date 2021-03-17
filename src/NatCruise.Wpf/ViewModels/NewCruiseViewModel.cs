using CruiseDAL;
using CruiseDAL.V3.Models;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Services;
using NatCruise.Wpf.Services;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class NewCruiseViewModel : BindableBase, IDialogAware
    {
        private ICommand _createCruiseCommand;
        private ICommand _cancelCommand;
        private string _saleName;
        private string _saleNumber;
        private string _region;
        private string _forest;
        private string _district;
        private string _purpose;
        private string _uom;

        public NewCruiseViewModel(IDataserviceProvider dataserviceProvider, ISetupInfoDataservice setupInfo, IFileDialogService fileDialogService, IDeviceInfoService deviceInfo)
        {
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            SetupinfoDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            DeviceInfo = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
        }

        protected IDataserviceProvider DataserviceProvider { get; }
        protected ISetupInfoDataservice SetupinfoDataservice { get; }
        protected IFileDialogService FileDialogService { get; }
        protected IDeviceInfoService DeviceInfo { get; set; }

        public string SaleName
        {
            get => _saleName;
            set => SetProperty(ref _saleName, value);
        }

        public string SaleNumber
        {
            get => _saleNumber;
            set => SetProperty(ref _saleNumber, value);
        }

        public string Region
        {
            get => _region;
            set
            {
                SetProperty(ref _region, value);
                RaisePropertyChanged(nameof(ForestOptions));
            }
        }

        public string Forest
        {
            get => _forest;
            set
            {
                SetProperty(ref _forest, value);
                RaisePropertyChanged(nameof(DistrictOptions));
            }
        }



        public string District
        {
            get => _district;
            set => SetProperty(ref _district, value);
        }

        public string Purpose
        {
            get => _purpose;
            set => SetProperty(ref _purpose, value);
        }

        public string UOM
        {
            get => _uom;
            set => SetProperty(ref _uom, value);
        }

        public string Title => "Create New Cruise";

        public event Action<IDialogResult> RequestClose;

        public ICommand CreateCruiseCommand => _createCruiseCommand ?? (_createCruiseCommand = new DelegateCommand(CreateCruise));

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));

        public IEnumerable<Purpose> PurposeOptions => SetupinfoDataservice.GetPurposes();

        public IEnumerable<Region> RegionOptions => SetupinfoDataservice.GetRegions();

        public IEnumerable<Forest> ForestOptions => SetupinfoDataservice.GetForests(Region);

        public IEnumerable<District> DistrictOptions
        {
            get
            {
                var stuff = SetupinfoDataservice.GetDistricts(Region, Forest);
                return stuff;
            }
        }

        public IEnumerable<UOM> UOMOptions => SetupinfoDataservice.GetUOMCodes();

        public void Cancel()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        private async void CreateCruise()
        {
            var isSaleValid = ValidateSale();
            if (isSaleValid == false) { return; }

            var defaultFileName = $"{SaleNumber} {SaleName} {Purpose}.crz3";

            var filePath = await FileDialogService.SelectCruiseFileDestinationAsync(defaultFileName: defaultFileName);
            if (filePath != null)
            {
                var fileInfo = new FileInfo(filePath);

                var extension = fileInfo.Extension.ToLower();
                if (extension == ".crz3")
                {
                    var saleID = Guid.NewGuid().ToString();
                    var sale = new CruiseDAL.V3.Models.Sale()
                    {
                        SaleID = saleID,
                        SaleNumber = SaleNumber,
                        Name = SaleName,
                        Region = Region,
                        Forest = Forest,
                        District = District,
                    };

                    var purpose = Purpose;
                    var cruiseNumber = (purpose == "TS") ? SaleNumber : SaleNumber + Purpose;
                    var cruiseID = Guid.NewGuid().ToString();
                    var cruise = new CruiseDAL.V3.Models.Cruise()
                    {
                        CruiseID = cruiseID,
                        SaleID = saleID,
                        CruiseNumber = cruiseNumber,
                        Purpose = Purpose,
                    };

                    var database = new CruiseDatastore_V3(fileInfo.FullName, true);
                    database.Insert(sale);
                    database.Insert(cruise);

                    DataserviceProvider.Database = database;

                    RaiseRequestClose(new DialogResult(ButtonResult.OK));
                }
            }
        }

        public void SelectTemplate()
        {
        }

        protected void LoadTemplate(string templatePath)
        {
        }

        private bool ValidateSale()
        {
            if (string.IsNullOrWhiteSpace(SaleName)) { return false; }
            if (string.IsNullOrWhiteSpace(SaleNumber)) { return false; }

            return true;
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}