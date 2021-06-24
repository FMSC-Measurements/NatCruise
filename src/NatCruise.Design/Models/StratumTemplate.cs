using Prism.Mvvm;

namespace NatCruise.Design.Models
{
    public class StratumTemplate : BindableBase
    {
        private string _stratumCode;
        private string _method;
        private double? _basalAreaFactor;
        private double? _fixedPlotSize;
        private int? _kz3ppnt;
        private int? _samplingFrequency;
        private string _hotKey;
        private string _fbsCode;
        private string _yieldComponent;
        private string _fixCNTField;
        private string _stratumTemplateName;

        public string StratumTemplateName
        {
            get => _stratumTemplateName;
            set => SetProperty(ref _stratumTemplateName, value);
        }

        public string StratumCode
        {
            get => _stratumCode;
            set => SetProperty(ref _stratumCode, value);
        }

        public string Method
        {
            get => _method;
            set => SetProperty(ref _method, value);
        }

        public double? BasalAreaFactor
        {
            get => _basalAreaFactor;
            set => SetProperty(ref _basalAreaFactor, value);
        }

        public double? FixedPlotSize
        {
            get => _fixedPlotSize;
            set => SetProperty(ref _fixedPlotSize, value);
        }

        public int? KZ3PPNT
        {
            get => _kz3ppnt;
            set => SetProperty(ref _kz3ppnt, value);
        }

        public int? SamplingFrequency
        {
            get => _samplingFrequency;
            set => SetProperty(ref _samplingFrequency, value);
        }

        public string Hotkey
        {
            get => _hotKey;
            set => SetProperty(ref _hotKey, value);
        }

        public string FBSCode
        {
            get => _fbsCode;
            set => SetProperty(ref _fbsCode, value);
        }

        public string YieldComponent
        {
            get => _yieldComponent;
            set => SetProperty(ref _yieldComponent, value);
        }

        public string FixCNTField
        {
            get => _fixCNTField;
            set => SetProperty(ref _fixCNTField, value);
        }

        public override string ToString()
        {
            return StratumTemplateName;
        }
    }
}