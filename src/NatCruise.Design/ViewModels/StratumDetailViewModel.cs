using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class StratumDetailViewModel : ViewModelBase
    {
        private Stratum _stratum;
        private IEnumerable<CruiseMethod> _methods;
        private IEnumerable<TreeField> _treefieldOptions;

        public StratumDetailViewModel(IDataserviceProvider dataserviceProvider, ISetupInfoDataservice setupDataservice)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }

            var stratumDataservice = dataserviceProvider.GetDataservice<IStratumDataservice>();
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            TemplateDataservice = dataserviceProvider.GetDataservice<ITemplateDataservice>() ?? throw new ArgumentNullException(nameof(TemplateDataservice));

            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));

            Methods = SetupDataservice.GetCruiseMethods();
            TreeFieldOptions = TemplateDataservice.GetTreeFields();

            //HotKeyOptions = new string[]
            //{
            //    "A", "B", "C", "D", "E", "F", "G", "H",
            //    "I", "J", "K", "L", "M", "N", "O", "P",
            //    "Q", "R", "S", "T", "U", "V", "X", "Y", "Z",
            //    "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            //};
        }

        public ITemplateDataservice TemplateDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }
        protected IStratumDataservice StratumDataservice { get; }

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                if (_stratum != null) { _stratum.PropertyChanged -= Stratum_PropertyChanged; }
                _stratum = value;
                OnStratumChanged(value);
                RaisePropertyChanged();
            }
        }

        private void OnStratumChanged(Stratum stratum)
        {
            if (stratum == null) { return; }

            var stratumCode = Stratum.StratumCode;
            CuttingUnits = StratumDataservice.GetCuttingUnitCodesByStratum(stratumCode);
            stratum.PropertyChanged += Stratum_PropertyChanged;
        }

        public IEnumerable<string> CuttingUnits { get; set; }

        public IEnumerable<CruiseMethod> Methods
        {
            get => _methods;
            protected set => SetProperty(ref _methods, value);
        }

        public IEnumerable<TreeField> TreeFieldOptions
        {
            get => _treefieldOptions;
            protected set => SetProperty(ref _treefieldOptions, value);
        }

        public bool IsPlot => CruiseMethods.PLOT_METHODS.Contains(Stratum?.Method);

        public bool IsVariableRariousePlot => CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(Stratum?.Method);

        public bool IsFixedSizePlot
        {
            get
            {
                var method = Stratum?.Method;
                return method == CruiseMethods.FIX || method == CruiseMethods.FCM || method == CruiseMethods.F3P;
            }
        }

        public bool Is3PPNT => Stratum?.Method == CruiseMethods.THREEPPNT;

        public bool IsFixCNT => Stratum?.Method == CruiseMethods.FIXCNT;

        private void Stratum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var stratum = Stratum;

            if (stratum == null) { return; }
            StratumDataservice.UpdateStratum(stratum);

            if (e.PropertyName == nameof(Stratum.Method))
            {
                RaisePropertyChanged(nameof(IsPlot));
                RaisePropertyChanged(nameof(IsFixedSizePlot));
                RaisePropertyChanged(nameof(IsVariableRariousePlot));
                RaisePropertyChanged(nameof(Is3PPNT));
                RaisePropertyChanged(nameof(IsFixCNT));
            }
        }
    }
}