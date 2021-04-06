using FMSC.ORM.EntityModel.Attributes;
using System;

namespace NatCruise.Design.Models
{
    [Table("StratumDefault")]
    public class StratumDefault
    {
        public string StratumDefaultID { get; set; }

        public string Region { get; set; }

        public string Forest { get; set; }

        public string District { get; set; }

        public string StratumCode { get; set; }

        public string Description { get; set; }

        public string Method { get; set; }

        public double? BasalAreaFactor { get; set; }

        public double? FixedPlotSize { get; set; }

        public int? KZ3PPNT { get; set; }

        public int? SamplingFrequency { get; set; }

        public string Hotkey { get; set; }

        public string FBSCode { get; set; }

        public string YieldComponent { get; set; }

        public string FixCNTField { get; set; }
    }
}