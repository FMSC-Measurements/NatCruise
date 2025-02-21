using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.Models
{
    public class ToleranceVolumeOptions : ICloneable
    {
        
        public bool IncludeVolumes { get; set; }

        public VolumeGroupingOption VolumeGroupingOption { get; set; }

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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
