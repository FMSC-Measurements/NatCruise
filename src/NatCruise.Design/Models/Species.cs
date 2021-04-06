using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("Species")]
    public class Species
    {
        public string SpeciesCode { get; set; }

        public string ContractSpecies { get; set; }

        public string FIACode { get; set; }
    }
}