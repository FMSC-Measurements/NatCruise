using CruiseDAL;
using CruiseDAL.UpConvert;
using CruiseDAL.V3.Sync;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Services;
using NatCruise.Wpf.Data;
using NatCruise.Wpf.Validation;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class NewCruiseViewModel : ValidationViewModelBase, IDialogAware
    {
        protected readonly TemplateCopier TEMPLATE_COPIER = new();

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
        protected IDataserviceProvider TemplateDataserviceProvider { get; set; }

        public string SaleName
        {
            get => _saleName;
            set => SetPropertyAndValidate(this, value, (m, v) => _saleName = v);
        }

        public string SaleNumber
        {
            get => _saleNumber;
            set => SetPropertyAndValidate(this, value, (m, v) => _saleNumber = v);
        }

        public string Region
        {
            get => _region;
            set
            {
                SetPropertyAndValidate(this, value, (m, v) => _region = v);
                OnPropertyChanged(nameof(ForestOptions));
            }
        }

        public string Forest
        {
            get => _forest;
            set
            {
                SetPropertyAndValidate(this, value, (m, v) => _forest = v);
                OnPropertyChanged(nameof(DistrictOptions));
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
            var defaultSaleFolder = $"{SaleNumber} {SaleName}";
            var filePath = await FileDialogService.SelectCruiseFileDestinationAsync(defaultFileName: defaultFileName, defaultSaleFolder: defaultSaleFolder);

            if (!string.IsNullOrEmpty(filePath))
            {
                var fileDirectory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                var fileInfo = new FileInfo(filePath);

                var extension = fileInfo.Extension.ToLower();
                if (extension == ".crz3")
                {
                    var purpose = Purpose;
                    var saleID = Guid.NewGuid().ToString();
                    var saleNumber = SaleNumber;
                    var cruiseNumber = (purpose.ShortCode.Equals("TS")) ? saleNumber : saleNumber + purpose.ShortCode;
                    var cruiseID = Guid.NewGuid().ToString();
                    var sale = new CruiseDAL.V3.Models.Sale()
                    {
                        SaleID = saleID,
                        SaleNumber = saleNumber,
                        Name = SaleName,
                        Region = Region,
                        Forest = Forest,
                        District = District,
                    };
                    var cruise = new CruiseDAL.V3.Models.Cruise()
                    {
                        CruiseID = cruiseID,
                        SaleID = saleID,
                        CruiseNumber = cruiseNumber,
                        SaleNumber = SaleNumber,
                        Purpose = purpose.PurposeCode,
                        UseCrossStrataPlotTreeNumbering = UseCrossStrataPlotTreeNumbering,
                        DefaultUOM = UOM,
                    };

                    var database = new CruiseDatastore_V3(fileInfo.FullName, true);
                    database.Insert(sale);
                    database.Insert(cruise);

                    DataserviceProvider.Database = database;
                    DataserviceProvider.CruiseID = cruiseID;

                    var templateDataserviceProvider = TemplateDataserviceProvider;
                    if (templateDataserviceProvider != null)
                    {
                        database.BeginTransaction();
                        try
                        {
                            CopyTemplateData(TemplateDataserviceProvider, DataserviceProvider);
                            database.CommitTransaction();
                        }
                        catch
                        {
                            database.RollbackTransaction();
                            throw;
                        }
                    }

                    RaiseRequestClose(new DialogResult(ButtonResult.OK));
                }
            }
        }

        protected void CopyTemplateData(IDataserviceProvider src, IDataserviceProvider dest)
        {
            var tmpltDs = src.Database;
            var tmpltCruiseID = src.CruiseID;
            var crzDs = dest.Database;
            var crzCruiseID = dest.CruiseID;
            TEMPLATE_COPIER.Copy(tmpltDs, crzDs, tmpltCruiseID, crzCruiseID);



            //var srcTemplateDS = src.GetDataservice<ITemplateDataservice>();
            //var destTemplateDS = dest.GetDataservice<ITemplateDataservice>();

            //// Species
            //var srcSpDS = src.GetDataservice<ISpeciesDataservice>();
            //var destSpDS = dest.GetDataservice<ISpeciesDataservice>();

            //var species = srcSpDS.GetSpecies();
            //foreach (var sp in species)
            //{
            //    destSpDS.AddSpecies(sp);

            //    var sp_prods = srcSpDS.GetSpeciesProducts(sp.SpeciesCode);
            //    foreach (var sp_prod in sp_prods)
            //    {
            //        destSpDS.AddSpeciesProduct(sp_prod);
            //    }
            //}

            //// Tree Audit Rules
            //var srcTarDs = src.GetDataservice<ITreeAuditRuleDataservice>();
            //var destTarDs = dest.GetDataservice<ITreeAuditRuleDataservice>();
            //var tars = srcTarDs.GetTreeAuditRules();
            //foreach (var tar in tars)
            //{
            //    destTarDs.AddTreeAuditRule(tar);
            //}

            //// Tree Audit Rule Selectors
            //var ruleSelectors = srcTarDs.GetRuleSelectors();
            //foreach (var ruleSelector in ruleSelectors)
            //{
            //    destTarDs.AddRuleSelector(ruleSelector);
            //}

            //// Tree Default Values
            //var tdvs = srcTemplateDS.GetTreeDefaultValues();
            //foreach (var tdv in tdvs)
            //{
            //    destTemplateDS.AddTreeDefaultValue(tdv);
            //}

            //// Stratum Templates
            //var srcStTemplateDS = src.GetDataservice<IStratumTemplateDataservice>();
            //var destStTemplateDS = dest.GetDataservice<IStratumTemplateDataservice>();

            //var stratumTemplates = srcStTemplateDS.GetStratumTemplates();

            //foreach (var st in stratumTemplates)
            //{
            //    destStTemplateDS.UpsertStratumTemplate(st);

            //    var treeFieldSetupDefaults = srcStTemplateDS.GetStratumTemplateTreeFieldSetups(st.StratumTemplateName);
            //    foreach (var tfsd in treeFieldSetupDefaults)
            //    {
            //        destStTemplateDS.UpsertStratumTemplateTreeFieldSetup(tfsd);
            //    }

            //    var logFieldSetupDefaults = srcStTemplateDS.GetStratumTemplateLogFieldSetups(st.StratumTemplateName);
            //    foreach (var lfsd in logFieldSetupDefaults)
            //    {
            //        destStTemplateDS.UpsertStratumTemplateLogFieldSetup(lfsd);
            //    }
            //}

            //// Tree Fields
            //var scrTreeFieldDS = src.GetDataservice<ITreeFieldDataservice>();
            //var destTreeFieldDS = dest.GetDataservice<ITreeFieldDataservice>();
            //var treeFields = scrTreeFieldDS.GetTreeFields();
            //foreach (var tf in treeFields
            //    .Where(x => string.IsNullOrEmpty(x.Heading) is false))
            //{
            //    destTreeFieldDS.UpdateTreeField(tf);
            //}

            //// Log Fields
            //var srcLogFieldDS = src.GetDataservice<ILogFieldDataservice>();
            //var destLogFieldDS = dest.GetDataservice<ILogFieldDataservice>();
            //var logFields = srcLogFieldDS.GetLogFields();
            //foreach (var lf in logFields
            //    .Where(x => string.IsNullOrEmpty(x.Heading) is false))
            //{
            //    destLogFieldDS.UpdateLogField(lf);
            //}

            //// Reports
            //var reports = srcTemplateDS.GetReports();
            //foreach (var rpt in reports)
            //{
            //    destTemplateDS.AddReport(rpt);
            //}

            //// Volume Equations
            //var volumeEquations = srcTemplateDS.GetVolumeEquations();
            //foreach (var ve in volumeEquations)
            //{
            //    destTemplateDS.AddVolumeEquation(ve);
            //}
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
            TemplateDataserviceProvider = new WpfDataserviceProvider(v3TemplateDb, DeviceInfo)
            {
                CruiseID = cruiseID,
            };

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