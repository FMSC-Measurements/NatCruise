using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    public class TallyPopulation_Plot : TallyPopulation
    {
        private int _plotTreeCount;

        [IgnoreField]
        public bool InCruise { get; set; }

        [IgnoreField]
        public bool IsEmpty { get; set; }

        [IgnoreField]
        public int PlotTreeCount
        {
            get => _plotTreeCount;
            set => SetProperty(ref _plotTreeCount, value);
        }
    }
}