using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    public class PlotError : Model_Base
    {
        [Field("PlotID")]
        public string PlotID { get; set; }
    }
}