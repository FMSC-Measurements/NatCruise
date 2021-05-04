using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Design.Models
{
    [Table("SampleGroup")]
    public class SampleGroup : BindableBase
    {
        private string _sampleGroupCode;
        private string _description;
        private string _cutLeave;
        private string _uom;
        private string _primaryProduct;
        private string _secondaryProduct;
        private string _biomassProduct;
        private string _defaultLiveDead;
        private int _samplingFrequency;
        private int _insuranceFrequency;
        private int _kz;
        private int _bigBAF;
        private bool _tallyBySubPop;
        private int _minKPI;
        private int _maxKPI;
        private string _sampleSelectorType;
        private bool _useExternalSampler;
        private double _smallFPS;

        [PrimaryKeyField("SampleGroup_CN")]
        public int SampleGroup_CN { get; set; }

        public string SampleGroupID { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set => SetProperty(ref _sampleGroupCode, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string CutLeave
        {
            get => _cutLeave;
            set => SetProperty(ref _cutLeave, value);
        }

        public string UOM
        {
            get => _uom;
            set => SetProperty(ref _uom, value);
        }

        public string PrimaryProduct
        {
            get => _primaryProduct;
            set => SetProperty(ref _primaryProduct, value);
        }

        public string SecondaryProduct
        {
            get => _secondaryProduct;
            set => SetProperty(ref _secondaryProduct, value);
        }

        public string BiomassProduct
        {
            get => _biomassProduct;
            set => SetProperty(ref _biomassProduct, value);
        }

        public string DefaultLiveDead
        {
            get => _defaultLiveDead;
            set => SetProperty(ref _defaultLiveDead, value);
        }

        public int SamplingFrequency
        {
            get => _samplingFrequency;
            set => SetProperty(ref _samplingFrequency, value);
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

        public int BigBAF
        {
            get => _bigBAF;
            set => SetProperty(ref _bigBAF, value);
        }

        public bool TallyBySubPop
        {
            get => _tallyBySubPop;
            set => SetProperty(ref _tallyBySubPop, value);
        }

        public string SampleSelectorType
        {
            get => _sampleSelectorType;
            set => SetProperty(ref _sampleSelectorType, value);
        }

        public bool UseExternalSampler
        {
            get => _useExternalSampler;
            set => SetProperty(ref _useExternalSampler, value);
        }

        public int MinKPI
        {
            get => _minKPI;
            set => SetProperty(ref _minKPI, value);
        }

        public int MaxKPI
        {
            get => _maxKPI;
            set => SetProperty(ref _maxKPI, value);
        }

        public double SmallFPS
        {
            get => _smallFPS;
            set => SetProperty(ref _smallFPS, value);
        }
    }
}