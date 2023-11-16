using CruiseDAL.Schema;
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
    public partial class SampleGroupDetailViewModel : ValidationViewModelBase
    {
        protected readonly string[] SampleSelector_Type_Options = new[] { CruiseMethods.BLOCK_SAMPLER_TYPE, CruiseMethods.SYSTEMATIC_SAMPLER_TYPE, CruiseMethods.CLICKER_SAMPLER_TYPE };
        protected readonly string[] Supports_SamplerType_Selection = new[] { CruiseMethods.STR, };

        private SampleGroup _sampleGroup;
        private IEnumerable<Product> _productOptions;
        private IApplicationSettingService _appSettings;
        private bool _isSuperuserModeEnabled;
        private bool _isLocked;

        public SampleGroupDetailViewModel(ISampleGroupDataservice sampleGroupDataservice,
            ISetupInfoDataservice setupInfo,
            IApplicationSettingService applicationSettingService,
            SampleGroupValidator validator)
            : base(validator)
        {
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SetupDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));

            AppSettings = applicationSettingService ?? throw new ArgumentNullException(nameof(applicationSettingService));

            ProductOptions = SetupDataservice.GetProducts();
        }

        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }
        public IApplicationSettingService AppSettings
        {
            get => _appSettings;
            private set
            {
                if (_appSettings != null) { _appSettings.PropertyChanged -= AppSettings_PropertyChanged; }
                _appSettings = value;
                if (value != null)
                {
                    IsSuperuserModeEnabled = value.IsSuperuserMode;
                    _appSettings.PropertyChanged += AppSettings_PropertyChanged;
                }
                OnPropertyChanged(nameof(AppSettings));
            }
        }

        private void AppSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IApplicationSettingService.IsSuperuserMode))
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
                IsLocked = (SampleGroup?.HasTrees ?? false) && !value;
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        public IEnumerable<string> SampleSelectorTypeOptions => SampleSelector_Type_Options;

        public IEnumerable<Product> ProductOptions
        {
            get => _productOptions;
            protected set => SetProperty(ref _productOptions, value);
        }

        public SampleGroup SampleGroup
        {
            get => _sampleGroup;
            set
            {
                //OnSampleGroupChanging(_sampleGroup);
                SetProperty(ref _sampleGroup, value);
                OnSampleGroupChanged(value);
            }
        }

        private void OnSampleGroupChanged(SampleGroup newValue)
        {
            ValidateAll(newValue);

            OnPropertyChanged(nameof(SampleGroupCode));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(CutLeave));
            OnPropertyChanged(nameof(UOM));
            OnPropertyChanged(nameof(PrimaryProduct));
            OnPropertyChanged(nameof(SecondaryProduct));
            OnPropertyChanged(nameof(BiomassProduct));
            OnPropertyChanged(nameof(DefaultLiveDead));
            OnPropertyChanged(nameof(SamplingFrequency));
            OnPropertyChanged(nameof(InsuranceFrequency));
            OnPropertyChanged(nameof(KZ));
            OnPropertyChanged(nameof(BigBAF));
            OnPropertyChanged(nameof(TallyBySubPop));
            OnPropertyChanged(nameof(UseExternalSampler));
            OnPropertyChanged(nameof(MinKPI));
            OnPropertyChanged(nameof(MaxKPI));
            OnPropertyChanged(nameof(SmallFPS));
            OnPropertyChanged(nameof(SampleSelectorType));

            OnPropertyChanged(nameof(CruiseMethod));
            OnPropertyChanged(nameof(DefaultSampleSelectorType));

            IsLocked = (newValue?.HasTrees ?? false) && !IsSuperuserModeEnabled;
        }

        //private void SampleGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    var sampleGroup = (SampleGroup)sender;
        //    SampleGroupDataservice.UpdateSampleGroup(sampleGroup);
        //}

        public string SampleGroupCode
        {
            get => SampleGroup?.SampleGroupCode;
            set
            {
                var origValue = SampleGroup?.SampleGroupCode;

                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SampleGroupCode = x);

                try
                {
                    SampleGroupDataservice.UpdateSampleGroupCode(SampleGroup);
                }
                catch (FMSC.ORM.UniqueConstraintException)
                {
                    SampleGroup.SampleGroupCode = origValue;
                    OnPropertyChanged(nameof(SampleGroupCode));
                    //DialogService.ShowNotification("Unit Code Already Exists");
                }
            }
        }

        public string Description
        {
            get => SampleGroup?.Description;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.Description = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string CutLeave
        {
            get => SampleGroup?.CutLeave;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.CutLeave = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string UOM
        {
            get => SampleGroup?.UOM;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.UOM = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string PrimaryProduct
        {
            get => SampleGroup?.PrimaryProduct;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.PrimaryProduct = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string SecondaryProduct
        {
            get => SampleGroup?.SecondaryProduct;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SecondaryProduct = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string BiomassProduct
        {
            get => SampleGroup?.BiomassProduct;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.BiomassProduct = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string DefaultLiveDead
        {
            get => SampleGroup?.DefaultLiveDead;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.DefaultLiveDead = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int SamplingFrequency
        {
            get => SampleGroup?.SamplingFrequency ?? default(int);
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(SamplingFrequency)); }

                var sg = SampleGroup;
                if (sg.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SamplingFrequency = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int InsuranceFrequency
        {
            get => SampleGroup?.InsuranceFrequency ?? default(int);
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(InsuranceFrequency)); }

                var sg = SampleGroup;
                if(sg.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.InsuranceFrequency = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int KZ
        {
            get => SampleGroup?.KZ ?? default(int);
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(KZ)); }

                var sg = SampleGroup;
                if (sg.HasTrees && !IsSuperuserModeEnabled)
                {
                    return;
                }

                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.KZ = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public double BigBAF
        {
            get => SampleGroup?.BigBAF ?? default(int);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.BigBAF = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public bool TallyBySubPop
        {
            get => (CruiseMethodType.ThreePMethods.HasFlag(this.CruiseMethodType)) ? true : SampleGroup?.TallyBySubPop ?? default(bool);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.TallyBySubPop = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public bool UseExternalSampler
        {
            get => SampleGroup?.UseExternalSampler ?? default(bool);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.UseExternalSampler = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int MinKPI
        {
            get => SampleGroup?.MinKPI ?? default(int);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.MinKPI = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int MaxKPI
        {
            get => SampleGroup?.MaxKPI ?? default(int);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.MaxKPI = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public double SmallFPS
        {
            get => SampleGroup?.SmallFPS ?? default(double);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SmallFPS = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string SampleSelectorType
        {
            get => SampleGroup?.SampleSelectorType ?? DefaultSampleSelectorType;
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SampleSelectorType = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public string CruiseMethod => SampleGroup?.CruiseMethod;

        public CruiseMethodType CruiseMethodType => CruiseMethodTypeExtentions.StringToCruiseMethodType(this.CruiseMethod);

        public string DefaultSampleSelectorType
        {
            get
            {
                var method = CruiseMethod;
                if (method == CruiseMethods.STR)
                { return CruiseMethods.BLOCK_SAMPLER_TYPE; }
                else if (method == CruiseMethods.PCM || method == CruiseMethods.FCM)
                { return CruiseMethods.SYSTEMATIC_SAMPLER_TYPE; }
                return null;
            }
        }

        public bool CanSelectSampleSelectorType => Supports_SamplerType_Selection.Contains(CruiseMethod);
    }
}