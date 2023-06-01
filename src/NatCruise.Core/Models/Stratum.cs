using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Models
{
    [Table("Stratum")]
    public class Stratum : BindableBase
    {
        private string _stratumCode;
        private string _description;
        private string _hotKey;
        private string _method;
        private double _basalAreaFactor;
        private double _fixedPlotSize;
        private int _kz3ppnt;
        private int _samplingFrequency;
        private string _fbsCode;
        private string _yieldComponent;
        private string _fixCNTField;
        private IEnumerable<string> _errors;

        [PrimaryKeyField("Stratum_CN")]
        public int Stratum_CN { get; set; }

        public string StratumID { get; set; }

        public string StratumCode
        {
            get => _stratumCode;
            set => SetProperty(ref _stratumCode, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Method
        {
            get => _method;
            set => SetProperty(ref _method, value);
        }

        public double BasalAreaFactor
        {
            get => _basalAreaFactor;
            set => SetProperty(ref _basalAreaFactor, value);
        }

        public double FixedPlotSize
        {
            get => _fixedPlotSize;
            set => SetProperty(ref _fixedPlotSize, value);
        }

        public int KZ3PPNT
        {
            get => _kz3ppnt;
            set => SetProperty(ref _kz3ppnt, value);
        }

        public int SamplingFrequency
        {
            get => _samplingFrequency;
            set => SetProperty(ref _samplingFrequency, value);
        }

        public string HotKey
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

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public bool HasFieldData { get; set; }

        // TODO use CruiseMethods Look up table to get this value from the db?
        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);

        [IgnoreField]
        public IEnumerable<string> Errors
        {
            get => _errors;
            set => SetProperty(ref _errors, value);
        }

        public override string ToString()
        {
            return $"{StratumCode}: {Description}";
        }
    }
}