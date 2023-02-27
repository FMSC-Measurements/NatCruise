using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;

namespace NatCruise.Models
{
    public class TreeFieldValue : BindableBase
    {
        private string _dbType;
        private string _field;
        private double? _valueReal;
        private int? _valueInt;
        private bool? _valueBool;
        private string _valueText;
        private TreeError _error;

        public event EventHandler ValueChanged;

        public string TreeID { get; set; }

        public string Field
        {
            get => _field;
            set
            {
                _field = value?.ToUpper();
            }
        }

        public string DBType
        {
            get => _dbType;
            set
            {
                _dbType = value?.ToUpper();
            }
        }

        public string Heading { get; set; }

        public double? ValueReal
        {
            get => _valueReal;
            set
            {
                SetProperty(ref _valueReal, value);
                RaiseValueChanged();
            }
        }

        public int? ValueInt
        {
            get => _valueInt;
            set
            {
                SetProperty(ref _valueInt, value);
                RaiseValueChanged();
            }
        }

        public bool? ValueBool
        {
            get => _valueBool;
            set
            {
                SetProperty(ref _valueBool, value);
                RaiseValueChanged();
            }
        }

        public string ValueText
        {
            get => _valueText;
            set
            {
                SetProperty(ref _valueText, value);
                RaiseValueChanged();
            }

        }

        public bool IsHidden { get; set; }

        public bool IsLocked { get; set; }

        public double? DefaultValueReal { get; set; }

        public int? DefaultValueInt { get; set; }

        public bool? DefaultValueBool { get; set; }

        public string DefaultValueText { get; set; }

        [IgnoreField]
        public object Value
        {
            get
            {
                switch (DBType)
                {
                    case "REAL":
                        { return ValueReal; }
                    case "INT":
                    case "INTEGER":
                        { return ValueInt; }
                    case "TEXT":
                        { return ValueText; }
                    case "BOOL":
                    case "BOOLEAN":
                        { return ValueBool; }
                    default:
                        return null;
                }
            }
            set
            {
                switch (DBType)
                {
                    case "REAL":
                        { ValueReal = (double?)value; break; }
                    case "INT":
                    case "INTEGER":
                        { ValueInt = (int?)value; break; }
                    case "TEXT":
                        { ValueText = (string)value; break; }
                    case "BOOL":
                    case "BOOLEAN":
                        { ValueBool = (bool?)value; break; }
                    default:
                        throw new InvalidOperationException($"Unable to set value with DbType of {DBType ?? "null"}");
                }
            }
        }

        [IgnoreField]
        public string StrDefaultValue
        {
            get
            {
                switch (DBType)
                {
                    case "REAL":
                        { return DefaultValueReal?.ToString(); }
                    case "INT":
                    case "INTEGER":
                        { return DefaultValueInt?.ToString(); }
                    case "TEXT":
                        { return DefaultValueText; }
                    case "BOOL":
                    case "BOOLEAN":
                        { return DefaultValueBool?.ToString(); }
                    default:
                        return null;
                }
            }
        }

        [IgnoreField]
        public string InputRegex { get; set; }

        [IgnoreField]
        public string DbError { get; set; }

        [IgnoreField]
        public TreeError Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        public void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}