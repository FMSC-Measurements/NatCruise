using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace NatCruise.Models
{
    [Table("SampleGroup")]
    public class SampleGroup : BindableBase
    {
        private int _bigBAF;
        private string _biomassProduct;
        private string _cutLeave;
        private string _defaultLiveDead;
        private string _description;
        private IEnumerable<string> _errors;
        private int _insuranceFrequency;
        private int _kz;
        private int _maxKPI;
        private int _minKPI;
        private string _primaryProduct;
        private string _sampleGroupCode;
        private string _sampleSelectorType;
        private int _samplingFrequency;
        private string _secondaryProduct;
        private double _smallFPS;
        private bool _tallyBySubPop;
        private string _uom;
        private bool _useExternalSampler;

        [PrimaryKeyField("SampleGroup_CN")]
        public int SampleGroup_CN { get; set; }

        public string SampleGroupID { get; set; }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set => SetProperty(ref _sampleGroupCode, value);
        }

        public string StratumCode { get; set; }

        public int BigBAF
        {
            get => _bigBAF;
            set => SetProperty(ref _bigBAF, value);
        }

        public string BiomassProduct
        {
            get => _biomassProduct;
            set => SetProperty(ref _biomassProduct, value);
        }

        public string CutLeave
        {
            get => _cutLeave;
            set => SetProperty(ref _cutLeave, value);
        }

        public string DefaultLiveDead
        {
            get => _defaultLiveDead;
            set => SetProperty(ref _defaultLiveDead, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        

        public int InsuranceFrequency
        {
            get => _insuranceFrequency;
            set => SetProperty(ref _insuranceFrequency, value);
        }

        public int KZ
        {
            get => _kz;
            set => SetProperty(ref _kz, value);
        }

        public int MaxKPI
        {
            get => _maxKPI;
            set => SetProperty(ref _maxKPI, value);
        }

        public int MinKPI
        {
            get => _minKPI;
            set => SetProperty(ref _minKPI, value);
        }

        public string PrimaryProduct
        {
            get => _primaryProduct;
            set => SetProperty(ref _primaryProduct, value);
        }

        public string SampleSelectorType
        {
            get => _sampleSelectorType;
            set => SetProperty(ref _sampleSelectorType, value);
        }

        public int SamplingFrequency
        {
            get => _samplingFrequency;
            set => SetProperty(ref _samplingFrequency, value);
        }

        public string SecondaryProduct
        {
            get => _secondaryProduct;
            set => SetProperty(ref _secondaryProduct, value);
        }

        public double SmallFPS
        {
            get => _smallFPS;
            set => SetProperty(ref _smallFPS, value);
        }

        public bool TallyBySubPop
        {
            get => _tallyBySubPop;
            set => SetProperty(ref _tallyBySubPop, value);
        }

        public string UOM
        {
            get => _uom;
            set => SetProperty(ref _uom, value);
        }

        public bool UseExternalSampler
        {
            get => _useExternalSampler;
            set => SetProperty(ref _useExternalSampler, value);
        }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public string CruiseMethod { get; set; }

        //[Obsolete]
        //public string Method { get; set; }
		
		[IgnoreField]
        public IEnumerable<string> Errors
        {
            get => _errors;
            set => SetProperty(ref _errors, value);
        }

        public override string ToString()
        {
            return $"{SampleGroupCode}: {Description}";
        }
    }
}