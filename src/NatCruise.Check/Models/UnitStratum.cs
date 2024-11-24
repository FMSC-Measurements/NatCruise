using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckCruise.Models
{
    public class UnitStratum
    {
        public string CuttingUnitCode { get; set; }
        public string StratumCode { get; set; }
        public long CuttingUnit_CN { get; set; }
        public long Stratum_CN { get; set; }
        public string Method { get; set; }
    }
}
