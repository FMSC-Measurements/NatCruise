using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    public class PlotError : Model_Base
    {
        public string PlotID { get; set; }

        public string Message { get; set; }

        public string Level { get; set; }
    }
}