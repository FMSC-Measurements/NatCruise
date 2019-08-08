namespace NatCruise.Wpf.Models
{
    public class UOM
    {
        public string UOMCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{UOMCode} - {FriendlyName}";
        }
    }
}