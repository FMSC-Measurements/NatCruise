using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Services;
using Prism.Commands;
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
        private bool _useCrossStrataPlotTreeNumbering = true;
        private string _templatePath;
        private ICommand _selectTemplateCommand;

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
        protected ITemplateDataservice TemplateDataservice { get; set; }

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

        public bool UseCrossStrataPlotTreeNumbering
        {
            get => _useCrossStrataPlotTreeNumbering;
            set => SetProperty(ref _useCrossStrataPlotTreeNumbering, value);
        }

        public string TemplatePath
        {
            get => _templatePath;
            set => SetProperty(ref _templatePath, value);
        }

        public string Title => "Create New Cruise";

        public event Action<IDialogResult> RequestClose;

        public ICommand CreateCruiseCommand => _createCruiseCommand ??= new DelegateCommand(CreateCruise);

        public ICommand SelectTemplateCommand => _selectTemplateCommand ??= new DelegateCommand(SelectTemplate);

        public ICommand CancelCommand => _cancelCommand ??= new DelegateCommand(Cancel);

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
                    var cruiseNumber = (purpose == "TS" || purpose == "Timber Sale") ? SaleNumber + "TS" : SaleNumber;
                    var cruiseID = Guid.NewGuid().ToString();
                    var cruise = new CruiseDAL.V3.Models.Cruise()
                    {
                        CruiseID = cruiseID,
                        SaleID = saleID,
                        CruiseNumber = cruiseNumber,
                        Purpose = Purpose,
                        UseCrossStrataPlotTreeNumbering = UseCrossStrataPlotTreeNumbering,
                        DefaultUOM = UOM,
                    };

                    var database = new CruiseDatastore_V3(fileInfo.FullName, true);
                    database.Insert(sale);
                    database.Insert(cruise);

                    var srcTemplateDataservice = TemplateDataservice;
                    if (srcTemplateDataservice != null)
                    {
                        var newCruiseTemplateDataservice = new TemplateDataservice(database, cruiseID, DeviceInfo.DeviceID);
                        database.BeginTransaction();
                        try
                        {
                            CopyTemplateData(TemplateDataservice, newCruiseTemplateDataservice);
                            database.CommitTransaction();
                        }
                        catch
                        {
                            database.RollbackTransaction();
                            throw;
                        }
                    }

                    DataserviceProvider.Database = database;
                    DataserviceProvider.CruiseID = cruiseID;
                    RaiseRequestClose(new DialogResult(ButtonResult.OK));
                }
            }
        }

        protected void CopyTemplateData(ITemplateDataservice src, ITemplateDataservice dest)
        {
            var species = src.GetSpecies();
            foreach (var sp in species)
            {
                dest.AddSpecies(sp);
            }

            var tars = src.GetTreeAuditRules();
            foreach (var tar in tars)
            {
                dest.AddTreeAuditRule(tar);
            }

            var ruleSelectors = src.GetRuleSelectors();
            foreach (var ruleSelector in ruleSelectors)
            {
                dest.AddRuleSelector(ruleSelector);
            }

            var tdvs = src.GetTreeDefaultValues();
            foreach (var tdv in tdvs)
            {
                dest.AddTreeDefaultValue(tdv);
            }

            var stratumDefaults = src.GetStratumDefaults();

            foreach (var sd in stratumDefaults)
            {
                dest.AddStratumDefault(sd);

                var treeFieldSetupDefaults = src.GetTreeFieldSetupDefaults(sd.StratumDefaultID);
                foreach (var tfsd in treeFieldSetupDefaults)
                {
                    dest.AddTreeFieldSetupDefault(tfsd);
                }

                var logFieldSetupDefaults = src.GetLogFieldSetupDefaults(sd.StratumDefaultID);
                foreach (var lfsd in logFieldSetupDefaults)
                {
                    dest.AddLogFieldSetupDefault(lfsd);
                }
            }

            var reports = src.GetReports();
            foreach (var rpt in reports)
            {
                dest.AddReport(rpt);
            }

            var volumeEquations = src.GetVolumeEquations();
            foreach (var ve in volumeEquations)
            {
                dest.AddVolumeEquation(ve);
            }
        }

        public void SelectTemplate()
        {
            var templatePath = FileDialogService.SelectTemplateFileAsync().Result;

            if (templatePath == null || File.Exists(templatePath) == false) { return; }

            var v3TemplateDb = new CruiseDatastore_V3();
            Migrator.MigrateFromV2ToV3(templatePath, v3TemplateDb, DeviceInfo.DeviceID);

            var cruiseID = v3TemplateDb.ExecuteScalar<string>("SELECT CruiseID FROM Cruise LIMIT 1;");

            var templateDataservice = new TemplateDataservice(v3TemplateDb, cruiseID, DeviceInfo.DeviceID);
            TemplateDataservice = templateDataservice;
            TemplatePath = templatePath;
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