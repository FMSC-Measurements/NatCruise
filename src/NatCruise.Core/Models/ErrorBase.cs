namespace NatCruise.Models
{
    public abstract class ErrorBase
    {
        public const string LEVEL_WARNING = "W";
        public const string LEVEL_ERROR = "E";

        public string Field { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public bool IsResolved { get; set; }

        public string Resolution { get; set; }

        public string ResolutionInitials { get; set; }
    }
}