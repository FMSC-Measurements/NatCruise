using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("SubPopulation")]
    public class Subpopulation : BindableBase
    {
        private string _liveDead;
        private int _interval;
        private int _min;
        private int _max;

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

        public int IntervalSize
        {
            get => _interval;
            set
            {
                if (value < 0) { return; }
                SetProperty(ref _interval, value);
            }
        }

        public int Min
        {
            get => _min;
            set
            {
                if(value < 0) { return; }
                SetProperty(ref _min, value);
            }
        }

        public int Max
        {
            get => _max;
            set
            {
                if (value < 0) { return; }
                SetProperty(ref _max, value);
            }
        }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public bool HasTrees
        {
            get;
            set;
        }


        public override string ToString()
        {
            var liveDead = LiveDead;

            if (liveDead != null && liveDead.ToUpper() != "L")
            {
                return $"{SpeciesCode} - {liveDead}";
            }
            else
            {
                return SpeciesCode;
            }
        }

    }
}