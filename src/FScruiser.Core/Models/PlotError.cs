using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace FScruiser.Models
{
    public class PlotError : Model_Base
    {
        [Field("PlotID")]
        public string PlotID { get; set; }
    }
}