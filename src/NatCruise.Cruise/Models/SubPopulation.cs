using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    public class SubPopulation
    {
        [Field("Species")]
        public string Species { get; set; }

        [Field("LiveDead")]
        public string LiveDead { get; set; }

        public override string ToString()
        {
            var liveDead = LiveDead;

            if (liveDead.ToUpper() != "L")
            {
                return $"{Species} - {liveDead}";
            }
            else
            {
                return Species;
            }
        }
    }
}