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

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string SubpopulationID { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string StratumCode { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string SampleGroupCode { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string SpeciesCode { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Always)]
        public string LiveDead
        {
            get => _liveDead;
            set => SetProperty(ref _liveDead, value);
        }
    }
}