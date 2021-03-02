using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    public class SubPopulation
    {
        public string SpeciesCode { get; set; }
        public string LiveDead { get; set; }

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