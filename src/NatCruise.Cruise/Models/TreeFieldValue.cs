using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;
using System;

namespace NatCruise.Cruise.Models
{
    public class TreeFieldValue : Model_Base, IHasTreeID
    {
        private string _dbType;
        private string _field;
        private double? _valueReal;
        private int? _valueInt;
        private bool? _valueBool;
        private string _valueText;

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
            set => SetValue(ref _valueReal, value);
        }

        public int? ValueInt
        {
            get => _valueInt;
            set => SetValue(ref _valueInt, value);
        }

        public bool? ValueBool
        {
            get => _valueBool;
            set => SetValue(ref _valueBool, value);
        }

        public string ValueText
        {
            get => _valueText;
            set => SetValue(ref _valueText, value);
        }

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
    }
}