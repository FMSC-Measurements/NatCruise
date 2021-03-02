namespace NatCruise.Design.Models
{
    public class Purpose
    {
        public string PurposeCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{PurposeCode} - {FriendlyName}";
        }
    }
}