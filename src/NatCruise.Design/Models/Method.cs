namespace NatCruise.Design.Models
{
    public class Method
    {
        public string MethodCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return MethodCode;
        }
    }
}