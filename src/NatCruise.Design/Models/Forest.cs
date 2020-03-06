namespace NatCruise.Design.Models
{
    public class Forest
    {
        public string ForestCode { get; set; }

        public string ForestName { get; set; }

        public override string ToString()
        {
            return $"{ForestCode} - {ForestName}";
        }
    }
}