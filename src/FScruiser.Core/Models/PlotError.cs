using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class PlotError : Error_Base
    {
        [Field("PlotID")]
        public string PlotID { get; set; }
    }
}
