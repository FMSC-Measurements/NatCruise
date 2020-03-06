using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;
using FScruiser.Util;
using System;

namespace FScruiser.Models
{
    public class TreeFieldValue : INPC_Base, IHasTreeID
    {
        private string _dbType;
        private string _field;
        private double? _valueReal;
        private int? _valueInt;
        private bool? _valueBool;
        private string _valueText;

        [Field("TreeID")]
        public string TreeID { get; set; }

        [Field("Field")]
        public string Field
        {
            get => _field;
            set
            {
                _field = value?.ToUpper();
            }
        }

        [Field("DbType")]
        public string DBType
        {
            get => _dbType;
            set
            {
                _dbType = value?.ToUpper();
            }
        }

        [Field("Heading")]
        public string Heading { get; set; }

        [Field("ValueReal")]
        public double? ValueReal
        {
            get => _valueReal;
            set => SetValue(ref _valueReal, value);
        }

        [Field("ValueInt")]
        public int? ValueInt
        {
            get => _valueInt;
            set => SetValue(ref _valueInt, value);
        }

        [Field("ValueBool")]
        public bool? ValueBool
        {
            get => _valueBool;
            set => SetValue(ref _valueBool, value);
        }

        [Field("ValueText")]
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