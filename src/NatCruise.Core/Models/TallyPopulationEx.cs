using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    // TODO can this be consolidated into TallyPopulation or broken into a separate model
    public class TallyPopulationEx : TallyPopulation
    {


        public bool IsClickerTally => SampleSelectorType == CruiseMethods.CLICKER_SAMPLER_TYPE;

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public string SampleSelectorType { get; set; }

        // depreciated in favor of using SampleSelectorType with "ClickerSelecter"
        // IsClickerTally will reflect this
        //public bool UseExternalSampler { get; set; }

        
    }
}