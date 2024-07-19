using CommunityToolkit.Mvvm.ComponentModel;
using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Models
{
    public class LogFieldValue : ObservableObject
    {
        private double? _valueReal;
        private int? _valueInt;
        //private bool? _valueBool;
        private string _valueText;

        public event EventHandler ValueChanged;

        public string LogID { get; set; }
        public string Field { get; set; }


        public string Heading { get; set; }

        public double? ValueReal
        {
            get => _valueReal;
            set
            {
                if (SetProperty(ref _valueReal, value))
                { RaiseValueChanged(); }
            }
        }

        public int? ValueInt
        {
            get => _valueInt;
            set
            {
                if (SetProperty(ref _valueInt, value))
                { RaiseValueChanged(); }
            }
        }

        //public bool? ValueBool
        //{
        //    get => _valueBool;
        //    set
        //    {
        //        if (SetProperty(ref _valueBool, value))
        //        { RaiseValueChanged(); }
        //    }
        //}

        public string ValueText
        {
            get => _valueText;
            set
            {
                if (SetProperty(ref _valueText, value))
                { RaiseValueChanged(); }
            }

        }

        [IgnoreField]
        public object Value
        {
            get
            {
                switch (Field)
                {
                    case (nameof(Log.LogNumber)):
                    case (nameof(Log.Length)):
                        {
                            return ValueInt;
                        }
                    case (nameof(Log.Grade)):
                    case (nameof(Log.ExportGrade)):
                        {
                            return ValueText;
                        }
                    case (nameof(Log.BoardFootRemoved)):
                    case (nameof(Log.BarkThickness)):
                    case (nameof(Log.CubicFootRemoved)):
                    case (nameof(Log.DIBClass)):
                    case (nameof(Log.GrossBoardFoot)):
                    case (nameof(Log.GrossCubicFoot)):
                    case (nameof(Log.LargeEndDiameter)):
                    case (nameof(Log.NetBoardFoot)):
                    case (nameof(Log.NetCubicFoot)):
                    case (nameof(Log.PercentRecoverable)):
                    case (nameof(Log.SeenDefect)):
                    case (nameof(Log.SmallEndDiameter)):
                        {
                            return ValueReal;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown Log Field{Field}");
                        }
                }
            }
            set
            {
                switch (Field)
                {
                    case (nameof(Log.LogNumber)):
                    case (nameof(Log.Length)):
                        {
                            ValueInt = Convert.ToInt32(value);
                            break;
                        }
                    case (nameof(Log.Grade)):
                    case (nameof(Log.ExportGrade)):
                        {
                            ValueText = Convert.ToString(value);
                            break;
                        }
                    case (nameof(Log.BoardFootRemoved)):
                    case (nameof(Log.BarkThickness)):
                    case (nameof(Log.CubicFootRemoved)):
                    case (nameof(Log.DIBClass)):
                    case (nameof(Log.GrossBoardFoot)):
                    case (nameof(Log.GrossCubicFoot)):
                    case (nameof(Log.LargeEndDiameter)):
                    case (nameof(Log.NetBoardFoot)):
                    case (nameof(Log.NetCubicFoot)):
                    case (nameof(Log.PercentRecoverable)):
                    case (nameof(Log.SeenDefect)):
                    case (nameof(Log.SmallEndDiameter)):
                        {
                            ValueReal = Convert.ToDouble(value);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown Log Field{Field}");
                        }
                }
                

            }
        }

        public void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(Value));
        }
    }
}
