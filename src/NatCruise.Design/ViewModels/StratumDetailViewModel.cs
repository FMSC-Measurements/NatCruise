using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Design.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class StratumDetailViewModel : ValidationViewModelBase
    {
        public readonly string[] YealdComponent_Options = new string[] { "CL", "CD", "NL", "ND", };
        public readonly string[] Fixed_Size_Plot_Methods = new string[] { CruiseMethods.FIX, CruiseMethods.FCM, CruiseMethods.F3P, CruiseMethods.FIXCNT };

        private Stratum _stratum;
        private IEnumerable<CruiseMethod> _methods;
        private IEnumerable<TreeField> _treefieldOptions;

        public StratumDetailViewModel(IDataserviceProvider dataserviceProvider, ISetupInfoDataservice setupDataservice, StratumValidator validator)
            : base(validator)
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
                _stratum = value;
                if (value != null)
                {
                    var stratumCode = value.StratumCode;
                    CuttingUnits = StratumDataservice.GetCuttingUnitCodesByStratum(stratumCode);
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(StratumCode));
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(Method));
                RaisePropertyChanged(nameof(BasalAreaFactor));
                RaisePropertyChanged(nameof(FixedPlotSize));
                RaisePropertyChanged(nameof(KZ3PPNT));
                RaisePropertyChanged(nameof(SamplingFrequency));
                //RaisePropertyChanged(nameof(HotKey));
                //RaisePropertyChanged(nameof(FBSCode));
                RaisePropertyChanged(nameof(YieldComponent));
                RaisePropertyChanged(nameof(FixCNTField));

                NotifyCruiseMethodChanged();
            }
        }

        protected void NotifyCruiseMethodChanged()
        {
            RaisePropertyChanged(nameof(IsPlot));
            RaisePropertyChanged(nameof(IsFixedSizePlot));
            RaisePropertyChanged(nameof(IsVariableRariousePlot));
            RaisePropertyChanged(nameof(Is3PPNT));
            RaisePropertyChanged(nameof(IsFixCNT));
        }

        public string StratumCode
        {
            get => Stratum?.StratumCode;
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.StratumCode = x, st => StratumDataservice.UpdateStratum(st));
        }

        public string Description
        {
            get => Stratum?.Description;
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.Description = x, st => StratumDataservice.UpdateStratum(st));
        }

        public string Method
        {
            get => Stratum?.Method;
            set
            {
                SetPropertyAndValidate(Stratum, value, (st, x) => st.Method = x, st => StratumDataservice.UpdateStratum(st));
                NotifyCruiseMethodChanged();
            }
        }

        public double BasalAreaFactor
        {
            get => Stratum?.BasalAreaFactor ?? default(double);
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.BasalAreaFactor = x, st => StratumDataservice.UpdateStratum(st));
        }

        public double FixedPlotSize
        {
            get => Stratum?.FixedPlotSize ?? default(double);
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.FixedPlotSize = x, st => StratumDataservice.UpdateStratum(st));
        }

        public int KZ3PPNT
        {
            get => Stratum?.KZ3PPNT ?? default(int);
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.KZ3PPNT = x, st => StratumDataservice.UpdateStratum(st));
        }

        public int SamplingFrequency
        {
            get => Stratum?.SamplingFrequency ?? default(int);
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.SamplingFrequency = x, st => StratumDataservice.UpdateStratum(st));
        }

        //public string HotKey
        //{
        //    get => Stratum?.HotKey;
        //    set => SetPropertyAndValidate(Stratum, value, (st, x) => st.HotKey = x, st => StratumDataservice.UpdateStratum(st));
        //}

        //public string FBSCode
        //{
        //    get => Stratum?.FBSCode;
        //    set => SetPropertyAndValidate(Stratum, value, (st, x) => st.FBSCode = x, st => StratumDataservice.UpdateStratum(st));
        //}

        public string YieldComponent
        {
            get => Stratum?.YieldComponent;
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.YieldComponent = x, st => StratumDataservice.UpdateStratum(st));
        }

        public string FixCNTField
        {
            get => Stratum?.FixCNTField;
            set => SetPropertyAndValidate(Stratum, value, (st, x) => st.FixCNTField = x, st => StratumDataservice.UpdateStratum(st));
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

        public IEnumerable<string> YieldComponentOptions => YealdComponent_Options;

        public bool IsPlot => CruiseMethods.PLOT_METHODS.Contains(Stratum?.Method) || Stratum?.Method == CruiseMethods.FIXCNT;

        public bool IsVariableRariousePlot => CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(Stratum?.Method);

        public bool IsFixedSizePlot => Fixed_Size_Plot_Methods.Contains(Stratum?.Method);

        public bool Is3PPNT => Stratum?.Method == CruiseMethods.THREEPPNT;

        public bool IsFixCNT => Stratum?.Method == CruiseMethods.FIXCNT;
    }
}