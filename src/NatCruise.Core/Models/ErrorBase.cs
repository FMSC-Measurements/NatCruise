namespace NatCruise.Models
{
    public abstract class ErrorBase
    {
        public string Field { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public bool IsResolved { get; set; }

        public string Resolution { get; set; }

        public string ResolutionInitials { get; set; }
    }
}