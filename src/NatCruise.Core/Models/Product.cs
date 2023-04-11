using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    [Table("LK_Product")]
    public class Product
    {
        public static readonly Product AnyProductOption = new Product
        {
            FriendlyName = "",
            ProductCode = null,
        };

        [Field("Product")]
        public string ProductCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{ProductCode} - {FriendlyName}";
        }
    }
}