using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Design.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class SampleGroupDetailViewModel : ValidationViewModelBase
    {
        protected readonly string[] Frequency_Based_CruiseMethods = new[] {CruiseMethods.STR, CruiseMethods.PCM, CruiseMethods.FCM, CruiseMethods.S3P, };
        protected readonly string[] SampleSelector_Type_Options = new[] { CruiseMethods.BLOCK_SAMPLER_TYPE, CruiseMethods.SYSTEMATIC_SAMPLER_TYPE, };
        protected readonly string[] Supports_SamplerType_Selection = new[] { CruiseMethods.STR, }; 

        private SampleGroup _sampleGroup;
        private string _method;
        private IEnumerable<Product> _productOptions;

        public SampleGroupDetailViewModel(IDataserviceProvider dataserviceProvider, ISetupInfoDataservice setupInfo, SampleGroupValidator validator)
            : base(validator)
        {
            var sampleGroupDataservice = dataserviceProvider.GetDataservice<ISampleGroupDataservice>();
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SetupDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));

            ProductOptions = SetupDataservice.GetProducts();
        }

        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }

        public IEnumerable<string> DefaultLiveDeadOptions { get; } = new[] { "L", "D" };

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

                RaisePropertyChanged(nameof(IsSTR));
                RaisePropertyChanged(nameof(Is3P));
                RaisePropertyChanged(nameof(IsVariableRadious));
                RaisePropertyChanged(nameof(CanSelectSampleSelectorType));
                RaisePropertyChanged(nameof(DefaultSampleSelectorType));
                RaisePropertyChanged(nameof(IsFrequencyBased));
            }
        }

        private void SampleGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var sampleGroup = (SampleGroup)sender;
            SampleGroupDataservice.UpdateSampleGroup(sampleGroup);
        }

        public string SampleGroupCode
        {
            get => SampleGroup?.SampleGroupCode;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SampleGroupCode = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string Description
        {
            get => SampleGroup?.Description;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.Description = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string CutLeave
        {
            get => SampleGroup?.CutLeave;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.CutLeave = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string UOM
        {
            get => SampleGroup?.CutLeave;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.CutLeave = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string PrimaryProduct
        {
            get => SampleGroup?.PrimaryProduct;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.PrimaryProduct = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string SecondaryProduct
        {
            get => SampleGroup?.SecondaryProduct;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SecondaryProduct = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string BiomassProduct
        {
            get => SampleGroup?.BiomassProduct;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.BiomassProduct = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string DefaultLiveDead
        {
            get => SampleGroup?.CutLeave;
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.DefaultLiveDead = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public int SamplingFrequency
        {
            get => SampleGroup?.SamplingFrequency ?? default(int);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SamplingFrequency = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public int InsuranceFrequency
        {
            get => SampleGroup?.InsuranceFrequency ?? default(int);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.InsuranceFrequency = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public int KZ
        {
            get => SampleGroup?.KZ ?? default(int);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.KZ = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public int BigBAF
        {
            get => SampleGroup?.BigBAF ?? default(int);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.BigBAF = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public bool TallyBySubPop
        {
            get => SampleGroup?.TallyBySubPop ?? default(bool);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.TallyBySubPop = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public bool UseExternalSampler
        {
            get => SampleGroup?.UseExternalSampler ?? default(bool);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.UseExternalSampler = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public int MinKPI
        {
            get => SampleGroup?.MinKPI ?? default(int);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.MinKPI = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public int MaxKPI
        {
            get => SampleGroup?.MaxKPI ?? default(int);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.MaxKPI = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public double SmallFPS
        {
            get => SampleGroup?.SmallFPS ?? default(double);
            set => SetPropertyAndValidate(SampleGroup, value, (sg, x) => sg.SmallFPS = x, sg => SampleGroupDataservice.UpdateSampleGroup(sg));
        }

        public string SampleSelectorType
        {
            get => SampleGroup?.SampleSelectorType ?? DefaultSampleSelectorType;
            set
            {
                SampleGroup.SampleSelectorType = value;
            }
        }


        public string CruiseMethod => SampleGroup?.CruiseMethod;


        public string Method
        {
            get => _method;
            protected set
            {
                SetProperty(ref _method, value);
            }
        }

        public bool IsSTR => Method == CruiseMethods.STR;
        public bool IsFrequencyBased => Frequency_Based_CruiseMethods.Contains(CruiseMethod);
        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(CruiseMethod) || CruiseMethod == CruiseMethods.S3P;

        public bool IsVariableRadious => CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(CruiseMethod);

        public IEnumerable<string> SampleSelectorTypeOptions => SampleSelector_Type_Options;

        public string DefaultSampleSelectorType
        {
            get
            {
                if(Method == CruiseMethods.STR)
                { return CruiseMethods.BLOCK_SAMPLER_TYPE; }
                else if (Method == CruiseMethods.PCM || Method == CruiseMethods.FCM)
                { return CruiseMethods.SYSTEMATIC_SAMPLER_TYPE; }
                return null;
            }
        }

        public bool CanSelectSampleSelectorType => Supports_SamplerType_Selection.Contains(CruiseMethod);

        

        //protected override void Refresh(CruiseManagerNavigationParamiters navParams)
        //{
        //    var sampleGroupCode = navParams.SampleGroupCode;
        //    var stratum = navParams.StratumCode;

        //    SampleGroup = SampleGroupDataservice.GetSampleGroup(stratum, sampleGroupCode);

        //    Method = SampleGroupDataservice.GetMethod(stratum);
        //    RaisePropertyChanged(nameof(Is3P));
        //    RaisePropertyChanged(nameof(IsSTR));
        //    RaisePropertyChanged(nameof(IsVariableRadious));
        //}

        public override void Load()
        {
            var sampleGroup = SampleGroup;
            if (sampleGroup == null) { return; }

            //switch (method)
            //{
            //    case CruiseMethods.STR:
            //    case CruiseMethods.PCM:
            //    case CruiseMethods.FCM:
            //        {
            //            SampleSelectorTypeOptions = new[]
            //              {
            //                SampleGroupTableDefinition.SAMPLESELECTORTYPE_BLOCKSELECTER,
            //                SampleGroupTableDefinition.SAMPLESELECTORTYPE_SYSTEMATICSELECTER,
            //                SampleGroupTableDefinition.SAMPLESELECTORTYPE_CLICKERSELECTER
            //            };
            //            break;
            //        }
            //}
        }
    }
}