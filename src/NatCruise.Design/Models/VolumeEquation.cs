using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Models
{
    [Table("VolumeEquation")]
    public class VolumeEquation
    {
        public string Species { get; set; }

        public string PrimaryProduct { get; set; }

        public string VolumeEquationNumber { get; set; }

        public double? StumpHeight { get; set; }

        public double? TopDIBPrimary { get; set; }

        public double? TopDIBSecondary { get; set; }

        public int? CalcTotal { get; set; }

        public int? CalcBoard { get; set; }

        public int? CalcCubic { get; set; }

        public int? CalcCord { get; set; }

        public int? CalcTopwood { get; set; }

        public int? CalcBiomass { get; set; }

        public double? Trim { get; set; }

        public int? SegmentationLogic { get; set; }

        public double? MinLogLengthPrimary { get; set; }

        public double? MaxLogLengthPrimary { get; set; }

        public double? MinLogLengthSecondary { get; set; }

        public double? MaxLogLengthSecondary { get; set; }

        public double? MinMerchLength { get; set; }

        public string Model { get; set; }

        public string CommonSpeciesName { get; set; }

        public int? MerchModFlag { get; set; }

        public int? EvenOddSegment { get; set; }
    }
}
