using Prism.Mvvm;

namespace NatCruise.Models
{
    public class TreeFieldSetup : BindableBase
    {
        private string _stratumCode;
        private string _sampleGroupCode;
        private TreeField _field;
        private int? _fieldOrder;
        private bool _isHidden;
        private bool _isLocked;
        private int? _defaultValueInt;
        private double? _defaultValueReal;
        private bool? _defaultValueBool;
        private string _defaultValueText;

        public string StratumCode
        {
            get => _stratumCode;
            set => SetProperty(ref _stratumCode, value);
        }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set => SetProperty(ref _sampleGroupCode, value);
        }

        public TreeField Field
        {
            get => _field;
            set => SetProperty(ref _field, value);
        }

        public int? FieldOrder
        {
            get;
            set;
        }

        public bool IsHidden
        {
            get => _isHidden;
            set => SetProperty(ref _isHidden, value);
        }

        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        public int? DefaultValueInt
        {
            get => _defaultValueInt;
            set => SetProperty(ref _defaultValueInt, value);
        }

        public double? DefaultValueReal
        {
            get => _defaultValueReal;
            set => SetProperty(ref _defaultValueReal, value);
        }

        public bool? DefaultValueBool
        {
            get => _defaultValueBool;
            set => SetProperty(ref _defaultValueBool, value);
        }

        public string DefaultValueText
        {
            get => _defaultValueText;
            set => SetProperty(ref _defaultValueText, value);
        }

        public override string ToString()
        {
            return Field.ToString();
        }
    }
}