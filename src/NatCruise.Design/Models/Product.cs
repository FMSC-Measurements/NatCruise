namespace NatCruise.Design.Models
{
    public class Product
    {
        public string ProductCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{ProductCode} - {FriendlyName}";
        }
    }
}