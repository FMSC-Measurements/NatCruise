using CruiseDAL;
using NatCruise.Wpf.Data;
using NatCruise.Wpf.Models;
using NatCruise.Wpf.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class NewCruisePageViewModel : BindableBase, IDialogAware
    {
        private ICommand _createCruiseCommand;
        private ICommand _cancelCommand;

        public NewCruisePageViewModel(IDataserviceProvider dataserviceProvider, IFileDialogService fileDialogService)
        {
            DataserviceProvider = dataserviceProvider;
            SetupinfoDataservice = dataserviceProvider.GetDataservice<ISetupInfoDataservice>();
            FileDialogService = fileDialogService;

            Sale.PropertyChanged += Sale_PropertyChanged;
        }

        private void Sale_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;
            if (propName == nameof(Sale.Region))
            {
                RaisePropertyChanged(nameof(ForestOptions));
            }
        }

        public IDataserviceProvider DataserviceProvider { get; }
        public ISetupInfoDataservice SetupinfoDataservice { get; }
        public IFileDialogService FileDialogService { get; }
        public Sale Sale { get; set; } = new Sale();

        public string Title => "Create New Cruise";

        public event Action<IDialogResult> RequestClose;

        public ICommand CreateCruiseCommand => _createCruiseCommand ?? (_createCruiseCommand = new DelegateCommand(CreateCruise));

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));

        public IEnumerable<Purpose> PurposeOptions => SetupinfoDataservice.GetPurposes();

        public IEnumerable<Region> RegionOptions => SetupinfoDataservice.GetRegions();

        public IEnumerable<Forest> ForestOptions
        {
            get => SetupinfoDataservice.GetForests(Sale.Region);
        }

        public void Cancel()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        private void CreateCruise()
        {
            var isSaleValid = ValidateSale();
            if(isSaleValid == false) { return; }

            var defaultFileName = $"{Sale.SaleNumber} {Sale.Name}.crz3";

            var filePath = FileDialogService.SelectCruiseFileDestination(defaultFileName: defaultFileName);
            if (filePath != null)
            {
                var fileInfo = new FileInfo(filePath);

                var extension = fileInfo.Extension.ToLower();
                if (extension == ".crz3")
                {
                    using (var database = new CruiseDatastore_V3(fileInfo.FullName, true))
                    {
                        database.Insert(Sale);
                    }

                    DataserviceProvider.OpenFile(fileInfo.FullName);

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

        bool ValidateSale()
        {
            var sale = Sale;
            if(string.IsNullOrWhiteSpace(sale.Name)) { return false; }
            if(string.IsNullOrWhiteSpace(sale.SaleNumber)) { return false; }

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