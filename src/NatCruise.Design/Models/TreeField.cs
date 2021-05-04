using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("TreeField")]
    public class TreeField
    {
        public string Field { get; set; }

        public string Heading { get; set; }

        public string DefaultHeading { get; set; }

        public string DbType { get; set; }

        public override string ToString()
        {
            return Heading ?? DefaultHeading;
        }
    }
}