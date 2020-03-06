namespace FScruiser.Validation
{
    public enum ValidationLevel { Error, Warning, Info }

    public class ValidationError
    {
        public ValidationError()
        {
        }

        public ValidationError(IValidationRule rule)
        {
            Property = rule.Property;
            Message = rule.Message;
            Level = rule.Level;
        }

        public string Property { get; set; }

        public string Message { get; set; }

        public ValidationLevel Level { get; set; }
    }
}