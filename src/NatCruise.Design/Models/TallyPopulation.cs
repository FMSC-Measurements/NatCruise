using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Design.Models
{
    [Table("TallyPopulation")]
    public class TallyPopulation : BindableBase
    {
        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string Species { get; set; }

        public string LiveDead { get; set; }

        public string HotKey { get; set; }

        public string Description { get; set; }
    }
}