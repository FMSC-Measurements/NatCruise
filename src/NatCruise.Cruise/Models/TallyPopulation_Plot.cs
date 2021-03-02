using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    public class TallyPopulation_Plot : TallyPopulation_Base
    {
        [IgnoreField]
        public bool InCruise { get; set; }

        [IgnoreField]
        public bool IsEmpty { get; set; }
    }
}