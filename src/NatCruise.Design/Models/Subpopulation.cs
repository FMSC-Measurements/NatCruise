using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Design.Models
{
    [Table("SubPopulation")]
    public class Subpopulation : BindableBase
    {
        private string _liveDead;

        [PrimaryKeyField("Subpopulation_CN")]
        public int Subpopulation_CN { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead
        {
            get => _liveDead;
            set => SetProperty(ref _liveDead, value);
        }
    }
}