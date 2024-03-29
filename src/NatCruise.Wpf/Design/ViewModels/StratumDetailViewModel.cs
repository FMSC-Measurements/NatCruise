﻿using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Services;
using NatCruise.Wpf.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class StratumDetailViewModel : ValidationViewModelBase
    {
        public readonly string[] YieldComponent_Options = new string[] { "CL", "CD", "NL", "ND", };
        public readonly string[] Fixed_Size_Plot_Methods = new string[] { CruiseMethods.FIX, CruiseMethods.FCM, CruiseMethods.F3P, CruiseMethods.FIXCNT };

        private Stratum _stratum;
        private IEnumerable<CruiseMethod> _methods;
        private IEnumerable<TreeField> _treefieldOptions;
        private IApplicationSettingService _appSettings;
        private bool _isSuperuserModeEnabled;
        private bool _isLocked;

        public StratumDetailViewModel(IStratumDataservice stratumDataservice,
            ITreeFieldDataservice treeFieldDataservice,
            ISetupInfoDataservice setupDataservice,
            ISaleDataservice saleDataservice,
            ICuttingUnitDataservice cuttingUnitDataservice,
            IApplicationSettingService applicationSettingService,
            StratumValidator validator)
            : base(validator)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));

            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
            AppSettings = applicationSettingService ?? throw new ArgumentNullException(nameof(applicationSettingService));

            var cruise = saleDataservice.GetCruise();
            if (cruise.Purpose == "Recon")
            {
                Methods = new[]
                {
                    new CruiseMethod { Method = "FIX", FriendlyName = "Fixed Plot", IsPlotMethod = true },
                    new CruiseMethod { Method = "PNT", FriendlyName = "Point (Variable Plot)", IsPlotMethod= true },
                    new CruiseMethod { Method = "FIXCNT", FriendlyName = "Fixed Count", IsPlotMethod= true },
                };
            }
            else
            {
                Methods = SetupDataservice.GetCruiseMethods();
            }

            TreeFieldOptions = TreeFieldDataservice.GetTreeFields();

            //HotKeyOptions = new string[]
            //{
            //    "A", "B", "C", "D", "E", "F", "G", "H",
            //    "I", "J", "K", "L", "M", "N", "O", "P",
            //    "Q", "R", "S", "T", "U", "V", "X", "Y", "Z",
            //    "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            //};
        }

        public ITreeFieldDataservice TreeFieldDataservice { get; }
        public ISaleDataservice SaleDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }
        public IApplicationSettingService AppSettings
        {
            get => _appSettings;
            private set
            {
                if(_appSettings != null) { _appSettings.PropertyChanged -= AppSettings_PropertyChanged; }
                _appSettings = value;
                if(value != null)
                {
                    IsSuperuserModeEnabled = value.IsSuperuserMode;
                    value.PropertyChanged += AppSettings_PropertyChanged;
                }
                OnPropertyChanged(nameof(AppSettings));
            }
        }

        private void AppSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {   
            if(e.PropertyName == nameof(IApplicationSettingService.IsSuperuserMode))
            {
                var appSettings = (IApplicationSettingService)sender;
                IsSuperuserModeEnabled = appSettings.IsSuperuserMode;
            }
        }

        public bool IsSuperuserModeEnabled
        {
            get => _isSuperuserModeEnabled;
            set
            {
                SetProperty(ref _isSuperuserModeEnabled, value);
                IsLocked = (Stratum?.HasTrees ?? false) && !value;
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

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
                    CuttingUnits = CuttingUnitDataservice.GetCuttingUnitCodesByStratum(stratumCode);
                }
                ValidateAll(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(StratumCode));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(Method));
                OnPropertyChanged(nameof(BasalAreaFactor));
                OnPropertyChanged(nameof(FixedPlotSize));
                OnPropertyChanged(nameof(KZ3PPNT));
                OnPropertyChanged(nameof(SamplingFrequency));
                //RaisePropertyChanged(nameof(HotKey));
                //RaisePropertyChanged(nameof(FBSCode));
                OnPropertyChanged(nameof(YieldComponent));
                OnPropertyChanged(nameof(FixCNTField));

                IsLocked = (value?.HasTrees ?? false) && !IsSuperuserModeEnabled;

                NotifyCruiseMethodChanged();
            }
        }

        protected void NotifyCruiseMethodChanged()
        {
            OnPropertyChanged(nameof(IsPlot));
            OnPropertyChanged(nameof(IsFixedSizePlot));
            OnPropertyChanged(nameof(IsVariableRadiusPlot));
            OnPropertyChanged(nameof(Is3PPNT));
            OnPropertyChanged(nameof(IsFixCNT));
        }

        public string StratumCode
        {
            get => Stratum?.StratumCode;
            set
            {
                var origValue = Stratum?.StratumCode;
                SetPropertyAndValidate(Stratum, value, (st, x) => st.StratumCode = x);

                try
                {
                    StratumDataservice.UpdateStratumCode(Stratum);
                }
                catch (FMSC.ORM.UniqueConstraintException)
                {
                    Stratum.StratumCode = origValue;
                    OnPropertyChanged(nameof(StratumCode));
                    //DialogService.ShowNotification("Stratum Code Already Exists");
                }
            }
        }

        public string Description
        {
            get => Stratum?.Description;
            set
            {
                SetPropertyAndValidate(Stratum, value, (st, x) => st.Description = x);
                StratumDataservice.UpdateStratum(Stratum);
            }
        }

        public string Method
        {
            get => Stratum?.Method;
            set
            {
                var stratum = Stratum;
                if (stratum.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(stratum, value, (st, x) => st.Method = x);
                StratumDataservice.UpdateStratum(stratum);
                NotifyCruiseMethodChanged();
            }
        }

        public double BasalAreaFactor
        {
            get => Stratum?.BasalAreaFactor ?? default(double);
            set
            {
                var stratum = Stratum;
                if (stratum.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(stratum, value, (st, x) => st.BasalAreaFactor = x);
                StratumDataservice.UpdateStratum(stratum);
            }
        }

        public double FixedPlotSize
        {
            get => Stratum?.FixedPlotSize ?? default(double);
            set
            {
                var stratum = Stratum;
                if (stratum.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(stratum, value, (st, x) => st.FixedPlotSize = x);
                StratumDataservice.UpdateStratum(stratum);
            }
        }

        public int KZ3PPNT
        {
            get => Stratum?.KZ3PPNT ?? default(int);
            set
            {
                var stratum = Stratum;
                if (stratum.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(stratum, value, (st, x) => st.KZ3PPNT = x);
                StratumDataservice.UpdateStratum(stratum);
            }
        }

        public int SamplingFrequency
        {
            get => Stratum?.SamplingFrequency ?? default(int);
            set
            {
                var stratum = Stratum;
                if (stratum.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(Stratum, value, (st, x) => st.SamplingFrequency = x);
                StratumDataservice.UpdateStratum(Stratum);
            }
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
            set
            {
                SetPropertyAndValidate(Stratum, value, (st, x) => st.YieldComponent = x);
                StratumDataservice.UpdateStratum(Stratum);
            }
        }

        public string FixCNTField
        {
            get => Stratum?.FixCNTField;
            set
            {
                SetPropertyAndValidate(Stratum, value, (st, x) => st.FixCNTField = x);
                StratumDataservice.UpdateStratum(Stratum);
            }
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

        public IEnumerable<string> YieldComponentOptions => YieldComponent_Options;

        public bool IsPlot => CruiseMethods.PLOT_METHODS.Contains(Stratum?.Method) || Stratum?.Method == CruiseMethods.FIXCNT || Stratum?.Method == CruiseMethods.THREEPPNT;

        public bool IsVariableRadiusPlot => CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(Stratum?.Method);

        public bool IsFixedSizePlot => Fixed_Size_Plot_Methods.Contains(Stratum?.Method);

        public bool Is3PPNT => Stratum?.Method == CruiseMethods.THREEPPNT;

        public bool IsFixCNT => Stratum?.Method == CruiseMethods.FIXCNT;
    }
}