using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    public class TallyPopulation_Plot : TallyPopulation
    {
        [IgnoreField]
        public bool InCruise { get; set; }

        [IgnoreField]
        public bool IsEmpty { get; set; }
    }
}