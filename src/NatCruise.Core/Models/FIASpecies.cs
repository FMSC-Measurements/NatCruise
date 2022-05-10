using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    [Table("LK_FIA")]
    public class FIASpecies
    {
        public string FIACode { get; set; }

        public string CommonName { get; set; }

        public override string ToString()
        {
            return $"{FIACode} - {CommonName}";
        }
    }
}