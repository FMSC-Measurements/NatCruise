using CruiseDAL;
using CruiseDAL.UpConvert;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Util;
using NatCruise.Design.Validation;
using NatCruise.Services;
using NatCruise.Wpf.Validation;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using NatCruise.Models;
using NatCruise.MVVM;

namespace NatCruise.Wpf.ViewModels
{
    public class NewCruiseViewModel : ValidationViewModelBase, IDialogAware
    {
        private ICommand _createCruiseCommand;
        private ICommand _cancelCommand;
        private string _saleName;
        private string _saleNumber;
        private string _region;
        private string _forest;
        private string _district;
        private Purpose _purpose;
        private string _uom;
        private bool _useCrossStrataPlotTreeNumbering = true;
        private string _templatePath;
        private ICommand _selectTemplateCommand;
        private IEnumerable<Purpose> _purposeOptions;
        private IEnumerable<Region> _regionOptions;
        private IEnumerable<UOM> _uomOptions;

        public NewCruiseViewModel(IDataserviceProvider dataserviceProvider, ISetupInfoDataservice setupInfo, IFileDialogService fileDialogService, IDeviceInfoService deviceInfo)
            : base(new NewCruiseValidator())
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
            set => SetPropertyAndValidate(this, value, (m, v) => _saleName = v);
        }

        public string SaleNumber
        {
            get => _saleNumber;
            set => SetPropertyAndValidate(this, value, (m,v) => _saleNumber = v);
        }

        public string Region
        {
            get => _region;
            set
            {
                SetPropertyAndValidate(this, value, (m, v) => _region = v);
                RaisePropertyChanged(nameof(ForestOptions));
            }
        }

        public string Forest
        {
            get => _forest;
            set
            {
                SetPropertyAndValidate(this, value, (m, v) => _forest = v);
                RaisePropertyChanged(nameof(DistrictOptions));
            }
        }

        public string District
        {
            get => _district;
            set => SetPropertyAndValidate(this, value, (m, v) => _district = v);
        }

        public Purpose Purpose
        {
            get => _purpose;
            set => SetPropertyAndValidate(this, value, (m, v) => _purpose = v);
        }

        public string UOM
        {
            get => _uom;
            set => SetPropertyAndValidate(this, value, (m, v) => _uom = v);
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

        public ICommand CreateCruiseCommand => _createCruiseCommand ??= new DelegateCommand(() => CreateCruise().FireAndForget());
        public ICommand SelectTemplateCommand => _selectTemplateCommand ??= new DelegateCommand(SelectTemplate);
        public ICommand CancelCommand => _cancelCommand ??= new DelegateCommand(Cancel);

        public IEnumerable<Purpose> PurposeOptions => _purposeOptions ??= SetupinfoDataservice.GetPurposes();
        public IEnumerable<Region> RegionOptions => _regionOptions ??= SetupinfoDataservice.GetRegions();
        public IEnumerable<Forest> ForestOptions => SetupinfoDataservice.GetForests(Region);
        public IEnumerable<District> DistrictOptions => SetupinfoDataservice.GetDistricts(Region, Forest);
        public IEnumerable<UOM> UOMOptions => _uomOptions ??= SetupinfoDataservice.GetUOMCodes();

        public void Cancel()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        public async Task CreateCruise()
        {
            ValidateAll(this);
            if (HasErrors == true) { return; }

            var defaultFileName = $"{SaleNumber} {SaleName} {Purpose}.crz3";

            var filePath = await FileDialogService.SelectCruiseFileDestinationAsync(defaultFileName: defaultFileName);
            if (filePath != null)
            {
                var fileInfo = new FileInfo(filePath);

                var extension = fileInfo.Extension.ToLower();
                if (extension == ".crz3")
                {
                    var saleID = Guid.NewGuid().ToString();
                    var saleNumber = SaleNumber;
                    var sale = new CruiseDAL.V3.Models.Sale()
                    {
                        SaleID = saleID,
                        SaleNumber = saleNumber,
                        Name = SaleName,
                        Region = Region,
                        Forest = Forest,
                        District = District,
                    };

                    var purpose = Purpose;
                    var cruiseNumber = (purpose.ShortCode.Equals("TS")) ? SaleNumber : SaleNumber + purpose.ShortCode;
                    var cruiseID = Guid.NewGuid().ToString();
                    var cruise = new CruiseDAL.V3.Models.Cruise()
                    {
                        CruiseID = cruiseID,
                        SaleID = saleID,
                        CruiseNumber = cruiseNumber,
                        SaleNumber = saleNumber,
                        Purpose = purpose.PurposeCode,
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

            var stratumTemplate = src.GetStratumTemplates();

            foreach (var st in stratumTemplate)
            {
                dest.UpsertStratumTemplate(st);

                var treeFieldSetupDefaults = src.GetStratumTemplateTreeFieldSetups(st.StratumTemplateName);
                foreach (var tfsd in treeFieldSetupDefaults)
                {
                    dest.UpsertStratumTemplateTreeFieldSetup(tfsd);
                }

                var logFieldSetupDefaults = src.GetStratumTemplateLogFieldSetups(st.StratumTemplateName);
                foreach (var lfsd in logFieldSetupDefaults)
                {
                    dest.UpsertStratumTemplateLogFieldSetup(lfsd);
                }
            }

            var treeFields = src.GetTreeFields();
            foreach (var tf in treeFields)
            {
                if (string.IsNullOrEmpty(tf.Heading) is false)
                {
                    dest.UpdateTreeField(tf);
                }
            }

            var logFields = src.GetLogFields();
            foreach (var lf in logFields)
            {
                if (string.IsNullOrEmpty(lf.Heading) is false)
                {
                    dest.UpdateLogField(lf);
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

            SelectTemplate(templatePath);
        }

        public void SelectTemplate(string templatePath)
        {
            if (templatePath == null || File.Exists(templatePath) == false) { return; }

            var extention = Path.GetExtension(templatePath);

            CruiseDatastore_V3 v3TemplateDb = null;
            if (extention is ".cut")
            {
                v3TemplateDb = new CruiseDatastore_V3();
                new Migrator().MigrateFromV2ToV3(templatePath, v3TemplateDb, DeviceInfo.DeviceID);
            }
            else if (extention is ".crz3t")
            {
                v3TemplateDb = new CruiseDatastore_V3(templatePath);
            }
            else return;

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