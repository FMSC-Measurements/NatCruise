using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class SampleGroupDetailViewModel : ValidationViewModelBase
    {
        protected readonly string[] SampleSelector_Type_Options = new[] { CruiseMethods.BLOCK_SAMPLER_TYPE, CruiseMethods.SYSTEMATIC_SAMPLER_TYPE, CruiseMethods.CLICKER_SAMPLER_TYPE };
        protected readonly string[] Supports_SamplerType_Selection = new[] { CruiseMethods.STR, };

        private SampleGroup _sampleGroup;
        private IEnumerable<Product> _productOptions;

        public SampleGroupDetailViewModel(ISampleGroupDataservice sampleGroupDataservice, ISetupInfoDataservice setupInfo, SampleGroupValidator validator)
            : base(validator)
        {
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SetupDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));

            ProductOptions = SetupDataservice.GetProducts();
        }

        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }

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
                //OnSampleGroupChanged(value);
                ValidateAll(value);

                RaisePropertyChanged(nameof(SampleGroupCode));
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(CutLeave));
                RaisePropertyChanged(nameof(UOM));
                RaisePropertyChanged(nameof(PrimaryProduct));
                RaisePropertyChanged(nameof(SecondaryProduct));
                RaisePropertyChanged(nameof(BiomassProduct));
                RaisePropertyChanged(nameof(DefaultLiveDead));
                RaisePropertyChanged(nameof(SamplingFrequency));
                RaisePropertyChanged(nameof(InsuranceFrequency));
                RaisePropertyChanged(nameof(KZ));
                RaisePropertyChanged(nameof(BigBAF));
                RaisePropertyChanged(nameof(TallyBySubPop));
                RaisePropertyChanged(nameof(UseExternalSampler));
                RaisePropertyChanged(nameof(MinKPI));
                RaisePropertyChanged(nameof(MaxKPI));
                RaisePropertyChanged(nameof(SmallFPS));
                RaisePropertyChanged(nameof(SampleSelectorType));

                RaisePropertyChanged(nameof(CruiseMethod));
                RaisePropertyChanged(nameof(DefaultSampleSelectorType));
            }
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
                    RaisePropertyChanged(nameof(SampleGroupCode));
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
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SamplingFrequency = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int InsuranceFrequency
        {
            get => SampleGroup?.InsuranceFrequency ?? default(int);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.InsuranceFrequency = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int KZ
        {
            get => SampleGroup?.KZ ?? default(int);
            set
            {
                SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.KZ = x);
                SampleGroupDataservice.UpdateSampleGroup(SampleGroup);
            }
        }

        public int BigBAF
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