using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    public class TallyPopulation_Plot : TallyPopulation_Base
    {
        [IgnoreField]
        public bool InCruise { get; set; }

        [IgnoreField]
        public bool IsEmpty { get; set; }
    }
}