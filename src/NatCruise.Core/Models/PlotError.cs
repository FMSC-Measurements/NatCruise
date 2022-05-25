using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table]
    public class PlotError : BindableBase
    {
        public string PlotID { get; set; }

        public string Message { get; set; }

        public string Level { get; set; }
    }
}