using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckCruise
{
    [Table("Tolerances")]
    public class TolerancesList
    {
        public string T_Element { get; set; }
        public string T_Tolerance { get; set; }
        public string T_Units { get; set; }
        public string T_AddParam { get; set; }
        public float T_Weight { get; set; }
        public float T_CUFTPGtolerance { get; set; }
        public float T_CUFTPNtolerance { get; set; }
        public float T_CUFTSGtolerance { get; set; }
        public float T_CUFTSNtolerance { get; set; }
        public float T_CUFTPSGtolerance { get; set; }
        public float T_CUFTPSNtolerance { get; set; }
        public float T_BDFTPGtolerance { get; set; }
        public float T_BDFTPNtolerance { get; set; }
        public float T_BDFTSGtolerance { get; set; }
        public float T_BDFTSNtolerance { get; set; }
        public float T_BDFTPSGtolerance { get; set; }
        public float T_BDFTPSNtolerance { get; set; }
        public int T_IncludeVolume { get; set; }
        public int T_BySpecies { get; set; }
        public int T_ByProduct { get; set; }
        public float T_ElementAccuracy { get; set; }
        public float T_OverallAccuracy { get; set; }
        public string T_DateStamp { get; set; }
    }
}
