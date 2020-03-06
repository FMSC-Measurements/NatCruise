using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class CuttingUnit_Ex : CuttingUnit
    {
        [IgnoreField]
        public bool HasPlotStrata { get; set; }

        [IgnoreField]
        public bool HasTreeStrata { get; set; }

    }
}
