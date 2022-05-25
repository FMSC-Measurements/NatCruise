using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    [Table("LK_Product")]
    public class Product
    {
        [Field("Product")]
        public string ProductCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{ProductCode} - {FriendlyName}";
        }
    }
}